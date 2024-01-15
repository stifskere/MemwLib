using JetBrains.Annotations;

namespace MemwLib.Http.Types;

/// <summary>The HTTP request method type enumerator.</summary>
[PublicAPI, Flags]
public enum RequestMethodType
{
    /// <summary>OPTIONS: Used to retrieve information about the communication options for the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/OPTIONS"/>
    Options,

    /// <summary>GET: Used to retrieve data from the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/GET"/>
    Get,

    /// <summary>HEAD: Similar to GET, but without the response body. Used to check resource headers.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/HEAD"/>
    Head,

    /// <summary>POST: Used to send data to the target resource for processing.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/POST"/>
    Post,

    /// <summary>PATCH: Used to apply partial modifications to a resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/PATCH"/>
    Patch,

    /// <summary>PUT: Used to replace the target resource with the provided payload.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/PUT"/>
    Put,

    /// <summary>DELETE: Used to request the removal of the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/DELETE"/>
    Delete,

    /// <summary>TRACE: Used to perform a diagnostic test along the path to the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/TRACE"/>
    Trace,

    /// <summary>CONNECT: Used to establish a network connection to the target resource.</summary>
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Methods/CONNECT"/>
    Connect
}
