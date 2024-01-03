using JetBrains.Annotations;

namespace MemwLib.Http.Types.Attributes;

// TODO: Http versions matter!

/// <summary>Defines a group member that forms part of a route group.</summary>
[AttributeUsage(AttributeTargets.Method), UsedImplicitly]
public sealed class GroupMemberAttribute : Attribute
{
    internal string Route { get; }
    
    internal RequestMethodType RequestMethod { get; }
    
    internal bool AsRegex { get; }

    /// <summary>Constructor to define group member route.</summary>
    /// <param name="requestMethod">The HTTP method for the route.</param>
    /// <param name="route">The route, must not be an empty string.</param>
    /// <param name="asRegex">Specifies whether the route will be using as regex or not.</param>
    /// <remarks>
    /// If placed inside a class that doesn't have the
    /// RouteGroupAttribute, will define the route from root.
    /// BEWARE THE ROUTES SHOULD MATCH, IF THE GROUP MEMBER ENDS WITH / AND THIS STARTS WITH / IT WILL TRY TO MATCH //
    /// </remarks>
    public GroupMemberAttribute(RequestMethodType requestMethod, string route, bool asRegex = false)
    {
        RequestMethod = requestMethod;
        Route = route;
        AsRegex = asRegex;
    }
}