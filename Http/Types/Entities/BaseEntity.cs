using System.Text;
using JetBrains.Annotations;
using MemwLib.Http.Types.Collections;

namespace MemwLib.Http.Types.Entities;

public abstract class BaseEntity
{
    private string Start => BuildStart();
    [PublicAPI]
    public HeaderCollection Headers { get; set; } = new();
    [PublicAPI]
    public string Body { get; set; } = string.Empty;

    public abstract string BuildStart();

    public override string ToString()
        => $"{Start}\r\n{(string)Headers}\r\n{Body}";

    public byte[] ToArray() 
        => Encoding.ASCII.GetBytes(ToString());

    public static explicit operator string(BaseEntity instance)
        => instance.ToString();
}