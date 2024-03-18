using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using MemwLib.Http.Types.Content.Views.Exceptions;

namespace MemwLib.Http.Types.Content.Views;

public static class ViewParser
{
    [RequiresUnreferencedCode("Not compatible with trimming, might be unable to find existing symbols.")]
    public static string Parse(string content, Dictionary<string, object?> references)
    {
        Regex invalidSymbolRegex = new(@"\b(foreach|endforeach|if|elif|endif|for|endfor|new|\d\S*)\b");

        references = references
            .ToDictionary(k => k.Key.Trim(), v => v.Value);
        
        foreach (string key in references.Keys)
        {
            if (invalidSymbolRegex.IsMatch(key))
                throw new InvalidReferenceNameException(key);
        }
        
        MatchReplacer replacer = new(content);
        
        foreach (Match expression in Regex.Matches(content, @"\$\{(?<content>(?:[^{}]|\{[^{}]*\})*)\}"))
            replacer.Replace(
                expression, 
                ViewHelper.ProcessComplexExpression(
                    expression.Groups["content"].Value,
                    Location.Find(expression, content),
                    references
                )
            );

        return replacer.ReplacedTarget;
    }
}