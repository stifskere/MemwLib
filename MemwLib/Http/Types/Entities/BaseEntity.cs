using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;
using MemwLib.Http.Types.Content;
using MemwLib.Http.Types.Exceptions;

namespace MemwLib.Http.Types.Entities;

/// <summary>Abstract class for base HTTP entity, contains common fields between all the entity types.</summary>
public abstract partial class BaseEntity
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

    /// <summary>
    /// This method can be called from a inheriting constructor to
    /// initialize the common properties of an entity, getting the
    /// top part while avoiding the use of abstract methods.
    /// </summary>
    /// <param name="reader">Where to read the entity from.</param>
    /// <returns>The top part of the entity as it's what changes.</returns>
    /// <exception cref="TimeoutException">It took too long for the initializer to receive data.</exception>
    /// <exception cref="ParseException{T}">This entity was not correctly formatted or there was an error while parsing.</exception>
    protected string InitEntity(StreamReader reader)
    {
        CancellationTokenSource timeoutToken = new();
        Task timeout = Task.Delay(TimeSpan.FromSeconds(12), timeoutToken.Token);
        
        string? top;
        while (string.IsNullOrEmpty(top = reader.ReadLine()))
        {
            if (timeout.IsCompleted)
                throw new TimeoutException("Reading entity operation timed out.");
        }
        
        timeoutToken.Cancel();
        
        if (string.IsNullOrEmpty(top))
            throw new UnreachableException("Top should never be null at this point.");
        
        string? target;
        
        Headers = new HeaderCollection();
        
        while (!string.IsNullOrEmpty(target = reader.ReadLine()))
        {
            string[] splitTarget = target.Split(": ");

            if (splitTarget.Length != 2)
                throw new ParseException<RequestEntity>();

            Headers[splitTarget[0]] = splitTarget[1];
        }
        
        if (!Headers.Contains("Content-Length"))
        {
            Body = BodyConverter.Empty;
            return top;
        }

        if (!int.TryParse(Headers["Content-Length"], out int bodyLength))
            throw new ParseException<RequestEntity>();

        byte[] body = new byte[bodyLength];

        int index = 0;
        while (bodyLength-- > 0)
        {
            int read = reader.Read();

            if (read == -1)
            {
                Headers["Content-Length"] = index.ToString();
                break;
            }

            body[index++] = (byte)read;
        }

        Body = new BodyConverter(body);

        return top;
    }
    
    [GeneratedRegex(@"HTTP\/\d+\.\d+", RegexOptions.IgnoreCase, "en-US")]
    protected static partial Regex HttpVersionRegex();
}