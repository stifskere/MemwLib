#if DEBUG

using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Data.DomParser.Collections;
using MemwLib.Data.DomParser.Exceptions;

namespace MemwLib.Data.DomParser.Structures;

/// <summary>
/// This class represents an HTML element which acts similarly
/// as the JavaScript HTMLElement except properties are not typed
/// for sake of simplicity.
/// </summary>
[PublicAPI]
public partial class HtmlElement
{
    private string _tagName = default!;
    
    /// <summary>The element name.</summary>
    /// <exception cref="InvalidElementException">Thrown when an invalid name is given to this property.</exception>
    /// <example>The value means the following: &lt;{value}&gt;&lt;/{value}&gt;</example>
    public string TagName
    {
        get => _tagName;
        set
        {
            if (!ValidTagRegex().IsMatch(value))
                throw new InvalidElementException("Invalid value given to HtmlElement TagName setter.");

            _tagName = value;
        } 
    }
    
    /// <summary>Contains the attributes of this element</summary>
    /// <example>The value means the following &lt;name {key}="{value}"/&gt;</example>
    public Dictionary<string, string> Attributes { get; } = new();

    /// <summary>Property to set the inner elements of the tag</summary>
    /// <example>The value means the following: &lt;tag&gt;{value}&lt;/tag&gt;</example>
    public HtmlNode InnerHtml { get; set; } = new();

    /// <summary>Tries to find an ID attribute in the tag and returns its value, otherwise an empty string.</summary>
    public string Id => Attributes.TryGetValue("id", out string? id) ? id : string.Empty;

    /// <summary>The list of classes from the class attribute, if the attribute is not found or empty an empty string[].</summary>
    public string[] ClassList => Attributes.TryGetValue("class", out string? className) ? className.Split(' ') : Array.Empty<string>();
    
    /// <summary>Initializes an element with the name and possible attributes.</summary>
    /// <param name="tagName">The name of the tag.</param>
    /// <param name="attributes">The attributes this element must have.</param>
    public HtmlElement(string tagName, params (string, string)[] attributes)
    {
        TagName = tagName;

        foreach ((string key, string value) in attributes)
            Attributes.Add(key, value);
    }
    
    /// <inheritdoc cref="Object.ToString"/>
    public override string ToString() 
        => $"<{TagName}" +
           $"{(Attributes.Count != 0
               ? " " + Attributes.Aggregate(string.Empty, (result, pair) => result + $"{pair.Key}=\"{pair.Value}\" ") 
               : string.Empty)}" +
           $"{(InnerHtml.IsEmpty ? "/>\n" : $">\n{InnerHtml}</{TagName}>\n")}";
    

    [GeneratedRegex(@"^[a-z\-]+$")]
    internal static partial Regex ValidTagRegex();
}

#endif