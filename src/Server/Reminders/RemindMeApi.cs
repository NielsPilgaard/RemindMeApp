using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace RemindMeApp.Server.Reminders;

internal static class RemindMeApi
{
    public static RouteGroupBuilder MapReminders(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/reminders");

        group.WithTags("Reminders");

        group.MapGet("/", async (RemindMeDbContext db, CurrentUser owner) =>
            await db.Reminders.Where(reminder => reminder.OwnerId == owner.Id).Select(t => t.AsReminderItem()).AsNoTracking().ToListAsync());

        group.MapGet("/{id}", async Task<Results<Ok<ReminderItem>, NotFound>> (RemindMeDbContext db, int id, CurrentUser owner) =>
        {
            return await db.Reminders.FindAsync(id) switch
            {
                { } reminder when reminder.OwnerId == owner.Id || owner.IsAdmin => TypedResults.Ok(reminder.AsReminderItem()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<ReminderItem>> (RemindMeDbContext db, ReminderItem newReminder, CurrentUser owner) =>
        {
            var reminder = new Reminder
            {
                Title = newReminder.Title,
                OwnerId = owner.Id
            };

            db.Reminders.Add(reminder);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/todos/{reminder.Id}", reminder.AsReminderItem());
        });

        group.MapPut("/{id:int}", async Task<Results<Ok, NotFound, BadRequest>> (RemindMeDbContext db, int id, ReminderItem reminderItem, CurrentUser owner) =>
        {
            if (id != reminderItem.Id)
            {
                return TypedResults.BadRequest();
            }

            int rowsAffected = await db.Reminders.Where(reminder => reminder.Id == id && (reminder.OwnerId == owner.Id || owner.IsAdmin))
                                             .ExecuteUpdateAsync(updates =>
                                                updates.SetProperty(t => t.IsComplete, reminderItem.IsComplete)
                                                       .SetProperty(t => t.Title, reminderItem.Title));

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (RemindMeDbContext db, int id, CurrentUser owner) =>
        {
            int rowsAffected = await db.Reminders.Where(reminder => reminder.Id == id && (reminder.OwnerId == owner.Id || owner.IsAdmin))
                                             .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }
}
