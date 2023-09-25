using System.Text.RegularExpressions;

namespace MemwLib.Http.Types.Identifiers;

internal class RegexRequestIdentifier : IRequestIdentifier
{
    public required RequestMethodType RequestType { get; init; }
    public required Regex Path { get; init; }
}