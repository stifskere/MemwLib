using System.Collections;
using System.Globalization;
using System.Reflection;
using JetBrains.Annotations;
using MemwLib.CoreUtils;
using MemwLib.Data.Json.Attributes;
using MemwLib.Data.Json.Enums;
using MemwLib.Data.Json.Exceptions;

namespace MemwLib.Data.Json;

/// <summary>A class that defines utilities for working with the JSON data type.</summary>
[PublicAPI]
public static class JsonParser
{
    /// <summary>
    /// Verifies an converts a JSON string into an instance
    /// of TResult? if it could be converted.
    /// </summary>
    /// <param name="payload">The JSON payload to convert.</param>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <exception cref="InvalidJsonConstraintException">Thrown when a constraint for the JSON standard was broken.</exception>
    /// <exception cref="InvalidJsonSequenceException">Thrown when a sequence of a JSON fragment didn't match enclosing rules.</exception>
    /// <exception cref="InvalidJsonTargetTypeException">Thrown when the type of this payload didn't match the target type.</exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when a non-nullable property wasn't found in the object or not
    /// set due to JsonObjectBehavior definitions in the target type.
    /// </exception>
    /// <returns>An instance of TResult constructed based on the payload provided.</returns>
    /// <remarks>For collections only List&lt;T&gt; and T[] are supported.</remarks>
    public static TResult? Deserialize<TResult>(string payload)
    {
        payload = payload.Trim(' ', '\n');
        
        JsonTokenHandler.Validators.VerifyJson(payload, true);
        
        if (payload.StartsWith('{'))
        {
            Dictionary<string, object?> dictResult = JsonTokenHandler.Assigns.HandleObject(payload);

            if (typeof(IDictionary).IsAssignableFrom(typeof(TResult)))
                return (TResult?)(object)dictResult;

            return (TResult)TypeUtils.FillType(typeof(TResult), dictResult,
                type => type.GetCustomAttribute<JsonObjectAttribute>()?.Behavior ?? JsonObjectBehavior.AllProperties,
                (property, typeCondition) =>
                {
                    JsonPropertyAttribute? propertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                    return (JsonObjectBehavior)typeCondition! == JsonObjectBehavior.OnlyPropertiesWithAttribute && propertyAttribute is null
                        ? null
                        : propertyAttribute?.Name ?? property.Name;
                }
            );
        }

        if (payload.StartsWith('['))
        {
            if (!typeof(ICollection).IsAssignableFrom(typeof(TResult)))
                throw new InvalidJsonTargetTypeException(typeof(TResult).Name);
            
            return (TResult?)(object)JsonTokenHandler.Assigns.HandleList(payload);
        }

        return (TResult?)JsonTokenHandler.Assigns.HandlePrimitive(payload);
    }

    /// <summary>Simply checks if a JSON payload is valid or invalid.</summary>
    /// <param name="payload">the JSON payload to check.</param>
    /// <returns>Whether the payload is valid or invalid.</returns>
    /// <remarks>
    /// This is ONLY designed to check whether the JSON is valid or not,
    /// it will not check type matching, to validate type matching, you can
    /// use JsonParser.Deserialize and catch a InvalidJsonTargetTypeException.
    /// </remarks>
    public static bool IsValidJson(string payload)
        => JsonTokenHandler.Validators.VerifyJson(payload, false);

    /// <summary>Converts an object to a string JSON object as JavaScript's JSON.stringify does.</summary>
    /// <param name="payload">The Object to convert.</param>
    /// <param name="indentation">The indentation depth.</param>
    /// <exception cref="InvalidJsonConstraintException">Supplied dictionary keys aren't strings.</exception>
    /// <returns>a string representing your object in JSON.</returns>
    public static string Serialize(object? payload, int indentation = 0)
    {
        if (payload is null)
            return "null";

        if (payload is string payloadAsString)
            return $"\"{payloadAsString}\"";
        
        if (payload is double payloadAsDouble)
            return payloadAsDouble.ToString(CultureInfo.InvariantCulture);

        if (payload is float payloadAsFloat)
            return payloadAsFloat.ToString(CultureInfo.InvariantCulture);

        if (payload is int payloadAsInt)
            return payloadAsInt.ToString(CultureInfo.InvariantCulture);
        
        if (payload is bool payloadAsBool)
            return payloadAsBool.ToString().ToLower();

        string result;
        
        if (payload is IDictionary payloadAsDictionary)
        {
            result = payloadAsDictionary
                .Cast<KeyValuePair<object, object>>()
                .Aggregate("{", (last, next) => 
                    next.Key is not string ? throw new InvalidJsonConstraintException("Keys for json objects must be strings.")
                    : $"{last}\"{next.Key}\":{Serialize(next.Value)},"
                )[..^1] + '}';
        }

        else if (payload is ICollection payloadAsCollection)
        {
            result =  payloadAsCollection
                .Cast<object>()
                .Aggregate("[", (last, next) => $"{last}{Serialize(next)},")[..^1] + ']';
        }
        
        else {
            Type payloadType = payload.GetType();

            JsonObjectBehavior behavior =
                payloadType.GetCustomAttribute<JsonObjectAttribute>()?.Behavior
                ?? JsonObjectBehavior.AllProperties;

            result = "{";
            
            foreach (PropertyInfo property in payloadType.GetProperties())
            {
                JsonPropertyAttribute? propertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                if (behavior == JsonObjectBehavior.OnlyPropertiesWithAttribute && propertyAttribute is null)
                    continue;

                result += $"\"{propertyAttribute?.Name ?? property.Name}\":{Serialize(property.GetValue(payload))},";
            }

            result = result[..^1] + '}';
        }

        return indentation > 0 
            ? JsonTokenHandler.PrettifyJson(result, indentation) 
            : result;
    }
}