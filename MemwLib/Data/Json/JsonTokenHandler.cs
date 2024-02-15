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
            {
                for (int i = 1; i < s.Length - 1; i++)
                    if (s[i] == '"' && s[i - 1] == '\\' && i > 1 && s[i - 2] != '\\')
                        return s;
                
                throw new InvalidJsonSequenceException('"');
            }
            
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
    }

    public static class Validators
    {
        public static bool VerifyJson(string payload, bool throwOnError)
        {
            bool isValid = false;
            payload = payload.Trim();
            
            for (int iterator = 0; iterator < payload.Length; iterator++)
            {
                if (payload.IsEnclosedBy('{', '}'))
                    isValid = CheckObject(ref payload, ref iterator, throwOnError);
                else if (payload.IsEnclosedBy('[', ']'))
                    isValid = CheckList(ref payload, ref iterator, throwOnError);
                else
                    isValid = CheckPrimitive(ref payload, ref iterator, throwOnError);
            }

            return isValid;
        }
        
        private static bool CheckObject(ref string payload, ref int iterator, bool throwOnError)
        {
            iterator++;
            bool isKey = true;

            while (iterator < payload.Length)
            {
                if (payload[iterator] == ' ')
                {
                    iterator++;
                    continue;
                }

                if (payload[iterator] == '}')
                {
                    iterator++;
                    break;
                }
                
                if (isKey)
                {
                    if (payload[iterator] != '"')
                    {
                        return throwOnError
                            ? throw new InvalidJsonConstraintException("Object keys must be strings.")
                            : false;
                    }
                    
                    iterator++;
                    
                    bool escaped = false;
                    while (iterator < payload.Length)
                    {
                        char nextChar = payload[iterator++];
                        
                        if (nextChar == '\\' && !escaped)
                        {
                            escaped = true;
                            continue;
                        }
                        
                        if (nextChar == '"' && !escaped)
                        {
                            isKey = false; 
                            break;
                        }
                        
                        escaped = false;
                    }
                }
                else
                {
                    if (payload[iterator] != ':')
                    {
                        return throwOnError
                            ? throw new UnexpectedJsonEoiException(payload[iterator], ":", iterator)
                            : false;
                    }
                    
                    iterator++;
                    
                    while (iterator < payload.Length && payload[iterator] == ' ')
                        iterator++;
                    
                    switch (payload[iterator])
                    {
                        case '{':
                        {
                            if (!CheckObject(ref payload, ref iterator, throwOnError))
                                return false;
                            break;
                        }
                        case '[':
                        {
                            if (!CheckList(ref payload, ref iterator, throwOnError))
                                return false;
                            break;
                        }
                        default:
                        {
                            if (!CheckPrimitive(ref payload, ref iterator, throwOnError))
                                return false;
                            break;
                        }
                    }

                    iterator++;
                    isKey = true;
                }
            }
            
            return throwOnError 
                ? throw new UnexpectedJsonEoiException(payload[iterator], "}", iterator) 
                : false;
        }

        private static bool CheckList(ref string payload, ref int iterator, bool throwOnError)
        {
            bool needsToFindComa = false;
            iterator++;
            
            for (; iterator < payload.Length; iterator++)
            {
                if (payload[iterator] == ']')
                {
                    if (!needsToFindComa)
                        return throwOnError 
                            ? throw new UnexpectedJsonEoiException(payload[iterator], new []{ "number", "\"", "true", "false", "null", "]", "[", "{" }, iterator) 
                            : false;
                    
                    iterator++;
                    return true;
                }

                if (needsToFindComa && payload[iterator] != ' ' && payload[iterator] != ',')
                    return throwOnError
                        ? throw new UnexpectedJsonEoiException(payload[iterator], ",", iterator)
                        : false;
                
                bool exit;
                switch (payload[iterator])
                {
                    case ',':
                        if (!needsToFindComa)
                            return throwOnError 
                                ? throw new UnexpectedJsonEoiException(payload[iterator], new []{ "number", "\"", "true", "false", "null", "]", "[", "{" }, iterator) 
                                : false;

                        needsToFindComa = false;
                        continue;
                    case ' ':
                        continue;
                    case '{':
                        exit = CheckObject(ref payload, ref iterator, throwOnError);
                        needsToFindComa = true;
                        break;
                    case '[':
                        exit = CheckList(ref payload, ref iterator, throwOnError);
                        needsToFindComa = true;
                        break;
                    default:
                        exit = CheckPrimitive(ref payload, ref iterator, throwOnError);
                        needsToFindComa = true;
                        break;
                }

                if (!exit)
                    return false;
            }
            
            return throwOnError 
                ? throw new UnexpectedJsonEoiException(' ', "]", payload.Length - 1) 
                : false;
        }

        private static bool CheckPrimitive(ref string payload, ref int iterator, bool throwOnError)
        {
            string content = string.Empty;
            
            for (; iterator < payload.Length; iterator++)
            {
                if (payload[iterator] is ':' or ',' or '}' or ']')
                    break;

                content += payload[iterator];
            }

            content = content.Trim();

            if (string.IsNullOrEmpty(content))
                return throwOnError 
                    ? throw new UnexpectedJsonEoiException(payload[iterator], new []{ "number", "\"", "true", "false", "null" }, iterator) 
                    : false;
            
            if (content is "true" or "false")
                return true;

            if (content == "null")
                return true;
            
            if (content.IsEnclosedWithSameOf('"'))
            {
                for (int i = 1; i < content.Length - 1; i++)
                    if (content[i] == '"' && (content[i - 1] != '\\' || (i > 1 && content[i - 2] != '\\')))
                        return throwOnError 
                                ? throw new UnexpectedJsonEoiException(payload[iterator], ",", iterator)
                                : false;
                
                return true;
            }
                

            if (double.TryParse(content, out _))
                return true;

            return throwOnError 
                ? throw new InvalidJsonConstraintException($"Symbol not recognized \"{content}\"") 
                : false;
        }
    }

    public static string PrettifyJson(string json, int level)
    {
        if (level < 1)
            throw new ArgumentException("Indentation can't be less than 1.", nameof(level));
        
        int indent = 0;

        return json.Aggregate(string.Empty, (current, character) => current + character switch
        {
            ':' => $"{character} ",
            '{' => $"{character}\n{" ".Repeat(indent += level)}",
            '[' => $"{character}\n{" ".Repeat(indent += level)}",
            '}' or ']' => $"\n{" ".Repeat(indent -= level)}{character}",
            ',' => $"{character}\n{" ".Repeat(indent)}",
            _ => character
        });
    }
}