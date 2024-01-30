#if DEBUG

using System.Collections;
using JetBrains.Annotations;
using MemwLib.CoreUtils.Collections;
using MemwLib.Data.HtmlDocument.Structures;

namespace MemwLib.Data.HtmlDocument.Collections;

/// <summary>
/// This class serves as an HTML node and
/// works similarly as the JavaScript dom Node object.
/// </summary>
[PublicAPI]
public class HtmlNode : IEnumerable<HtmlElement>, ICountable
{
    private readonly List<HtmlElement> _elements = new();

    /// <inheritdoc cref="ICountable.Length"/>
    public int Length => _elements.Count;

    
    /// <inheritdoc cref="ICountable.IsEmpty"/>
    public bool IsEmpty => Length == 0;
    
    /// <summary>
    /// Adds one of more HtmlElements into this collection,
    /// all of the elements will be rendered in order of addition
    /// </summary>
    /// <param name="elements">The elements to add.</param>
    public HtmlNode Add(params HtmlElement[] elements)
    {
        foreach (HtmlElement element in elements)
            _elements.Add(element);

        return this;
    }

    /// <summary>Gets a an element of this node or sub-nodes with the specified ID</summary>
    /// <param name="id">The element ID</param>
    /// <returns>The element if an element with such ID was found, otherwise, null.</returns>
    public HtmlElement? GetElementById(string id)
    {
        foreach (HtmlElement element in _elements)
        {
            if (element.Id == id)
                return element;

            HtmlElement? subSearch = element.InnerHtml.GetElementById(id);

            if (subSearch is not null)
                return subSearch;
        }

        return null;
    }

    /// <summary>Find elements of this node or sub-nodes with the specified className</summary>
    /// <param name="className">The classname to find.</param>
    /// <exception cref="FormatException">The format of the className parameter should not have spaces.</exception>
    /// <returns>a collection of elements, empty if none found.</returns>
    public HtmlElement[] GetElementsByClassName(string className)
    {
        HashSet<HtmlElement> stack = new();

        foreach (HtmlElement element in _elements)
        {
            if (element.ClassList.Contains(className))
                stack.Add(element);
            
            foreach (HtmlElement subElement in element.InnerHtml.GetElementsByClassName(className))
                stack.Add(subElement);
        }

        return stack.ToArray();
    }

    /// <inheritdoc cref="Object.ToString"/>
    public override string ToString() 
        => _elements
            .Aggregate(string.Empty, (c, e) => $"{c}{e}")
            .TrimStart('\n');

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public IEnumerator<HtmlElement> GetEnumerator() 
        => _elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}

#endif