using System.Data;
using System.Text;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Content.Common;

[PublicAPI]
internal class View : IBody
{
    public string ContentType => "text/html";

    private string _parsed;
    
    public View(string parsed)
    {
        _parsed = parsed;
    }
    
    public static IBody ParseImpl(MemoryStream content) 
        => throw new ConstraintException("You cannot \"read views\".");

    public byte[] ToArray() 
        => Encoding.ASCII.GetBytes(_parsed);
}

/// <summary>This lets you render a view file within your solution.</summary>
/// <example>
/// To render a view use the name and the sub folders divided with '.' 
/// just like a laravel view
/// <para />
/// Views.Render("admin.index");
/// <para />
/// to define where to store the views add a &lt;ViewsFolder&gt; property in any
/// of your PropertyGroups in your .csproj, this will prepare the views folder
/// for compilation so you don't need to worry about moving it to the target directory.
/// <para />
/// &lt;ViewsFolder&gt;ProjectViews&lt;ViewsFolder/&gt;
/// <para />
/// This will move a folder called ProjectViews in the root of your project to the root of
/// the build directory.
/// </example>
[PublicAPI]
public static class Views
{
    private static string[] _reserved = { "memwlib" };
    
    /// <summary>Render a view as specified in the class documentation.</summary>
    /// <param name="location">The location where each subfolder is separated by '.'</param>
    /// <param name="references">The variables to be passed to the file for references</param>
    /// <exception cref="InvalidOperationException">You tried to access a view whose name contained a reserved word.</exception>
    /// <returns>A suitable value to pass as endpoint return body.</returns>
    /// <example>
    /// Views have something like preprocessor statements, you can reference
    /// variables and apply pre processor statements.
    /// <para/>
    /// To reference a variable simply do $YourVariable and it will try to
    /// find a variable in the dictionary you passed as references. If
    /// you do $$YourVariable that will be omitted by the pre processor, same happens
    /// with statements if you do @@if or @@foreach it will be omitted.
    /// <para/>
    /// There are a couple of statements you can apply such as:
    /// <para/>
    /// @if() - takes a boolean, you can make simple expressions such as $var1 > $var2.
    /// The following operators are available: ==, !=, &lt;, &gt;, &lt;=, &gt;=. And it
    /// must proceed with a @endif or @else statement. Boolean variables are
    /// also admitted as a valid expression, no TRUE or FALSE literals are available. 
    /// <para/>
    /// @foreach($item in $iterator) - foreach must reference a variable, you cannot
    /// create array literals, and string literals are not iterable, inside the statement 
    /// you can reference $item or whatever you named your item variable. This statement
    /// must proceed with a @endforeach statement
    /// </example>
    /// <remarks>
    /// There are a couple of prefixes you cannot use, as these are used by the library
    /// and will throw if you try using them.
    /// </remarks>
    public static IBody Render(string location, Dictionary<string, object?>? references = null)
    {
        string locationLower = location.ToLower();
        if (_reserved.Any(p => locationLower.Contains(p.ToLower())))
            throw new InvalidOperationException("You tried to access a view whose name contained a reserved word.");
                
        references ??= new Dictionary<string, object?>();
        
        string content = File.ReadAllText($"./views/{location.Replace('.', '/')}.view.html");

        foreach ((string key, object? value) in references)
        {
            
        }
        
        // \@(?:if|elif)\((?<condition>[^\)]*)\)(?<content>(?=(@elif|@endif))|.)+
        
        return new View(content);
    }
}