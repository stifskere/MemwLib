using JetBrains.Annotations;

namespace MemwLib.Http.Types.Attributes;

/// <summary>Defines a route group of static members for HTTP routes.</summary>
[AttributeUsage(AttributeTargets.Class), UsedImplicitly]
public class RouteGroupAttribute : Attribute
{
    internal string? Route { get; }
    
    internal bool AsRegex { get; }

    /// <summary>Constructor to define the route group's prefix.</summary>
    /// <param name="route">The prefix, must not be an empty string.</param>
    /// <param name="asRegex">Specifies whether the route will be using as regex or not.</param>
    /// <example>
    /// The first part of the route group as in /users and
    /// the members would need to be accessed within /users/member
    /// </example>
    /// <remarks>BEWARE THE ROUTES SHOULD MATCH, IF THE GROUP MEMBER ENDS WITH / AND THIS STARTS WITH / IT WILL TRY TO MATCH //</remarks>
    public RouteGroupAttribute(string route, bool asRegex = false)
    {
        Route = route;
        AsRegex = asRegex;
    }
}