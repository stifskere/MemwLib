#if DEBUG

using MemwLib.CoreUtils;
using MemwLib.Data.Json.Exceptions;

namespace MemwLib.Data.Json;

internal static class JsonTokenHandler
{
    public static class Assigns
    {
        public static Dictionary<string, object?> HandleObject(string payload)
        {
            payload = payload.Trim(' ', '\n');
            
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
            payload = payload.Trim(' ', '\n');
            
            if (!payload.StartsWith('[') || !payload.EndsWith(']'))
                throw new InvalidJsonSequenceException('[', ']');

            List<object?> result = new();
            
            foreach (string element in SplitAtLevel(payload[1..^1]))
            {
                string trimmed = element.Trim(' ', '\n');
                
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
            payload = payload.Trim(' ', '\n');
            
            object? inferred = TypeUtils.InferTypeOf(payload);

            if (inferred is not string s) 
                return inferred;

            if (s.IsEnclosedWithSameOf('"')) 
                return s[1..^1];
            
            for (int i = 1; i < s.Length - 1; i++)
                if (s[i] == '"' && s[i - 1] == '\\' && i > 1 && s[i - 2] != '\\')
                    return s;
                
            throw new InvalidJsonSequenceException('"');
        }
    }

    public static class Validators
    {
        private enum PrimitiveType
        {
            String = 1,
            Number = 1 << 1,
            Boolean = 1 << 2,
            Null = 1 << 3
        }
        
        public static bool VerifyJson(string payload, bool throwOnError)
        {
            payload = payload.Trim();

            return payload[0] switch
            {
                '{' => CheckObject(payload, throwOnError),
                '[' => CheckList(payload, throwOnError),
                _ => CheckPrimitive(payload, throwOnError) is not null
            };
        }
        
        private static bool CheckObject(string payload, bool throwOnError)
        {
            payload = payload.Trim();
            
            if (!payload.IsEnclosedBy('{', '}'))
                return FalseOrThrow(throwOnError, "JSON Objects must start with { and end with }.");

            foreach (string kvp in SplitAtLevel(payload[1..^1]))
            {
                // TODO: rethink this logic
                string[] splitKvp = kvp.Split(':');
                splitKvp = new[] { splitKvp[0], string.Join(':', splitKvp[1..]) };
                
                if (CheckPrimitive(splitKvp[0], throwOnError) != PrimitiveType.String)
                    return FalseOrThrow(throwOnError, "JSON Object keys must be Strings");

                if (!VerifyJson(splitKvp[1], throwOnError))
                    return false;
            }

            return true;
        }

        private static bool CheckList(string payload, bool throwOnError)
        {
            payload = payload.Trim();
            
            return !payload.IsEnclosedBy('[', ']') 
                ? FalseOrThrow(throwOnError, "JSON Lists must start with [ and end with ].") 
                : SplitAtLevel(payload[1..^1]).All(value => VerifyJson(value, throwOnError));
        }

        private static PrimitiveType? CheckPrimitive(string payload, bool throwOnError)
        {
            InvalidJsonConstraintException unsupportedTypeException
                = new("Supported JSON primitive types are String, Number and Null.");
            
            payload = payload.Trim();

            if (payload.IsEnclosedWithSameOf('"'))
                return PrimitiveType.String;

            if (payload.Split(' ').Length != 1)
                return throwOnError
                    ? throw unsupportedTypeException
                    : null;

            if (double.TryParse(payload, null, out _))
                return PrimitiveType.Number;

            if (bool.TryParse(payload, out _))
                return PrimitiveType.Boolean;

            if (payload == "null")
                return PrimitiveType.Null;

            return throwOnError 
                ? throw unsupportedTypeException
                : null;
        }

        private static bool FalseOrThrow(bool throwOnError, string error)
            => throwOnError
                ? throw new InvalidJsonConstraintException(error)
                : false;
    }

    public static string PrettifyJson(string json, int level)
    {
        if (level < 1)
            throw new ArgumentException("Indentation can't be less than 1.", nameof(level));
        
        int indent = 0;

        return json.Aggregate(string.Empty, (current, character) => current + character switch
        {
            ':' => $"{character} ",
            '{' => $"{character}\n{new string(' ', indent += level)}",
            '[' => $"{character}\n{new string(' ', indent += level)}",
            '}' or ']' => $"\n{new string(' ', indent -= level)}{character}",
            ',' => $"{character}\n{new string(' ', indent)}",
            _ => character
        });
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
}

#endif