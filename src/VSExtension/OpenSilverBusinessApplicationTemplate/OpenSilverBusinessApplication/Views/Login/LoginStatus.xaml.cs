using OpenRiaServices.DomainServices.Client.ApplicationServices;
using $ext_safeprojectname$.Web;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace $ext_safeprojectname$.LoginUI
{
    /// <summary>
    /// <see cref="UserControl"/> class that shows the current login status and allows login and logout.
    /// </summary>
    public partial class LoginStatus : UserControl
    {
        /// <summary>
        /// Creates a new <see cref="LoginStatus"/> instance.
        /// </summary>
        public LoginStatus()
        {
            this.InitializeComponent();

            if (DesignerProperties.IsInDesignTool)
            {
                VisualStateManager.GoToState(this, "loggedOut", false);
            }
            else
            {
                this.DataContext = WebContext.Current;
                WebContext.Current.Authentication.LoggedIn += this.Authentication_LoggedIn;
                WebContext.Current.Authentication.LoggedOut += this.Authentication_LoggedOut;
                this.UpdateLoginState();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginRegistrationWindow loginWindow = new LoginRegistrationWindow();
            loginWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            WebContext.Current.Authentication.Logout(logoutOperation =>
            {
                if (logoutOperation.HasError)
                {
                    ErrorWindow.Show(logoutOperation.Error);
                    logoutOperation.MarkErrorAsHandled();
                }
            }, /* userState */ null);
        }

        private void Authentication_LoggedIn(object sender, AuthenticationEventArgs e)
        {
            this.UpdateLoginState();
        }

        private void Authentication_LoggedOut(object sender, AuthenticationEventArgs e)
        {
            this.UpdateLoginState();
        }

        private void UpdateLoginState()
        {
            if (WebContext.Current.User.IsAuthenticated)
            {
                this.welcomeText.Text = string.Format(
                    CultureInfo.CurrentUICulture,
                    "Welcome {0}",
                    WebContext.Current.User.DisplayName);
            }
            else
            {
                this.welcomeText.Text = "authenticating...";
            }

            if (WebContext.Current.Authentication is WindowsAuthentication)
            {
                VisualStateManager.GoToState(this, "windowsAuth", true);
            }
            else
            {
                VisualStateManager.GoToState(this, (WebContext.Current.User.IsAuthenticated) ? "loggedIn" : "loggedOut", true);
            }
        }
    }
}