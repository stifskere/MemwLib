using JetBrains.Annotations;

namespace MemwLib.CoreUtils.Collections;

/// <summary>Interface that promises the collection will be able to be counted.</summary>
[PublicAPI]
public interface ICountable
{
    /// <summary>Gets the length of this collection.</summary>
    public int Length { get; }

    /// <summary>Whether this collection is empty or not.</summary>
    public bool IsEmpty { get; }
}