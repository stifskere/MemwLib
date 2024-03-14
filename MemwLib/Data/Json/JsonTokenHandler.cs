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
                return ResultOrThrow(throwOnError, "JSON Objects must start with { and end with }.", false);

            foreach (string kvp in SplitAtLevel(payload[1..^1]))
            {
                List<string> splitKvp = SplitAtLevel(kvp, ':');

                if (splitKvp.Count != 2)
                    return ResultOrThrow(throwOnError, "Invalid JSON key-value-pair.", false);
                
                if (CheckPrimitive(splitKvp[0], throwOnError) != PrimitiveType.String)
                    return ResultOrThrow(throwOnError, "JSON Object keys must be Strings.", false);

                if (!VerifyJson(splitKvp[1], throwOnError))
                    return false;
            }

            return true;
        }

        private static bool CheckList(string payload, bool throwOnError)
        {
            payload = payload.Trim();
            
            return !payload.IsEnclosedBy('[', ']') 
                ? ResultOrThrow(throwOnError, "JSON Lists must start with [ and end with ].", false) 
                : SplitAtLevel(payload[1..^1]).All(value => VerifyJson(value, throwOnError));
        }

        private static PrimitiveType? CheckPrimitive(string payload, bool throwOnError)
        {
            string possibleErrorMessage = "Supported JSON primitive types are String, Number and Null.";
            
            payload = payload.Trim();

            if (payload.IsEnclosedWithSameOf('"'))
                return PrimitiveType.String;

            if (payload.Split(' ').Length != 1)
                return ResultOrThrow<PrimitiveType?>(throwOnError, possibleErrorMessage, null);

            if (double.TryParse(payload, null, out _))
                return PrimitiveType.Number;

            if (bool.TryParse(payload, out _))
                return PrimitiveType.Boolean;

            if (payload == "null")
                return PrimitiveType.Null;

            return ResultOrThrow<PrimitiveType?>(throwOnError, possibleErrorMessage, null);
        }

        private static TReturn ResultOrThrow<TReturn>(bool throwOnError, string error, TReturn result)
            => throwOnError
                ? throw new InvalidJsonConstraintException(error)
                : result;
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
    
    private static List<string> SplitAtLevel(string payload, char splitBy = ',')
    {
        List<string> result = new();
        bool insideSquareBrackets = false;
        bool insideCurlyBraces = false;
        bool insideString = false;
        int startIndex = 0;

        for (int i = 0; i < payload.Length; i++)
        {
            switch (payload[i])
            {
                case '"' when i <= 0 || payload[i - 1] != '\\':
                    insideString = !insideString;
                    break;
                case '[' when !insideString:
                    insideSquareBrackets = true;
                    break;
                case ']' when !insideString:
                    insideSquareBrackets = false;
                    break;
                case '{' when !insideString:
                    insideCurlyBraces = true;
                    break;
                case '}' when !insideString:
                    insideCurlyBraces = false;
                    break;
                default:
                {
                    if (payload[i] != splitBy || insideSquareBrackets || insideCurlyBraces || insideString)
                        break;
                    
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