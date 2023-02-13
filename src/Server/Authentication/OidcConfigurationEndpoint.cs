using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;

namespace RemindMeApp.Server.Authentication;

public static class OidcConfigurationEndpoint
{
    public static RouteGroupBuilder MapOpenIdConnectEndpoint(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/_configuration");

        group.WithTags("OpenIdConnect");

        group.MapGet("{clientId}", ([FromRoute] string clientId,
            IClientRequestParametersProvider clientRequestParametersProvider, HttpContext context) =>
        {
            var parameters = clientRequestParametersProvider.GetClientParameters(context, clientId);
            Results.Ok(parameters);
        });

        return group;
    }
}
