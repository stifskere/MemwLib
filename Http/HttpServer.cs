using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.CoreUtils;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Identifiers;
using MemwLib.Http.Types.Logging;

namespace MemwLib.Http;

/// <summary>HTTP server that behaves like express.js and means easier use.</summary>
[UnsupportedOSPlatform("browser")]
public sealed class HttpServer
{
    private readonly TcpListener _listener;
    private readonly CancellationToken _cancellationToken;
    private readonly Dictionary<IRequestIdentifier, RequestDelegate> _endpoints = new();
    private readonly List<MiddleWareDelegate> _globalMiddleware = new();

    /// <summary>
    /// Contains the count of successful requests
    /// that returned 100-299 this server handled.
    /// </summary>
    [PublicAPI]
    public long SuccessfulRequests { get; private set; }
    
    /// <summary>
    /// Contains the count of failed requests
    /// that returned 300-599 this server handled.
    /// </summary>
    [PublicAPI]
    public long FailedRequests { get; private set; }

    /// <summary>Event that will be fired each time this server logged something.</summary>
    [PublicAPI]
    public event LogDelegate OnLog = _ => { };
    
    /// <summary>Default constructor for HttpServer.</summary>
    /// <param name="address">The IP address where to start this server.</param>
    /// <param name="port">The port where to start the server in the specified IP.</param>
    /// <param name="cancellationToken">Token to stop the server on cancellation.</param>
    /// <inheritdoc cref="TcpListener.Start()"/>
    /// <exception cref="OutOfMemoryException">There is not enough memory available to start this server.</exception>
    public HttpServer(IPAddress address, ushort port, CancellationToken? cancellationToken = null)
    {
        _cancellationToken = cancellationToken ?? CancellationToken.None;
        _listener = new TcpListener(address, port);
        _listener.Start();
        new Thread(ThreadHandler).Start();
    }

    private void ThreadHandler()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            TcpClient connection = _listener.AcceptTcpClient();
            NetworkStream incomingStream = connection.GetStream();

            while (!incomingStream.DataAvailable) { }
            
            byte[] incomingStreamData = new byte[connection.Available];
            _ = incomingStream.Read(incomingStreamData, 0, incomingStreamData.Length);
            
            ResponseEntity? responseEntity = null;
            RequestEntity parsedRequest;

            try
            {
                parsedRequest = new RequestEntity(Encoding.ASCII.GetString(incomingStreamData));
            }
            catch
            {
                WriteAndClose(new ResponseEntity(400));
                OnLog(new LogMessage(LogType.FailedRequest, "Client sent a malformed request."));
                continue;
            }
            
            foreach ((IRequestIdentifier identifier, RequestDelegate handler) in _endpoints)
            {
                try
                {
                    if (identifier is RegexRequestIdentifier regexIdentifier)
                    {
                        Match match = regexIdentifier.Route.Match(parsedRequest.Path.Route);
                        
                        if (!match.Success)
                            continue;
                        
                        parsedRequest.CapturedGroups.Add(match.Groups);
                    }
                    
                    if (identifier is StringRequestIdentifier stringIdentifier 
                        && stringIdentifier.Route != parsedRequest.Path.Route)
                        continue;

                    if (identifier is MixedIdentifier mixedIdentifier)
                    {
                        if (!mixedIdentifier.Equals(parsedRequest.Path.Route))
                            continue;
                        
                        parsedRequest.CapturedGroups 
                            = mixedIdentifier.GetRegexGroups(parsedRequest.Path.Route);
                    }
                    
                    if (!TypeUtils.TargetEnumHasFlags(identifier.RequestMethod, parsedRequest.RequestType))
                    {
                        responseEntity 
                            = new ResponseEntity(405, $"Cannot {parsedRequest.RequestType.ToString().ToUpper()}: {parsedRequest.Path}");
                        break;
                    }
                    
                    IResponsible next;
                    
                    RunMiddleware(_globalMiddleware);
                    RunMiddlewareFromAttributes(handler.Method.DeclaringType?.GetCustomAttributes<UsesMiddlewareAttribute>());
                    RunMiddlewareFromAttributes(handler.Method.GetCustomAttributes<UsesMiddlewareAttribute>());
                    
                    responseEntity ??= handler(parsedRequest);
                    break;

                    void RunMiddleware(IEnumerable<MiddleWareDelegate>? middleWarePieces)
                    {
                        if (middleWarePieces is null)
                            return;

                        foreach (MiddleWareDelegate middleware in middleWarePieces)
                        {
                            next = middleware(parsedRequest);

                            if (next is not ResponseEntity resEntity)
                                continue;

                            responseEntity = resEntity;
                            break;
                        }
                    }
                    
                    void RunMiddlewareFromAttributes(IEnumerable<UsesMiddlewareAttribute>? middlewarePieces)
                    {
                        if (middlewarePieces is null)
                            return; 
                        
                        foreach (UsesMiddlewareAttribute middleware in middlewarePieces)
                        {
                            next = middleware.Target(parsedRequest);

                            if (next is not ResponseEntity resEntity) 
                                continue;
                        
                            responseEntity = resEntity;
                            break;
                        }
                    }
                }
                catch (Exception error)
                {
                    responseEntity = new ResponseEntity(500, error.ToString());
                    OnLog(new LogMessage(LogType.Error, $"Exception thrown: {error.Message}{(error.Source != null ? $":{error.Source}" : string.Empty)}"));
                    break;
                }
            }

            responseEntity ??= new ResponseEntity(404);
            
            WriteAndClose(responseEntity);
            
            OnLog(new LogMessage(responseEntity.IsSuccessfulResponse
                ? LogType.SuccessfulRequest
                : LogType.FailedRequest, 
                $"{parsedRequest.Path.Route} returned {responseEntity.ResponseCode} {responseEntity.Hint}"));
            
            
            continue;

            void WriteAndClose(ResponseEntity entity)
            {
                entity.Headers["Content-Length"] ??= entity.Body.Length.ToString();
                _ = entity.IsSuccessfulResponse ? SuccessfulRequests++ : FailedRequests++;
                incomingStream.Write(entity.ToArray());
                incomingStream.Close();
                connection.Close();
            }
        }
    }

    /// <summary>Adds a global middleware piece that will be run before every endpoint.</summary>
    /// <param name="handler">The middleware piece to execute.</param>
    /// <returns>The same server instance for sake of chaining declarations.</returns>
    /// <remarks>This middleware will run before any other individual middleware, and will be called in order of declaration.</remarks>
    [PublicAPI]
    public HttpServer AddGlobalMiddleware(MiddleWareDelegate handler)
    {
        _globalMiddleware.Add(handler);
        return this;
    }
    
    /// <summary>Registers an endpoint to this server that runs the handler if the method and route match.</summary>
    /// <param name="requestMethod">The request method flags that will trigger this handler.</param>
    /// <param name="regexPattern">The regex pattern for matching the route to trigger this handler.</param>
    /// <param name="handler">What will this handler do when triggered.</param>
    /// <remarks>
    /// If two regex pattern conflict, the one that's added
    /// first will run while leaving the remaining useless.
    /// </remarks>
    [PublicAPI]
    public void AddEndpoint(RequestMethodType requestMethod, Regex regexPattern, RequestDelegate handler)
    {
        _endpoints.Add(new RegexRequestIdentifier { RequestMethod = requestMethod, Route = regexPattern }, handler);
    }
    
    /// <summary>Registers an endpoint to this server that runs the handler if the method and route match.</summary>
    /// <param name="requestMethod">The request method flags that will trigger this handler.</param>
    /// <param name="path">The literal path for matching the route to trigger this handler.</param>
    /// <param name="handler">What will this handler do when triggered.</param>
    /// <remarks>
    /// If two path conflicts or is the same as another,
    /// the one that's added first will run while leaving the remaining useless.
    /// </remarks>
    [PublicAPI]
    public void AddEndpoint(RequestMethodType requestMethod, string path, RequestDelegate handler)
    {
        _endpoints.Add(new StringRequestIdentifier { RequestMethod = requestMethod, Route = path }, handler);
    }

    /// <summary>Adds a route group defined by a RouteGroupAttribute.</summary>
    /// <typeparam name="TGroup">The group class type.</typeparam>
    /// <remarks>The target type must not be internal in order to get all the members from it.</remarks>
    [PublicAPI]
    public void AddGroup<TGroup>()
        => AddGroup(typeof(TGroup));
    
    /// <summary>Adds a route group defined by a RouteGroupAttribute.</summary>
    /// <param name="type">The group class type.</param>
    /// <remarks>The target type must not be internal in order to get all the members from it.</remarks>
    [PublicAPI]
    public void AddGroup(Type type)
    {
        RouteGroupAttribute? prefix = type.GetCustomAttribute<RouteGroupAttribute>();
        
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding))
        {
            GroupMemberAttribute? postFix = method.GetCustomAttribute<GroupMemberAttribute>();

            if (postFix is null)
                return;
            
            _endpoints.Add(new MixedIdentifier
            {
                Prefix = prefix is not null ? prefix.AsRegex ? new Regex(prefix.Route!) : prefix.Route : null, 
                Postfix = postFix.AsRegex ? new Regex(postFix.Route) : postFix.Route,
                RequestMethod = postFix.RequestMethod
            }, (RequestDelegate)Delegate.CreateDelegate(typeof(RequestDelegate), method));
        }
    }
}