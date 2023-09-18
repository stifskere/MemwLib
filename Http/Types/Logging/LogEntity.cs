using JetBrains.Annotations;
using MemwLib.Http.Types.Entities;

namespace MemwLib.Http.Types.Logging;

[PublicAPI]
public class LogEntity
{
    public RequestMethodType RequestType { get; }
    public int ResponseCode { get; }
    
    public LogEntity(RequestEntity req, ResponseEntity res)
    {
        RequestType = req.RequestType;
        ResponseCode = res.ResponseCode;
    }
}