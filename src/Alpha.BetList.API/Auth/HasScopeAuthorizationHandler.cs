using Microsoft.AspNetCore.Authorization;

namespace Alpha.BetList.API.Auth;

public class HasScopeAuthorizationHandler : AuthorizationHandler<HasScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == "scope"))
            return Task.CompletedTask;

        // Split the scopes string into an array
        var scopes = context.User.FindFirst(c => c.Type == "scope")?.Value.Split(' ');

        // Succeed if the scope array contains the required scope
        if (scopes != null && scopes.Any(s => s == requirement.Scope))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}