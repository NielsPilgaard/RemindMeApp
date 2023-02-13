using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RemindMeApp.Server.Reminders;

namespace RemindMeApp.Server.Data;

public class RemindMeDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public DbSet<Reminder> Reminders => Set<Reminder>();

    public RemindMeDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }
}
