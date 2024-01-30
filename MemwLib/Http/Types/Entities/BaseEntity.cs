using System.Text;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Content;
using HeaderCollection = MemwLib.Http.Types.Collections.HeaderCollection;

namespace MemwLib.Http.Types.Entities;

/// <summary>Abstract class for base HTTP entity, contains common fields between all the entity types.</summary>
public abstract class BaseEntity
{
    private string Start => BuildStart();

    /// <summary>The header collection corresponding to this HTTP entity.</summary>
    [PublicAPI]
    public HeaderCollection Headers { get; set; } = new();
    
    /// <summary>The body corresponding to this HTTP entity.</summary>
    [PublicAPI]
    public BodyConverter Body { get; set; } = BodyConverter.Empty;

    /// <summary>Protected method to build the first line of the entity.</summary>
    /// <returns>The built first line of the entity as string.</returns>
    /// <remarks>This method should not be exposed.</remarks>
    protected abstract string BuildStart();

    /// <summary>ToString override to build the entity as a String.</summary>
    /// <returns>The built entity as string.</returns>
    public override string ToString()
        => $"{Start}\r\n{(string)Headers}\r\n{Body}";

    /// <summary>Builds a Byte[] from the String version of the entity prepared for streams.</summary>
    /// <returns>The entity as a Byte[] prepared to be sent in a TCP stream.</returns>
    public byte[] ToArray() 
        => Encoding.ASCII.GetBytes(ToString());
    
    /// <summary>Runs the ToString() method of the specified instance.</summary>
    /// <param name="instance">The instance to run the method on.</param>
    /// <returns>The result of the ToString() call in the instance.</returns>
    public static explicit operator string(BaseEntity instance)
        => instance.ToString();
}