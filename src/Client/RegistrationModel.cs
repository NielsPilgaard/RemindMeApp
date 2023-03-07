using System.ComponentModel.DataAnnotations;

namespace RemindMeApp.Client;

public class RegistrationModel
{
    [Required]
    [StringLength(16, ErrorMessage = "Name length can't be more than 16.")]
    public string? Username { get; set; }

    [Required]
    [StringLength(32, MinimumLength = 6, ErrorMessage = "The password must be between 6 and 32 characters long.")]
    [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).*$",
        MatchTimeoutInMilliseconds = 1000,
        ErrorMessage = "The password must contain a lower-case letter, an upper-case letter and a digit.")]
    public string? Password { get; set; }

    [Required]
    [Compare(nameof(Password))]
    public string? RepeatPassword { get; set; }
}
