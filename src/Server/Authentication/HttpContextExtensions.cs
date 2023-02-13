using System.Security.Claims;

namespace RemindMeApp.Server.Authentication;

internal static class HttpContextExtensions
{
    internal static string GetUserId(this HttpContext context) => context.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    internal static bool UserIsAdmin(this HttpContext context) => context.User.IsInRole("admin");
}
