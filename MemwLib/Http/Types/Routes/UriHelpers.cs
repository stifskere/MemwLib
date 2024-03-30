using System.Globalization;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Routes;

/// <summary>Class for methods related to URIS</summary>
[PublicAPI]
public static class UriHelpers
{
    /// <summary>Decodes a URI component from it's hex representation to a normal string.</summary>
    /// <param name="component">The encoded component.</param>
    /// <returns>The decoded component.</returns>
    public static string DecodeUriComponent(string component)
    {
        string decoded = string.Empty;

        for (int i = 0; i < component.Length;)
        {
            if (component[i] != '%' 
                || i + 2 >= component.Length 
                || !byte.TryParse(component.AsSpan(i + 1, 2), NumberStyles.HexNumber, null, out byte result))
            {
                decoded += component[i++];
                continue;
            }

            decoded += (char)result;
            i += 3;
        }

        return decoded;
    }

    /// <summary>Encodes a URI component from a basic string component to it's hex representation.</summary>
    /// <param name="component">The basic string component.</param>
    /// <returns>The encoded component.</returns>
    public static string EncodeUriComponent(string component)
    {
        char[] reservedChars = { '!', '*', '\'', '(', ')', ';', ':', '@', '&', '=', '+', '$', ',', '/', '?', '#', '[', ']', '%' };

        return component.Aggregate(string.Empty, (current, c) => current + (reservedChars.Contains(c) ? ((byte)c).ToString("X") : c));
    }
}