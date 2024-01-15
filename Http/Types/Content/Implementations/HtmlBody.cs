using JetBrains.Annotations;

namespace MemwLib.Http.Types.Content.Implementations;

#if DEBUG

/// <summary>Body implementation for handling a request body as HTML.</summary>
[PublicAPI]
public class HtmlBody : IBody
{
    /// <inheritdoc cref="IBody.ContentType"/>
    public string ContentType => "text/html";
    
    /// <inheritdoc cref="IBody.ParseImpl"/>
    public static IBody ParseImpl(string content)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IBody.ToRaw"/>
    public string ToRaw()
    {
        throw new NotImplementedException();
    }

    private static void BodyParser()
    {
        
    }
}

#endif