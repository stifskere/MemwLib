using System.Diagnostics;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.CoreUtils;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Configuration;
using MemwLib.Http.Types.Content.Implementations;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Identifiers;
using MemwLib.Http.Types.Logging;
using MemwLib.Http.Types.SSL;

namespace MemwLib.Http;

/// <summary>HTTP server that behaves like express.js and means easier use.</summary>
[UnsupportedOSPlatform("browser"), PublicAPI]
public sealed class HttpServer
{
    private readonly TcpListener _listener;
    private readonly CancellationToken _cancellationToken;
    private readonly Dictionary<IRequestIdentifier, RequestDelegate> _endpoints = new();
    private readonly List<MiddleWareDelegate> _globalMiddleware = new();
    private X509Certificate? _serverCertificate;
    
    /// <summary>Whether this server instance is on development or production mode.</summary>
    public ServerStates State { get; set; }
    
    /// <summary>
    /// Contains the count of successful requests
    /// that returned 100-299 this server handled.
    /// </summary>
    public long SuccessfulRequests { get; private set; }
    
    /// <summary>
    /// Contains the count of failed requests
    /// that returned 300-599 this server handled.
    /// </summary>
    public long FailedRequests { get; private set; }

    /// <summary>Event that will be fired each time this server logged something.</summary>
    public event LogDelegate OnLog = _ => { };
    
    /// <summary>Default constructor for HttpServer.</summary>
    /// <param name="config">Tells the server how it should behave.</param>
    /// <param name="cancellationToken">Token to stop the server on cancellation.</param>
    /// <inheritdoc cref="TcpListener.Start()"/>
    /// <exception cref="OutOfMemoryException">There is not enough memory available to start this server.</exception>
    public HttpServer(HttpServerConfig? config = null, CancellationToken? cancellationToken = null)
    {
        config ??= new HttpServerConfig();
        
        if (config is { SslBehavior: SslBehavior.DoNotUseCertificateIfNotFound, SslCertificate: not null })
                _serverCertificate = new X509Certificate(config.SslCertificate.CertificatePath,
                    config.SslCertificate.CertificatePassword);
        
        if (config.SslBehavior == SslBehavior.AlwaysFindAndUseCertificate)
            _serverCertificate = config.SslCertificate is not null 
                ? new X509Certificate(config.SslCertificate.CertificatePath, config.SslCertificate.CertificatePassword) 
                : CertificateManager.GenerateOrRetrieveCertificate();
        
        State = config.ServerState;
        
        _cancellationToken = cancellationToken ?? CancellationToken.None;
        
        _listener = new TcpListener(config.Address, config.Port);
        _listener.Start();
        
        new Thread(ThreadHandler).Start();
    }

    private void ThreadHandler()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            TcpClient connection = _listener.AcceptTcpClient();
            using Stream incomingStream = _serverCertificate is not null
                ? new SslStream(connection.GetStream(), false)
                : connection.GetStream();

            try
            {
                if (incomingStream is SslStream sslStream)
                {
                    Debug.Assert(_serverCertificate is not null);
                    
                    sslStream.AuthenticateAsServer(_serverCertificate, false, SslProtocols.Tls13, true);
                }
            }
            catch (Exception)
            {
                OnLog(new LogMessage(LogType.Error, "There was some problem while authenticating with the SSL protocol."));
                continue;
            }
            
            ResponseEntity? responseEntity = null;
            RequestEntity parsedRequest;

            try
            {
                parsedRequest = new RequestEntity(new StreamReader(incomingStream));
            }
            catch
            {
                WriteAndClose(new ResponseEntity(ResponseCodes.BadRequest));
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
                    
                    if (!TypeUtils.TargetEnumHasFlags(identifier.RequestMethod, parsedRequest.RequestMethod))
                    {
                        responseEntity = new ResponseEntity(ResponseCodes.MethodNotAllowed, 
                            new MethodNotAllowedBody(parsedRequest.RequestMethod, parsedRequest.Path.Route));
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
                    responseEntity = new ResponseEntity(ResponseCodes.InternalServerError, new ExceptionBody(error, State == ServerStates.Development));
                    OnLog(new LogMessage(LogType.Error, $"Exception thrown: {error.Message}{(error.Source != null ? $":{error.Source}" : string.Empty)}"));
                    break;
                }
            }

            responseEntity ??= new ResponseEntity(ResponseCodes.NotFound);
            
            WriteAndClose(responseEntity);
            
            OnLog(new LogMessage(responseEntity.IsSuccessfulResponse
                ? LogType.SuccessfulRequest
                : LogType.FailedRequest, 
                $"{parsedRequest.Path.Route} returned {responseEntity.ResponseCode.GetCode()} {responseEntity.ResponseCode.GetName()}"));
            
            
            continue;

            void WriteAndClose(ResponseEntity entity)
            {
                entity.Headers["Content-Length"] ??= entity.Body.Length.ToString();
                entity.Headers["Content-Type"] ??= entity.Body.ContentType;
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
    public HttpServer AddEndpoint(RequestMethodType requestMethod, Regex regexPattern, RequestDelegate handler)
    {
        _endpoints.Add(new RegexRequestIdentifier { RequestMethod = requestMethod, Route = regexPattern }, handler);
        return this;
    }
    
    /// <summary>Registers an endpoint to this server that runs the handler if the method and route match.</summary>
    /// <param name="requestMethod">The request method flags that will trigger this handler.</param>
    /// <param name="path">The literal path for matching the route to trigger this handler.</param>
    /// <param name="handler">What will this handler do when triggered.</param>
    /// <remarks>
    /// If two path conflicts or is the same as another,
    /// the one that's added first will run while leaving the remaining useless.
    /// </remarks>
    public HttpServer AddEndpoint(RequestMethodType requestMethod, string path, RequestDelegate handler)
    {
        _endpoints.Add(new StringRequestIdentifier { RequestMethod = requestMethod, Route = path }, handler);
        return this;
    }

    /// <summary>Adds a route group defined by a RouteGroupAttribute.</summary>
    /// <typeparam name="TGroup">The group class type.</typeparam>
    /// <remarks>The target type must not be internal in order to get all the members from it.</remarks>
    public HttpServer AddGroup<TGroup>()
        => AddGroup(typeof(TGroup));
    
    /// <summary>Adds a route group defined by a RouteGroupAttribute.</summary>
    /// <param name="type">The group class type.</param>
    /// <remarks>The target type must not be internal in order to get all the members from it.</remarks>
    public HttpServer AddGroup(Type type)
    {
        RouteGroupAttribute? prefix = type.GetCustomAttribute<RouteGroupAttribute>();
        
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding))
        {
            GroupMemberAttribute? postFix = method.GetCustomAttribute<GroupMemberAttribute>();

            if (postFix is null)
                return this;
            
            _endpoints.Add(new MixedIdentifier
            {
                Prefix = prefix is not null ? prefix.AsRegex ? new Regex(prefix.Route!) : prefix.Route : null, 
                Postfix = postFix.AsRegex ? new Regex(postFix.Route) : postFix.Route,
                RequestMethod = postFix.RequestMethod
            }, (RequestDelegate)Delegate.CreateDelegate(typeof(RequestDelegate), method));
        }

        return this;
    }
}