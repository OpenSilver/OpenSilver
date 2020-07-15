using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Page_ChooseDestinationNames.xaml
    /// </summary>
    public partial class Page_ChooseDestinationNames : Page, IWizardPage
    {
        Context _context;

        public Page_ChooseDestinationNames(Context context)
        {
            InitializeComponent();

            _context = context;

            if (_context.Solution != null)
            {
                ProjectsDataGrid.ItemsSource = _context.ProjectsToMigrate;
            }
            else
            {
                // For testing purposes:
                _context.ProjectsToMigrate = new List<Context.ProjectModel> {
                    new Context.ProjectModel() { Name = "Test1", DestinationName = "Test1.Cshtml5", ProjectType = Context.ProjectType.Silverlight, DestinationProjectType = "CSHTML5", ProjectOutputType = Context.ProjectOutputType.Application },
                    new Context.ProjectModel() { Name = "Test2", DestinationName = "Test2.Cshtml5", ProjectType = Context.ProjectType.Silverlight, DestinationProjectType = "CSHTML5", ProjectOutputType = Context.ProjectOutputType.Library } };
                _context.AllProjects = _context.ProjectsToMigrate;
                ProjectsDataGrid.ItemsSource = _context.ProjectsToMigrate;
            }
        }

        public bool TryCreateNextPage(Context context, out Page nextPage)
        {
            // Verify that the names are ok:
            if (ValidateNames())
            {
                nextPage = new Page_Options(context);
                return true;
            }
            else
            {
                nextPage = null;
                return false;
            }
        }

        bool ValidateNames()
        {
            if (_context.Solution != null)
            {
                string solutionFileFullName = ((EnvDTE80.Solution2)_context.Solution).FileName;
                string solutionFolderFullName = System.IO.Path.GetDirectoryName(solutionFileFullName);

                // Verify that the names are ok:
                for (int i = 0; i < _context.ProjectsToMigrate.Count; i++)
                {
                    string newProjectName = _context.ProjectsToMigrate[i].DestinationName;

                    // Verify that the name is valid:
                    if (string.IsNullOrWhiteSpace(newProjectName))
                    {
                        MessageBox.Show("A project name cannot be empty. Please ensure that a name has been given to each of the new projects.");
                        return false;
                    }

                    // Verify that the name is not duplicate:
                    for (int j = 0; j < _context.ProjectsToMigrate.Count; j++)
                    {
                        if (j != i && _context.ProjectsToMigrate[j].DestinationName.ToLowerInvariant() == newProjectName.ToLowerInvariant())
                        {
                            MessageBox.Show(string.Format("The name '{0}' appears twice. Please change the name of one of the new projects and try again.", newProjectName));
                            return false;
                        }
                    }

                    // Verify that the name does not already exist in the solution:
                    foreach (Context.ProjectModel projectInSolution in _context.AllProjects)
                    {
                        if (projectInSolution.Name.ToLowerInvariant() == newProjectName.ToLowerInvariant())
                        {
                            MessageBox.Show(string.Format("A project named '{0}' already exists in the solution. Please choose a different name and try again.", newProjectName));
                            return false;
                        }
                    }

                    // Verify that the destination path does not already exist:
                    string projectFolderFullName = System.IO.Path.Combine(solutionFolderFullName, newProjectName);
                    if (!(projectFolderFullName.EndsWith("\\")))
                        projectFolderFullName += "\\";
                    if (Directory.Exists(projectFolderFullName))
                    {
                        MessageBox.Show(string.Format("A project named '{0}' already exists and is located at:\r\n\r\n{1}\r\n\r\nPlease choose a different name and try again.", newProjectName, projectFolderFullName));
                        return false;
                    }
                }
            }

            return true;
        }

        public bool CanGoBack
        {
            get
            {
                return true;
            }
        }

        #region Enable single-click edit in DataGrid

        // Credits: http://stackoverflow.com/questions/3426765/single-click-edit-in-wpf-datagrid

        private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);

                Control control = GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (control != null)
                {
                    control.Focus();
                }
            }
        }

        private T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild((prop), i) as DependencyObject;
                if (child == null)
                    continue;

                T castedProp = child as T;
                if (castedProp != null)
                    return castedProp;

                castedProp = GetFirstChildByType<T>(child);

                if (castedProp != null)
                    return castedProp;
            }
            return null;
        }

        #endregion
    }
}
