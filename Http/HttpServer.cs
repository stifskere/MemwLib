using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Logging;

namespace MemwLib.Http;

public class HttpServer
{
    private readonly TcpListener _listener;
    private readonly CancellationToken _cancellationToken;
    private readonly Dictionary<IRequestIdentifier, RequestDelegate> _endpoints = new();

    [PublicAPI]
    public long SuccessfulRequests { get; private set; }
    [PublicAPI]
    public long FailedRequests { get; private set; }

    [PublicAPI]
    public event LogDelegate OnLog = _ => { };
    
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
                continue;
            }

            bool isErrored = false;
            
            foreach ((IRequestIdentifier identifier, RequestDelegate handler) in _endpoints)
            {
                if (!identifier.RequestType.HasFlag(parsedRequest.RequestType))
                    continue;

                try
                {
                    if (identifier is RegexRequestIdentifier regexIdentifier)
                    {
                        if (!regexIdentifier.Path.IsMatch(parsedRequest.Path))
                            continue;

                        responseEntity = handler(parsedRequest);
                        break;
                    }

                    if (identifier is StringRequestIdentifier stringIdentifier)
                    {
                        if (stringIdentifier.Path != parsedRequest.Path)
                            continue;

                        responseEntity = handler(parsedRequest);
                        break;
                    }
                }
                catch (Exception error)
                {
                    WriteAndClose(new ResponseEntity(500, error.ToString()), parsedRequest.Path);
                    OnLog(new LogMessage(LogType.Error, $"Exception thrown: {error.Message}{(error.Source != null ? $":{error.Source}" : string.Empty)}"));
                    isErrored = true;
                    break;
                }
            }

            responseEntity ??= new ResponseEntity(404);
            
            if (!isErrored)
                WriteAndClose(responseEntity, parsedRequest.Path);
            
            continue;

            void WriteAndClose(ResponseEntity entity, string? queryPath = null)
            {
                OnLog(new LogMessage(
                    entity.IsSuccessfulResponse 
                        ? LogType.SuccessfulRequest 
                        : LogType.FailedRequest, 
                    $"[{entity.ResponseCode}] {(queryPath != null ? $"[{queryPath}]" : string.Empty)} {entity.Hint}"
                    ));
                
                _ = entity.IsSuccessfulResponse ? SuccessfulRequests++ : FailedRequests++;
                incomingStream.Write(Encoding.ASCII.GetBytes((string)entity));
                incomingStream.Close();
                connection.Close();
            }
        }
    }

    [PublicAPI]
    public void AddEndpoint(RequestMethodType requestMethod, Regex regexPattern, RequestDelegate handler)
        => _endpoints.Add(new RegexRequestIdentifier {RequestType = requestMethod, Path = regexPattern}, handler);

    [PublicAPI]
    public void AddEndpoint(RequestMethodType requestMethod, string path, RequestDelegate handler)
        => _endpoints.Add(new StringRequestIdentifier { RequestType = requestMethod, Path = path }, handler);
}