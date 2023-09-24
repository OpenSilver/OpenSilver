using System;
using System.Windows;
using System.Windows.Controls;

namespace $ext_safeprojectname$
{
    public partial class ErrorWindow : ChildWindow
    {
        public static void Show(Exception ex)
        {
            var errorWindow = new ErrorWindow(ex);
            errorWindow.Show();
        }

        public static void Show(Uri uri)
        {
            var errorWindow = new ErrorWindow(uri);
            errorWindow.Show();
        }

        public static void Show(string message, string details = "")
        {
            var errorWindow = new ErrorWindow(message, details);
            errorWindow.Show();
        }

        private ErrorWindow(Exception e)
        {
            InitializeComponent();
            if (e != null)
            {
                ErrorTextBox.Text = e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
            }
        }

        private ErrorWindow(Uri uri)
        {
            InitializeComponent();
            if (uri != null)
            {
                ErrorTextBox.Text = "Page not found: \"" + uri.ToString() + "\"";
            }
        }

        private ErrorWindow(string message, string details)
        {
            InitializeComponent();
            ErrorTextBox.Text = message + Environment.NewLine + Environment.NewLine + details;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}