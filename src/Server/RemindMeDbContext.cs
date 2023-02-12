using Microsoft.EntityFrameworkCore;
using RemindMeApp.Server.Reminders;

namespace RemindMeApp.Server;

public class RemindMeDbContext : DbContext
{
    public RemindMeDbContext(DbContextOptions<RemindMeDbContext> options) : base(options) { }

    public DbSet<Reminder> Reminders => Set<Reminder>();
}
