using System.Text.RegularExpressions;

namespace MemwLib.Http.Types;

public interface IRequestIdentifier
{
    public RequestMethodType RequestType { get; init; }
}

public class RegexRequestIdentifier : IRequestIdentifier
{
    public required RequestMethodType RequestType { get; init; }
    public required Regex Path { get; init; }
}

public class StringRequestIdentifier : IRequestIdentifier
{
    public required RequestMethodType RequestType { get; init; }
    public required string Path { get; init; }
}