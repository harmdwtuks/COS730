using System.ComponentModel.DataAnnotations;

namespace InteractionLayer.Models
{
    public class ResetPassword
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DataType(DataType.Password)]
        [RegularExpression(pattern: @"^(?=.*[A-z])(?=.*\d)(?=.*(_|[^\w])).+$", ErrorMessage = "New password must contain at least one number and one special character.")]
        public string Password { get; set; }

        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string Key { get; set; }
    }
}