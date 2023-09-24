using OpenRiaServices.DomainServices.Client;
using OpenRiaServices.DomainServices.Client.ApplicationServices;
using System;
using System.ComponentModel.DataAnnotations;

namespace $ext_safeprojectname$.Web
{
    /// <summary>
    /// Extensions to provide client side custom validation and data binding to <see cref="RegistrationData"/>.
    /// </summary>
    public partial class RegistrationData
    {
        private OperationBase currentOperation;

        /// <summary>
        /// Gets or sets a function that returns the password.
        /// </summary>
        internal Func<string> PasswordAccessor { get; set; }

        /// <summary>
        /// Gets and sets the password.
        /// </summary>
        [Required(ErrorMessage = "This field is required")]
        [Display(Order = 3, Name = "Password", Description = "The password must be 7 characters long and must contain at least one special character e.g. @ or #")]
        [RegularExpression("^.*[^a-zA-Z0-9].*$", ErrorMessage = "A password needs to contain at least one special character e.g. @ or #")]
        [StringLength(50, MinimumLength = 7, ErrorMessage = "Password must be at least 7 and at most 50 characters long")]
        public string Password
        {
            get
            {
                return (this.PasswordAccessor == null) ? string.Empty : this.PasswordAccessor();
            }
            set
            {
                this.ValidateProperty("Password", value);
                this.CheckPasswordConfirmation();

                // Do not store the password in a private field as it should not be stored in memory in plain-text.
                // Instead, the supplied PasswordAccessor serves as the backing store for the value.
                this.RaisePropertyChanged("Password");
            }
        }

        /// <summary>
        /// Gets or sets a function that returns the password confirmation.
        /// </summary>
        internal Func<string> PasswordConfirmationAccessor { get; set; }

        /// <summary>
        /// Gets and sets the password confirmation string.
        /// </summary>
        [Required(ErrorMessage = "This field is required")]
        [Display(Order = 4, Name = "Confirm password")]
        public string PasswordConfirmation
        {
            get
            {
                return (this.PasswordConfirmationAccessor == null) ? string.Empty : this.PasswordConfirmationAccessor();
            }
            set
            {
                this.ValidateProperty("PasswordConfirmation", value);
                this.CheckPasswordConfirmation();

                // Do not store the password in a private field as it should not be stored in memory in plain-text.
                // Instead, the supplied PasswordAccessor serves as the backing store for the value.
                this.RaisePropertyChanged("PasswordConfirmation");
            }
        }

        /// <summary>
        /// Gets or sets the current registration or login operation.
        /// </summary>
        internal OperationBase CurrentOperation
        {
            get
            {
                return this.currentOperation;
            }
            set
            {
                if (this.currentOperation != value)
                {
                    if (this.currentOperation != null)
                    {
                        this.currentOperation.Completed -= (s, e) => this.CurrentOperationChanged();
                    }

                    this.currentOperation = value;

                    if (this.currentOperation != null)
                    {
                        this.currentOperation.Completed += (s, e) => this.CurrentOperationChanged();
                    }

                    this.CurrentOperationChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is presently being registered or logged in.
        /// </summary>
        [Display(AutoGenerateField = false)]
        public bool IsRegistering
        {
            get
            {
                return this.CurrentOperation != null && !this.CurrentOperation.IsComplete;
            }
        }

        /// <summary>
        /// Helper method for when the current operation changes.
        /// Used to raise appropriate property change notifications.
        /// </summary>
        private void CurrentOperationChanged()
        {
            this.RaisePropertyChanged("IsRegistering");
        }

        /// <summary>
        /// Checks to ensure the password and confirmation match.
        /// If they don't match, a validation error is added.
        /// </summary>
        private void CheckPasswordConfirmation()
        {
            // If either of the passwords has not yet been entered, then don't test for equality between the fields.
            // The Required attribute will ensure a value has been entered for both fields.
            if (string.IsNullOrWhiteSpace(this.Password)
                || string.IsNullOrWhiteSpace(this.PasswordConfirmation))
            {
                return;
            }

            // If the values are different, then add a validation error with both members specified
            if (this.Password != this.PasswordConfirmation)
            {
                this.ValidationErrors.Add(
                    new ValidationResult("Passwords do not match",
                    new string[] { "PasswordConfirmation", "Password" }));
            }
        }

        /// <summary>
        /// Perform logic after the UserName value has been entered.
        /// </summary>
        /// <param name="userName">The user name value that was entered.</param>
        /// <remarks>
        /// Allow the form to indicate when the value has been completely entered.
        /// Using the OnUserNameChanged method can lead to a premature call before the user has finished entering the value in the form.
        /// </remarks>
        internal void UserNameEntered(string userName)
        {
            // Auto-Fill FriendlyName to match UserName for new entities when there is not a friendly name specified
            if (string.IsNullOrWhiteSpace(this.FriendlyName))
            {
                this.FriendlyName = userName;
            }
        }

        /// <summary>
        /// Creates a new <see cref="LoginParameters"/> initialized with this entity's data (IsPersistent will default to false).
        /// </summary>
        public LoginParameters ToLoginParameters()
        {
            return new LoginParameters(this.UserName, this.Password, false, null);
        }
    }
}