using MemwLib.CoreUtils;
using MemwLib.Data.Json.Exceptions;

namespace MemwLib.Data.Json;

internal static class JsonTokenHandler
{
    public static Dictionary<string, object?> HandleObject(string payload)
    {
        payload = payload.Trim();
        
        if (!payload.StartsWith('{') || !payload.EndsWith('}'))
            throw new InvalidJsonSequenceException('{', '}');

        Dictionary<string, object?> result = new();
        
        foreach (string kvp in SplitAtLevel(payload[1..^1]))
        {
            string[] splitKvp = kvp.Split(':');
            
            if (HandlePrimitive(splitKvp[0].Trim()) is not string key)
                throw new InvalidJsonConstraintException("A non string key was found.");

            string value = string.Join(':', splitKvp[1..]).Trim();
            
            if (value.StartsWith('{'))
                result.Add(key, HandleObject(value));
            else if (value.StartsWith('['))
                result.Add(key, HandleList(value));
            else
                result.Add(key, HandlePrimitive(value));
        }

        return result;
    }

    public static object?[] HandleList(string payload)
    {
        payload = payload.Trim();
        
        if (!payload.StartsWith('[') || !payload.EndsWith(']'))
            throw new InvalidJsonSequenceException('[', ']');

        List<object?> result = new();
        
        foreach (string element in SplitAtLevel(payload[1..^1]))
        {
            string trimmed = element.Trim();
            
            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
                result.Add(HandleList(element));
            else if (trimmed.StartsWith('{') && trimmed.EndsWith('}'))
                result.Add(HandleObject(element));
            else
                result.Add(HandlePrimitive(element));
        }

        return result.ToArray();
    }

    public static object? HandlePrimitive(string payload)
    {
        payload = payload.Trim();
        
        object? inferred = TypeUtils.InferTypeOf(payload);

        if (inferred is not string s) 
            return inferred;
        
        if (!s.IsEnclosedWithAnyOf('"'))
            throw new InvalidJsonSequenceException('"');
        
        return s[1..^1];
    }
    
    private static List<string> SplitAtLevel(string payload)
    {
        List<string> result = new();
        bool insideSquareBrackets = false;
        bool insideCurlyBraces = false;
        int startIndex = 0;

        for (int i = 0; i < payload.Length; i++)
        {
            switch (payload[i])
            {
                case '[':
                    insideSquareBrackets = true;
                    break;
                case ']':
                    insideSquareBrackets = false;
                    break;
                case '{':
                    insideCurlyBraces = true;
                    break;
                case '}':
                    insideCurlyBraces = false;
                    break;
                case ',' when !insideSquareBrackets && !insideCurlyBraces:
                {
                    string substring = payload.Substring(startIndex, i - startIndex).Trim();
                    result.Add(substring);
                    startIndex = i + 1;
                    break;
                }
            }
        }

        string lastSubstring = payload[startIndex..].Trim();
        
        if (!string.IsNullOrEmpty(lastSubstring)) 
            result.Add(lastSubstring);

        return result;
    }

    public static bool VerifyStringKeys(string input)
    {
        Stack<char> stack = new();
        bool expectingKey = false;
        bool insideKey = false;

        // TODO: handle inside arrays
        
        string total = string.Empty;
        foreach (char character in input)
        {
            total += character;
            
            if (insideKey)
            {
                if (character is '"')
                    insideKey = false;
                
                continue;
            }
            
            if (character is '{' or '[')
                stack.Push(character);
        
            else if (character is '{' or '[' or ',')
                expectingKey = true;

            else if (character is ':')
                expectingKey = false;

            else if (expectingKey && character is not (' ' or '"' or '\n'))
            {
                Console.WriteLine(total);
                return false;
            }
            
            else
                insideKey = true;
        }

        return !expectingKey && !insideKey;
    }
    
    public static bool VerifyJson(string input, bool throwIfError)
    {
        Stack<char> stack = new();

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '"')
            {
                if (stack.Count == 0 || stack.Peek() != '"')
                    stack.Push('"');
                else
                    stack.Pop();
            }
            else if (input[i] is '{' or '[' && (stack.Count == 0 || stack.Peek() != '"'))
                stack.Push(input[i]);
            else if (input[i] is '}' or ']' && (stack.Count == 0 || stack.Peek() != '"'))
            {
                if ((stack.Count == 0 || stack.Pop() != GetMatchingOpeningBracket(input[i])) &&
                    (throwIfError || (i > 0 && input[i - 1] != '"' && input[i - 1] != '}' && input[i - 1] != ']')))
                    throw new UnexpectedJsonEoiException(input[i], GetMatchingOpeningBracket(input[i]), i);
            }
            else if (stack.Count == 0 && input[i] is '}' or ']')
                return throwIfError ? throw new UnexpectedJsonEoiException(input[i], '}', i) : false;
        }

        return stack.Count > 0 && throwIfError
            ? throw new UnexpectedJsonEoiException(stack.Peek(), '\0', input.Length - 1)
            : stack.Count == 0;

        static char GetMatchingOpeningBracket(char closingBracket) 
            => closingBracket == '}' ? '{' : '[';
    }
}