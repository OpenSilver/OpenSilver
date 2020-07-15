using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DotNetForHtml5.MigrationWizard
{
    /// <summary>
    /// Interaction logic for MigrationWizardWindow.xaml
    /// </summary>
    public partial class MigrationWizardWindow : Window
    {
        Context _context;

        public MigrationWizardWindow(EnvDTE.Solution solution)
        {
            InitializeComponent();

            _context = new Context(solution: solution);

            _context.MigrationCompleted += Context_MigrationCompleted;
            _context.MigrationStarted += Context_MigrationStarted;
            this.Loaded += MigrationWizardWindow_Loaded;
            MainContainer.Navigated += MainContainer_Navigated;
        }

        void MainContainer_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            UpdateButtonsVisibility();
        }

        void Context_MigrationCompleted(object sender, EventArgs e)
        {
            // Replace all the buttons with just a "Close" button:
            ButtonsContainer.Visibility = Visibility.Visible;
            ButtonNext.Visibility = Visibility.Collapsed;
            ButtonBack.Visibility = Visibility.Collapsed;
            ButtonCancel.Content = "Close";
        }

        void Context_MigrationStarted(object sender, EventArgs e)
        {
            ButtonsContainer.Visibility = Visibility.Collapsed;
        }

        void MigrationWizardWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Activate();
            this.Focus();
            this.Topmost = true;
            this.Topmost = false;

            MainContainer.Navigate(new Page_Welcome());
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainContainer.NavigationService.CanGoBack)
            {
                // We go back only if the content is not an IWizardPage OR if it is an IWizardPage and the "CanGoBack" property is True:
                if (!(MainContainer.Content is IWizardPage)
                    || ((IWizardPage)MainContainer.Content).CanGoBack)
                {
                    MainContainer.NavigationService.GoBack();
                }
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (MainContainer.Content is IWizardPage)
            {
                Page nextPage;
                if (((IWizardPage)MainContainer.Content).TryCreateNextPage(_context, out nextPage))
                {
                    MainContainer.Navigate(nextPage);
                }
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // Close the window:
            this.Close();
        }

        void UpdateButtonsVisibility()
        {
            ButtonBack.Visibility = (MainContainer.NavigationService.CanGoBack ? Visibility.Visible : Visibility.Collapsed);
        }
    }
}
