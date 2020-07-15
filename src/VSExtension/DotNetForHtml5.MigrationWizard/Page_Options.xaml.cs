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
    /// Interaction logic for Page_Options.xaml
    /// </summary>
    public partial class Page_Options : Page, IWizardPage
    {
        Context _context;

        public Page_Options(Context context)
        {
            InitializeComponent();

            _context = context;
        }

        public bool TryCreateNextPage(Context context, out Page nextPage)
        {
            if (CheckBoxToShareCSharpFiles.IsChecked == true)
            {
                _context.HowToDealWithCSharpFiles = Context.CopyOrAddAsLink.AddAsLink;
            }
            else
            {
                _context.HowToDealWithCSharpFiles = Context.CopyOrAddAsLink.Copy;
            }

            nextPage = new Page_ReadyToStart(context);
            return true;
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
