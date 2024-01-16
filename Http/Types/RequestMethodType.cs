using JetBrains.Annotations;

namespace MemwLib.Http.Types;

/// <summary>The HTTP request method type enumerator.</summary>
[PublicAPI, Flags]
public enum RequestMethodType
{
    /// <summary>OPTIONS: Used to retrieve information about the communication options for the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/OPTIONS"/>
    Options = 1,

    /// <summary>GET: Used to retrieve data from the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/GET"/>
    Get = 1 << 1,

    /// <summary>HEAD: Similar to GET, but without the response body. Used to check resource headers.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/HEAD"/>
    Head = 1 << 2,

    /// <summary>POST: Used to send data to the target resource for processing.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST"/>
    Post = 1 << 3,
    
    /// <summary>PATCH: Used to apply partial modifications to a resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/PATCH"/>
    Patch = 1 << 4,

    /// <summary>PUT: Used to replace the target resource with the provided payload.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/PUT"/>
    Put = 1 << 5,

    /// <summary>DELETE: Used to request the removal of the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/DELETE"/>
    Delete = 1 << 6,

    /// <summary>TRACE: Used to perform a diagnostic test along the path to the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/TRACE"/>
    Trace = 1 << 7,

    /// <summary>CONNECT: Used to establish a network connection to the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/CONNECT"/>
    Connect = 1 << 8
}
