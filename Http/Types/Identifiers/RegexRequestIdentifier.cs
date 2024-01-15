using System.Text.RegularExpressions;

namespace MemwLib.Http.Types.Identifiers;

internal class RegexRequestIdentifier : IRequestIdentifier
{
    public required RequestMethodType RequestMethod { get; init; }

    public required Regex Route { get; init; }
}