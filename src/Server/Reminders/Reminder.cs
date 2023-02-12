using System.ComponentModel.DataAnnotations;

namespace RemindMeApp.Server.Reminders;

public class Reminder
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = default!;
    public bool IsComplete { get; set; }

    [Required]
    public string OwnerId { get; set; } = default!;
}

// The DTO that excludes the OwnerId (we don't want that exposed to clients)
public class ReminderItem
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = default!;
    public bool IsComplete { get; set; }
}

public static class TodoMappingExtensions
{
    public static ReminderItem AsReminderItem(this Reminder reminder) =>
        new()
        {
            Id = reminder.Id,
            Title = reminder.Title,
            IsComplete = reminder.IsComplete,
        };
}
