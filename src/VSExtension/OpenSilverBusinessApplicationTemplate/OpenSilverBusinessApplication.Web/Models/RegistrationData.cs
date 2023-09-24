using System.ComponentModel.DataAnnotations;

namespace $ext_safeprojectname$.Web
{
    /// <summary>
    /// Class containing the values and validation rules for user registration.
    /// </summary>
    public sealed partial class RegistrationData
    {
        /// <summary>
        /// Gets and sets the user name.
        /// </summary>
        [Required(ErrorMessage = "This field is required")]
        [Display(Order = 0, Name = "User name")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Invalid user name. It must contain only alphanumeric characters")]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "The user name must be at least 4 and at most 255 characters long")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets and sets the email address.
        /// </summary>
        [Required(ErrorMessage = "This field is required")]
        [Display(Order = 2, Name = "Email")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                           ErrorMessage = "Invalid email. An email must use the format user@company.com")]
        public string Email { get; set; }

        /// <summary>
        /// Gets and sets the friendly name of the user.
        /// </summary>
        [Display(Order = 1, Name = "Friendly name", Description = "How do you want your name to be displayed in the application")]
        [StringLength(255, MinimumLength = 0, ErrorMessage = "The friendly name cannot be more than 255 characters long")]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets and sets the security question.
        /// </summary>
        [Required(ErrorMessage = "This field is required")]
        [Display(Order = 5, Name = "Security question")]
        public string Question { get; set; }

        /// <summary>
        /// Gets and sets the answer to the security question.
        /// </summary>
        [Required(ErrorMessage = "This field is required")]
        [Display(Order = 6, Name = "Security answer")]
        [StringLength(128, ErrorMessage = "The security answer cannot be more than 128 characters long")]
        public string Answer { get; set; }
    }
}
