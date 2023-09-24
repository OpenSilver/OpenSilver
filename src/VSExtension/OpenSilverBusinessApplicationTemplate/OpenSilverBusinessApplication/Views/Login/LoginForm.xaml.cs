using OpenRiaServices.DomainServices.Client.ApplicationServices;
using $ext_safeprojectname$.Web;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace $ext_safeprojectname$.LoginUI
{
    /// <summary>
    /// Form that presents the login fields and handles the login process.
    /// </summary>
    public partial class LoginForm : StackPanel
    {
        private LoginRegistrationWindow parentWindow;
        private LoginInfo loginInfo = new LoginInfo();
        private TextBox userNameTextBox;

        /// <summary>
        /// Creates a new <see cref="LoginForm"/> instance.
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();

            // Set the DataContext of this control to the LoginInfo instance to allow for easy binding.
            this.DataContext = this.loginInfo;
        }

        /// <summary>
        /// Sets the parent window for the current <see cref="LoginForm"/>.
        /// </summary>
        /// <param name="window">The window to use as the parent.</param>
        public void SetParentWindow(LoginRegistrationWindow window)
        {
            this.parentWindow = window;
        }

        /// <summary>
        /// Handles <see cref="DataForm.AutoGeneratingField"/> to provide the PasswordAccessor.
        /// </summary>
        private void LoginForm_AutoGeneratingField(object sender, DataFormAutoGeneratingFieldEventArgs e)
        {
            if (e.PropertyName == "UserName")
            {
                this.userNameTextBox = (TextBox)e.Field.Content;
            }
            else if (e.PropertyName == "Password")
            {
                PasswordBox passwordBox = new PasswordBox();
                e.Field.ReplaceTextBox(passwordBox, PasswordBox.PasswordProperty);
                this.loginInfo.PasswordAccessor = () => passwordBox.Password;
            }
        }

        /// <summary>
        /// Submits the <see cref="LoginOperation"/> to the server
        /// </summary>
        private void LoginButton_Click(object sender, EventArgs e)
        {
            // We need to force validation since we are not using the standard OK button from the DataForm.
            // Without ensuring the form is valid, we get an exception invoking the operation if the entity is invalid.
            if (this.loginForm.ValidateItem())
            {
                this.loginInfo.CurrentLoginOperation = WebContext.Current.Authentication.Login(this.loginInfo.ToLoginParameters(), this.LoginOperation_Completed, null);
                this.parentWindow.AddPendingOperation(this.loginInfo.CurrentLoginOperation);
            }
        }

        /// <summary>
        /// Completion handler for a <see cref="LoginOperation"/>.
        /// If operation succeeds, it closes the window.
        /// If it has an error, it displays an <see cref="ErrorWindow"/> and marks the error as handled.
        /// If it was not canceled, but login failed, it must have been because credentials were incorrect so a validation error is added to notify the user.
        /// </summary>
        private void LoginOperation_Completed(LoginOperation loginOperation)
        {
            if (loginOperation.LoginSuccess)
            {
                this.parentWindow.DialogResult = true;
            }
            else if (loginOperation.HasError)
            {
                ErrorWindow.Show(loginOperation.Error);
                loginOperation.MarkErrorAsHandled();
            }
            else if (!loginOperation.IsCanceled)
            {
                this.loginInfo.ValidationErrors.Add(new ValidationResult("Bad User Name Or Password" /*ErrorResources.ErrorBadUserNameOrPassword*/, new string[] { "UserName", "Password" }));
            }
        }

        /// <summary>
        /// Switches to the registration form.
        /// </summary>
        private void RegisterNow_Click(object sender, RoutedEventArgs e)
        {
            this.parentWindow.NavigateToRegistration();
        }

        /// <summary>
        /// If a login operation is in progress and is cancellable, cancel it.
        /// Otherwise, close the window.
        /// </summary>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (this.loginInfo.CurrentLoginOperation != null && this.loginInfo.CurrentLoginOperation.CanCancel)
            {
                this.loginInfo.CurrentLoginOperation.Cancel();
            }
            else
            {
                this.parentWindow.DialogResult = false;
            }
        }

        /// <summary>
        /// Maps Esc to the cancel button and Enter to the OK button.
        /// </summary>
        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.CancelButton_Click(sender, e);
            }
            else if (e.Key == Key.Enter && this.loginButton.IsEnabled)
            {
                this.LoginButton_Click(sender, e);
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