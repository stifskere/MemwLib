using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types;
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
                if (!identifier.RequestType.HasFlag(parsedRequest.RequestType))
                    continue;
                
                try
                {
                    if (identifier is RegexRequestIdentifier regexIdentifier)
                    {
                        if (!regexIdentifier.Path.IsMatch(parsedRequest.Path.Route))
                            continue;

                        responseEntity = handler(parsedRequest);
                        break;
                    }

                    if (identifier is StringRequestIdentifier stringIdentifier)
                    {
                        if (stringIdentifier.Path != parsedRequest.Path.Route)
                            continue;

                        responseEntity = handler(parsedRequest);
                        break;
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
        => _endpoints.Add(new RegexRequestIdentifier {RequestType = requestMethod, Path = regexPattern}, handler);

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
        => _endpoints.Add(new StringRequestIdentifier { RequestType = requestMethod, Path = path }, handler);
}