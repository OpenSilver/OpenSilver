using OpenRiaServices.DomainServices.Client;
using OpenRiaServices.DomainServices.Client.ApplicationServices;
using $ext_safeprojectname$.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace $ext_safeprojectname$.LoginUI
{
    /// <summary>
    /// Form that presents the <see cref="RegistrationData"/> and performs the registration process.
    /// </summary>
    public partial class RegistrationForm : StackPanel
    {
        private LoginRegistrationWindow parentWindow;
        private RegistrationData registrationData = new RegistrationData();
        private UserRegistrationContext userRegistrationContext = new UserRegistrationContext();
        private TextBox userNameTextBox;

        /// <summary>
        /// Creates a new <see cref="RegistrationForm"/> instance.
        /// </summary>
        public RegistrationForm()
        {
            InitializeComponent();

            // Set the DataContext of this control to the Registration instance to allow for easy binding.
            this.DataContext = this.registrationData;
        }

        /// <summary>
        /// Sets the parent window for the current <see cref="RegistrationForm"/>.
        /// </summary>
        /// <param name="window">The window to use as the parent.</param>
        public void SetParentWindow(LoginRegistrationWindow window)
        {
            this.parentWindow = window;
        }

        /// <summary>
        /// Wire up the Password and PasswordConfirmation accessors as the fields get generated.
        /// Also bind the Question field to a ComboBox full of security questions, and handle the LostFocus event for the UserName TextBox.
        /// </summary>
        private void RegisterForm_AutoGeneratingField(object dataForm, DataFormAutoGeneratingFieldEventArgs e)
        {
            // Put all the fields in adding mode
            e.Field.Mode = DataFieldMode.AddNew;

            if (e.PropertyName == "UserName")
            {
                this.userNameTextBox = (TextBox)e.Field.Content;
                this.userNameTextBox.LostFocus += this.UserNameLostFocus;
            }
            else if (e.PropertyName == "Password")
            {
                PasswordBox passwordBox = new PasswordBox();
                e.Field.ReplaceTextBox(passwordBox, PasswordBox.PasswordProperty);
                this.registrationData.PasswordAccessor = () => passwordBox.Password;
            }
            else if (e.PropertyName == "PasswordConfirmation")
            {
                PasswordBox passwordConfirmationBox = new PasswordBox();
                e.Field.ReplaceTextBox(passwordConfirmationBox, PasswordBox.PasswordProperty);
                this.registrationData.PasswordConfirmationAccessor = () => passwordConfirmationBox.Password;
            }
            else if (e.PropertyName == "Question")
            {
                ComboBox questionComboBox = new ComboBox();
                questionComboBox.ItemsSource = RegistrationForm.GetSecurityQuestions();
                e.Field.ReplaceTextBox(questionComboBox, ComboBox.SelectedItemProperty, binding => binding.Converter = new TargetNullValueConverter());
            }
        }

        /// <summary>
        /// The callback for when the UserName TextBox loses focus.
        /// Call into the registration data to allow logic to be processed, possibly setting the FriendlyName field.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void UserNameLostFocus(object sender, RoutedEventArgs e)
        {
            this.registrationData.UserNameEntered(((TextBox)sender).Text);
        }

        /// <summary>
        /// Returns a list of the resource strings defined in <see cref="SecurityQuestions" />.
        /// </summary>
        private static IEnumerable<string> GetSecurityQuestions()
        {
            return new string[]
            {
                "What is the name of your favorite childhood friend?",
                "What was your childhood nickname?",
                "What was the color of your first car?",
                "What was the make and model of your first car?",
                "In what city or town was your first job?",
                "Where did you vacation last year?",
                "What is your maternal grandmother's maiden name?",
                "What is your mother's maiden name?",
                "What is your pet's name?",
                "What school did you attend for sixth grade?",
                "In what year was your father born?"
            };
        }

        /// <summary>
        /// Submit the new registration.
        /// </summary>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // We need to force validation since we are not using the standard OK button from the DataForm.
            // Without ensuring the form is valid, we would get an exception invoking the operation if the entity is invalid.
            if (this.registerForm.ValidateItem())
            {
                this.registrationData.CurrentOperation = this.userRegistrationContext.CreateUser(
                    this.registrationData,
                    this.registrationData.Password,
                    this.RegistrationOperation_Completed, null);

                this.parentWindow.AddPendingOperation(this.registrationData.CurrentOperation);
            }
        }

        /// <summary>
        /// Completion handler for the registration operation.
        /// If there was an error, an <see cref="ErrorWindow"/> is displayed to the user.
        /// Otherwise, this triggers a login operation that will automatically log in the just registered user.
        /// </summary>
        private void RegistrationOperation_Completed(InvokeOperation<CreateUserStatus> operation)
        {
            if (!operation.IsCanceled)
            {
                if (operation.HasError)
                {
                    ErrorWindow.Show(operation.Error);
                    operation.MarkErrorAsHandled();
                }
                else if (operation.Value == CreateUserStatus.Success)
                {
                    this.registrationData.CurrentOperation = WebContext.Current.Authentication.Login(this.registrationData.ToLoginParameters(), this.LoginOperation_Completed, null);
                    this.parentWindow.AddPendingOperation(this.registrationData.CurrentOperation);
                }
                else if (operation.Value == CreateUserStatus.DuplicateUserName)
                {
                    this.registrationData.ValidationErrors.Add(
                        new ValidationResult("User name already exists. Please enter a different user name.",
                        new string[] { "UserName" }));
                }
                else if (operation.Value == CreateUserStatus.DuplicateEmail)
                {
                    this.registrationData.ValidationErrors.Add(
                        new ValidationResult("A user name for that e-mail address already exists. Please enter a different e-mail address.",
                        new string[] { "Email" }));
                }
                else
                {
                    ErrorWindow.Show("An unknown error has occurred. Please contact your administrator for help.");
                }
            }
        }

        /// <summary>
        /// Completion handler for the login operation that occurs after a successful registration and login attempt.
        /// This will close the window. If the operation fails, an <see cref="ErrorWindow"/> will display the error message.
        /// </summary>
        /// <param name="loginOperation">The <see cref="LoginOperation"/> that has completed.</param>
        private void LoginOperation_Completed(LoginOperation loginOperation)
        {
            if (!loginOperation.IsCanceled)
            {
                this.parentWindow.DialogResult = true;

                if (loginOperation.HasError)
                {
                    ErrorWindow.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture,
                        "Registration was successful but there was a problem while trying to log in with your credentials: {0}",
                        loginOperation.Error.Message));
                    loginOperation.MarkErrorAsHandled();
                }
                else if (loginOperation.LoginSuccess == false)
                {
                    // The operation was successful, but the actual login was not
                    ErrorWindow.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture,
                        "Registration was successful but there was a problem while trying to log in with your credentials: {0}",
                        "The user name or password is incorrect"));
                }
            }
        }

        /// <summary>
        /// Switches to the login window.
        /// </summary>
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            this.parentWindow.NavigateToLogin();
        }

        /// <summary>
        /// If a registration or login operation is in progress and is cancellable, cancel it.
        /// Otherwise, close the window.
        /// </summary>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (this.registrationData.CurrentOperation != null && this.registrationData.CurrentOperation.CanCancel)
            {
                this.registrationData.CurrentOperation.Cancel();
            }
            else
            {
                this.parentWindow.DialogResult = false;
            }
        }

        /// <summary>
        /// Maps Esc to the cancel button and Enter to the OK button.
        /// </summary>
        private void RegistrationForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.CancelButton_Click(sender, e);
            }
            else if (e.Key == Key.Enter && this.registerButton.IsEnabled)
            {
                this.RegisterButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Sets focus to the user name text box.
        /// </summary>
        public void SetInitialFocus()
        {
            this.userNameTextBox.Focus();
        }
    }
}