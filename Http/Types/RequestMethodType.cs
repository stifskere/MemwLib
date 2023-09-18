using JetBrains.Annotations;

namespace MemwLib.Http.Types;

[PublicAPI, Flags]
public enum RequestMethodType
{
    Options,
    Get,
    Head,
    Post,
    Patch,
    Put,
    Delete,
    Trace,
    Connect
}