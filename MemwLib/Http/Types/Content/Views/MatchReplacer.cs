using System.Text.RegularExpressions;

namespace MemwLib.Http.Types.Content.Views;

public class MatchReplacer(string target)
{
    private int _lengthDiff;

    public string ReplacedTarget => target;
        
    public void Replace(Capture match, object? to)
    {
        string sub = (target[..(match.Index - _lengthDiff)] +
                      target[(match.Index + match.Length - _lengthDiff)..])
            .Insert(match.Index - _lengthDiff, to?.ToString() ?? "null");

        _lengthDiff += target.Length - sub.Length;
        target = sub;
    }
}