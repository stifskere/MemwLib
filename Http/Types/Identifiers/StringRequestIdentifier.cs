using System.Text.RegularExpressions;

namespace MemwLib.Http.Types.Identifiers;

internal class StringRequestIdentifier : IRequestIdentifier
{
    public required RequestMethodType RequestMethod { get; init; }
    public required string Route { get; set; }
    
    public void AppendRoute(string start)
    {
        Route = $"{start}{Route}";
    }
}