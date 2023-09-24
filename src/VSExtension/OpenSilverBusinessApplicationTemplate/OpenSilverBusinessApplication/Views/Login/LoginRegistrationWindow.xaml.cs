using OpenRiaServices.DomainServices.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace $ext_safeprojectname$.LoginUI
{
    /// <summary>
    /// <see cref="ChildWindow"/> class that controls the registration process.
    /// </summary>
    public partial class LoginRegistrationWindow : ChildWindow
    {
        private IList<OperationBase> possiblyPendingOperations = new List<OperationBase>();

        /// <summary>
        /// Creates a new <see cref="LoginRegistrationWindow"/> instance.
        /// </summary>
        public LoginRegistrationWindow()
        {
            InitializeComponent();

            this.registrationForm.SetParentWindow(this);
            this.loginForm.SetParentWindow(this);

            NavigateToLogin();
        }

        /// <summary>
        /// Ensures the visual state and focus are correct when the window is opened.
        /// </summary>
        protected override void OnOpened()
        {
            base.OnOpened();
            this.NavigateToLogin();
        }

        /// <summary>
        /// Updates the window title according to which panel (registration / login) is currently being displayed.
        /// </summary>
        private void UpdateTitle(object sender, EventArgs eventArgs)
        {
            this.Title = (this.registrationForm.Visibility == Visibility.Visible)
                ? "Register"
                : "Login";
        }

        /// <summary>
        /// Notifies the <see cref="LoginRegistrationWindow"/> window that it can only close if <paramref name="operation"/> is finished or can be cancelled.
        /// </summary>
        /// <param name="operation">The pending operation to monitor</param>
        public void AddPendingOperation(OperationBase operation)
        {
            this.possiblyPendingOperations.Add(operation);
        }

        /// <summary>
        /// Causes the <see cref="VisualStateManager"/> to change to the "AtLogin" state.
        /// </summary>
        public virtual void NavigateToLogin()
        {
            this.loginForm.Visibility = Visibility.Visible;
            this.registrationForm.Visibility = Visibility.Collapsed;
            UpdateTitle(this, EventArgs.Empty);
        }

        /// <summary>
        /// Causes the <see cref="VisualStateManager"/> to change to the "AtRegistration" state.
        /// </summary>
        public virtual void NavigateToRegistration()
        {
            this.loginForm.Visibility = Visibility.Collapsed;
            this.registrationForm.Visibility = Visibility.Visible;
            UpdateTitle(this, EventArgs.Empty);
        }

        /// <summary>
        /// Prevents the window from closing while there are operations in progress
        /// </summary>
        private void LoginWindow_Closing(object sender, CancelEventArgs eventArgs)
        {
            foreach (OperationBase operation in this.possiblyPendingOperations)
            {
                if (!operation.IsComplete)
                {
                    if (operation.CanCancel)
                    {
                        operation.Cancel();
                    }
                    else
                    {
                        eventArgs.Cancel = true;
                    }
                }
            }
        }
    }
}