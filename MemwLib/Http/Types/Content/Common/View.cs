using System.Reflection;
using JetBrains.Annotations;

namespace MemwLib.Http.Types.Content.Common;

[PublicAPI]
internal class View : IBody
{
    public string ContentType => "text/html";
    
    public static IBody ParseImpl(MemoryStream content)
    {
        throw new NotImplementedException();
    }
    
    public byte[] ToArray()
    {
        throw new NotImplementedException();
    }
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
    /// <summary>Render a view as specified in the class documentation.</summary>
    /// <param name="location">The location where each subfolder is separated by '.'</param>
    /// <param name="references">The variables to be passed to the file for references</param>
    /// <exception cref="MissingFieldException">The views folder is not configured/found in your csproj.</exception>
    /// <returns>A suitable value to pass as endpoint return body.</returns>
    /// <remarks>
    /// Views have something like preprocessor statements, you can reference
    /// variables and apply pre processor statements.
    /// <para/>
    /// To reference a variable simply do ${YourVariable} and it will try to
    /// find a variable in the dictionary you passed as references. If
    /// you do $${} that will be omitted by the pre processor, same happens
    /// with statements if you do @@if or @@foreach it will be omitted.
    /// <para/>
    /// There are a couple of statements you can apply such as:
    /// <para/>
    /// @if() - takes a boolean, you can make simple expressions such as $var1 > $var2.
    /// The following operators are available: ==, !=, &lt;, &gt;, &lt;=, &gt;=. And it
    /// must proceed with a @endif or @else or @elif() statement. Boolean variables are
    /// also admitted as a valid expression, no TRUE or FALSE literals are available. 
    /// <para/>
    /// @foreach(item in iterator) - foreach must reference a variable, you cannot
    /// create array literals, and string literals are not iterable, inside the statement 
    /// you can reference ${item} or whatever you named your item variable. This statement
    /// must proceed with a @endforeach statement
    /// </remarks>
    public static IBody Render(string location, Dictionary<string, object?>? references = null)
    {
#if !MEMWLIB_VIEWS_DO_EXIST
        throw new MissingFieldException("The views folder is not configured, please add the " +
                                            "<ViewsFolder> property in any of your csproj PropertyGroups");
#else
        // process views in here.
#endif
    }
}