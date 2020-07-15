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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DotNetForHtml5.MigrationWizard
{
    /// <summary>
    /// Interaction logic for Page_Welcome.xaml
    /// </summary>
    public partial class Page_Welcome : Page, IWizardPage
    {
        public Page_Welcome()
        {
            InitializeComponent();
        }

        public bool TryCreateNextPage(Context context, out Page nextPage)
        {
            // Check if we are running inside VS:
            if (context.Solution != null)
            {
                // Verify that a solution has been open:
                if (context.Solution.IsOpen)
                {
                    // Go to the next page:
                    nextPage = new Page_ChooseTheProjects(context);
                    return true;
                }
                else
                {
                    MessageBox.Show("No solution is currently open. Please open the Visual Studio solution that contains the project(s) to migrate, and try again.");
                    nextPage = null;
                    return false;
                }
            }
            else
            {
                // We still continue, to enable test/debug outside VS:
                nextPage = new Page_ChooseTheProjects(context);
                return true;
            }
        }

        public bool CanGoBack
        {
            get
            {
                return true;
            }
        }
    }
}
