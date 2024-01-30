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
    /// <exception cref="UnexpectedJsonEoiException">Thrown when an unexpected end of input was found in this object while verifying.</exception>
    /// <exception cref="InvalidJsonTargetTypeException">Thrown when the type of this payload didn't match the target type.</exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when a non-nullable property wasn't found in the object or not
    /// set due to JsonObjectBehavior definitions in the target type.
    /// </exception>
    /// <returns>An instance of TResult constructed based on the payload provided.</returns>
    /// <remarks>For collections only List&lt;T&gt; and T[] are supported.</remarks>
    public static TResult? Deserialize<TResult>(string payload)
    {
        JsonTokenHandler.VerifyJson(payload, true);
        
        if (payload.StartsWith('{'))
        {
            Dictionary<string, object?> dictResult = JsonTokenHandler.HandleObject(payload);

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
            
            return (TResult?)(object)JsonTokenHandler.HandleList(payload);
        }

        return (TResult?)JsonTokenHandler.HandlePrimitive(payload);
    }

    /// <summary>Simply checks if a JSON payload is valid or invalid.</summary>
    /// <param name="payload">the JSON payload to check.</param>
    /// <returns>Whether the payload is valid or invalid.</returns>
    public static bool IsValidJson(string payload) 
        => JsonTokenHandler.VerifyJson(payload, false) 
           && JsonTokenHandler.VerifyStringKeys(payload);

    // TODO: take a look at this function, it adds x * 2 spaces more.
    /// <summary>Converts a class instance to a JSON string.</summary>
    /// <param name="payload">The class instance to convert.</param>
    /// <param name="indentation">The indentation level, if 0 it won't be indented nor new lined.</param>
    /// <exception cref="InvalidJsonConstraintException">
    /// Thrown when a serialization condition
    /// leads to breaking any JSON standard constraint.
    /// </exception>
    /// <returns>A string instance representing the TPayload instance as JSON.</returns>
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

        if (payload is ICollection payloadAsCollection)
        {
            int itemCount = 0;
            
            return payloadAsCollection.Cast<object>()
                .Aggregate("[", (current, item) => current + $"{(itemCount++ > 0 ? "," : "")}{(indentation > 0 ? "\n" : "")}{" ".Repeat(indentation)}{Serialize(item, indentation + 2)}")
                + $"{(indentation > 0 && itemCount > 0 ? "\n" : "")}{" ".Repeat(indentation)}]";
        }

        if (payload is IDictionary payloadAsDictionary)
        {
            string result = "{";

            int entryCount = 0;
            foreach (DictionaryEntry entry in payloadAsDictionary)
            {
                if (entry.Key is not string keyAsString)
                    throw new InvalidJsonConstraintException("JSON object keys must be strings.");

                result += $"{(entryCount > 0 ? "," : "")}{(indentation > 0 ? "\n" : "")}{" ".Repeat(indentation)}\"{keyAsString}\":{(indentation > 0 ? " " : "")}{Serialize(entry.Value, indentation + 2)}";
                entryCount++;
            }

            return result + $"{(indentation > 0 && entryCount > 0 ? "\n" : "")}{" ".Repeat(indentation)}}}";
        }

        {
            Type payloadType = payload.GetType();
            string result = "{";

            JsonObjectBehavior behavior =
                payloadType.GetCustomAttribute<JsonObjectAttribute>()?.Behavior
                ?? JsonObjectBehavior.AllProperties;

            int propertyCount = 0;
            foreach (PropertyInfo property in payloadType.GetProperties())
            {
                JsonPropertyAttribute? propertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                if (behavior == JsonObjectBehavior.OnlyPropertiesWithAttribute && propertyAttribute is null)
                    continue;

                result += $"{(propertyCount > 0 ? "," : "")}{(indentation > 0 ? "\n" : "")}{" ".Repeat(indentation)}\"{propertyAttribute?.Name ?? property.Name}\":{(indentation > 0 ? " " : "")}{Serialize(property.GetValue(payload), indentation + 2)}";
                propertyCount++;
            }

            return result + $"{(indentation > 0 && propertyCount > 0 ? "\n" : "")}{" ".Repeat(indentation)}}}";
        }
    }
}