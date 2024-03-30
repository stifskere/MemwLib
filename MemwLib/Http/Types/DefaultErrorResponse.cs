using JetBrains.Annotations;
using MemwLib.Data.Json.Attributes;

namespace MemwLib.Http.Types;

// this is provisional till I implement views.
[UsedImplicitly]
internal class DefaultErrorResponse(string message, Exception? ex = null)
{
    [JsonProperty("error")]
    public static bool Error => true;

    [JsonProperty("reason")]
    public string Message => message;

    [JsonProperty("exception")]
    public string? Exception { get; } = ex?.ToString();
}