using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using RemindMeApp.Server.Data;

namespace RemindMeApp.Server.Authentication;

public static class AuthExtensions
{
    internal static string GetUserId(this HttpContext context) => context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    internal static bool UserIsAdmin(this HttpContext context) => context.User.IsInRole("admin");

    internal static IApplicationBuilder UseAuthenticationAndAuthorization(this IApplicationBuilder app)
        => app.UseIdentityServer().UseAuthorization();

    internal static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services)
    {
        services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<RemindMeDbContext>();

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, RemindMeDbContext>();

        services.AddAuthentication()
            .AddIdentityServerJwt();

        // Used to send email confirmation links, reset password etc
        //services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }
}
