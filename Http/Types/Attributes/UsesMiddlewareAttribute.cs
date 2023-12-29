using System.Reflection;
using JetBrains.Annotations;
using MemwLib.Http.Types.Exceptions;

namespace MemwLib.Http.Types.Attributes;

/// <summary>Enables routes to use middleware.</summary>
/// <remarks>
/// The group middleware will be executed first,
/// then all of the member middleware, everything will be executed from first to last.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true), PublicAPI]
public class UsesMiddlewareAttribute : Attribute
{
    internal MiddleWareDelegate Target { get; }

    /// <summary>Constructor to set the middleware target.</summary>
    /// <remarks>Only the static methods are eligible for invocation, the order is specified by attribute declaration.</remarks>
    /// <exception cref="MiddleWareNotEligibleException">Thrown when the current method is not eligible for invocation.</exception>
    /// <remarks>
    /// The group middleware will be executed first,
    /// then all of the member middleware, everything will be executed from first to last.
    /// </remarks>
    /// <example>
    /// Please refer to the MiddlewareDelegate documentation to see the required
    /// method signature.
    /// </example>
    public UsesMiddlewareAttribute(Type type, string methodName)
    {
        MethodInfo? middleWareTarget 
            = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        if (middleWareTarget is null)
            throw new MiddleWareNotEligibleException(type, methodName);

        Target = (MiddleWareDelegate)Delegate.CreateDelegate(typeof(MiddleWareDelegate), middleWareTarget);
    }
}