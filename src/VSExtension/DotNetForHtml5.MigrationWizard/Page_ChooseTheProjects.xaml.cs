using DotNetForHtml5.MigrationWizard.Utils;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Page_ChooseTheProjects.xaml
    /// </summary>
    public partial class Page_ChooseTheProjects : Page, IWizardPage
    {
        const string SilverlightProjectTypeGuid = "A1591282-1198-4647-A2B1-27E5FF5F6F3B"; // Credits: https://www.mztools.com/articles/2008/MZ2008017.aspx
        const string WpfProjectTypeGuid = "60DC8134-EBA5-43B8-BCC9-BB4BC16C2548";

        // Credits for DataGrid with CheckBoxes:
        // http://blog.scottlogic.com/2008/11/26/multiselect-datagrid-with-checkboxes.html


        Context _context;

        public Page_ChooseTheProjects(Context context)
        {
            InitializeComponent();

            _context = context;

            // Clear the list of projects:
            _context.AllProjects.Clear();
            _context.CandidateProjectsToMigrate.Clear();

            // Check if we are running inside VS:
            if (_context.Solution != null)
            {
                // Get the list of projects:
                List<EnvDTE.Project> allProjects = AllSolutionProjectsSearcher.GetAllProjects(_context.Solution);

                // Process the list of projects:
                foreach (EnvDTE.Project project in allProjects)
                {
                    // Get project information (path, type, etc.):
                    string fullPathOfCSProj;
                    Context.ProjectType projectType;
                    Context.ProjectOutputType projectOutputType;
                    GetProjectInformation(project, context.Solution, out fullPathOfCSProj, out projectType, out projectOutputType);

                    // Create a new model for the project:
                    var projectModel = new Context.ProjectModel()
                    {
                        Name = project.Name,
                        FullPathOfCSProj = fullPathOfCSProj,
                        ProjectLoadedWithEnvDTE = project,
                        IsSelected = false,
                        DestinationName = AppendCSHTML5ToProjectName(project.Name),
                        DestinationProjectType = "CSHTML5",
                        ProjectType = projectType,
                        ProjectOutputType = projectOutputType
                    };

                    // Add the project:
                    _context.AllProjects.Add(projectModel);

                    // If the project is of type "Silverlight" and the other information is ok, it is a valid candidate for the migration:
                    if (projectType == Context.ProjectType.Silverlight
                        && projectOutputType != Context.ProjectOutputType.Unknown)
                    {
                        _context.CandidateProjectsToMigrate.Add(projectModel);
                    }
                }

                // Diplay the candidate projects:
                ProjectsDataGrid.ItemsSource = _context.CandidateProjectsToMigrate;
            }
            else
            {
                ProjectsDataGrid.ItemsSource = new List<Context.ProjectModel> {
                    new Context.ProjectModel() { Name = "Test1" },
                    new Context.ProjectModel() { Name = "Test2" } };
            }
        }

        static string AppendCSHTML5ToProjectName(string originalProjectName)
        {
            const string silverlightSuffixLowercase = ".silverlight";
            const string silverlightPrefixLowercase = "silverlight.";
            const string cshtml5Suffix = ".Cshtml5";
            const string cshtml5Prefix = "CSHTML5.";

            string newProjectName;

            // Replace the ".Silverlight" prefix or suffix if any:
            if (originalProjectName.ToLower().EndsWith(silverlightSuffixLowercase))
            {
                // Remove the original suffix (works with both lower and upper case):
                newProjectName = originalProjectName.Substring(0, (originalProjectName.Length - silverlightSuffixLowercase.Length));

                // Append the new suffix:
                newProjectName = newProjectName + cshtml5Suffix;
            }
            else if (originalProjectName.ToLower().StartsWith(silverlightPrefixLowercase))
            {
                // Remove the original prefix (works with both lower and upper case):
                newProjectName = originalProjectName.Substring(silverlightPrefixLowercase.Length);

                // Append the new prefix:
                newProjectName = cshtml5Prefix + newProjectName;
            }
            else
            {
                newProjectName = originalProjectName + cshtml5Suffix;
            }

            return newProjectName;
        }

        public bool TryCreateNextPage(Context context, out Page nextPage)
        {
            if (ProjectsDataGrid.SelectedItems.Count > 0)
            {
                // Retain only the selected projects:
                _context.ProjectsToMigrate = new List<Context.ProjectModel>(ProjectsDataGrid.SelectedItems.Cast<Context.ProjectModel>());

                nextPage = new Page_ChooseDestinationNames(context);
                return true;
            }
            else
            {
                MessageBox.Show("No project selected. Please select the project(s) to migrate and click Next.");
                nextPage = null;
                return false;
            }
        }

        public bool CanGoBack
        {
            get
            {
                return true;
            }
        }

        static void GetProjectInformation(EnvDTE.Project project, EnvDTE.Solution solution, out string fullPathOfCSProj, out Context.ProjectType projectType, out Context.ProjectOutputType projectOutputType)
        {
            string projectTypeGuids = GetProjectTypeGuids(project);

            // Get the full path of the CSPROJ:
            fullPathOfCSProj = project.FullName;

            // Get the Project Type:
            projectType = GetProjectType(projectTypeGuids);

            // Define some helpful functions:
            Func<string, string> functionToGetAProjectProperty = (string propertyName) =>
                {
                    var property = project.Properties.Item(propertyName); // Credits: https://www.mztools.com/articles/2007/mz2007014.aspx
                    if (property != null && property.Value != null)
                    {
                        return property.Value.ToString();
                    }
                    else
                    {
                        return null;
                    }
                };

            Func<string, string> functionToGetACSProjProperty = (string propertyName) =>
                {
                    return GetPropertyFromCSProj(propertyName, "Debug", project);
                };

            // Get the project Output Type:
            projectOutputType = GetProjectOutputType(projectType, functionToGetAProjectProperty, functionToGetACSProjProperty);
        }

        static bool TryLoadProjectFromCsproj(string csprojFullPath, out Microsoft.Build.Evaluation.Project project, out string projectName, out Context.ProjectType projectType, out Context.ProjectOutputType projectOutputType)
        {
            try
            {
                Microsoft.Build.Evaluation.ProjectCollection collection = new Microsoft.Build.Evaluation.ProjectCollection();
                collection.DefaultToolsVersion = "4.0";
                Microsoft.Build.Evaluation.Project theProject = collection.LoadProject(csprojFullPath);
                string projectTypeGuids = theProject.GetPropertyValue("ProjectTypeGuids"); //cf. https://github.com/Microsoft/visualfsharp/blob/master/vsintegration/src/FSharp.ProjectSystem.Base/Project/ProjectFactory.cs
                string isSilverlightApplicationString = theProject.GetPropertyValue("SilverlightApplication");

                // Get the project name:
                projectName = System.IO.Path.GetFileNameWithoutExtension(csprojFullPath);

                // Get the Project Type:
                projectType = GetProjectType(projectTypeGuids);

                // Get the project Output Type:
                Func<string, string> functionToGetAProjectProperty = (string propertyName) =>
                    {
                        return theProject.GetPropertyValue(propertyName);
                    };
                projectOutputType = GetProjectOutputType(projectType, functionToGetAProjectProperty, functionToGetAProjectProperty);

                // Return the project itself as well:
                project = theProject;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                project = null;
                projectName = null;
                projectType = Context.ProjectType.Other;
                projectOutputType = Context.ProjectOutputType.Unknown;
                return false;
            }
        }

        static Context.ProjectType GetProjectType(string projectTypeGuids)
        {
            Context.ProjectType projectType = Context.ProjectType.Other; // Default value
            if (projectTypeGuids.Contains(SilverlightProjectTypeGuid))
                projectType = Context.ProjectType.Silverlight;
            else if (projectTypeGuids.Contains(WpfProjectTypeGuid))
                projectType = Context.ProjectType.WPF;

            return projectType;
        }

        static Context.ProjectOutputType GetProjectOutputType(Context.ProjectType sourceProjectType, Func<string, string> functionToGetAProjectProperty, Func<string, string> functionToGetACSProjProperty)
        {
            Context.ProjectOutputType projectOutputType = Context.ProjectOutputType.Unknown; // Default value
            if (sourceProjectType == Context.ProjectType.WPF)
            {
                string outputTypeProperty = functionToGetAProjectProperty("OutputType");
                if (outputTypeProperty == "1")
                    projectOutputType = Context.ProjectOutputType.Application;
                else if (outputTypeProperty == "2")
                    projectOutputType = Context.ProjectOutputType.Application;
            }
            else if (sourceProjectType == Context.ProjectType.Silverlight)
            {
                var silverlightApplicationPropertyValue = functionToGetACSProjProperty("SilverlightApplication");
                if (silverlightApplicationPropertyValue != null)
                {
                    if (silverlightApplicationPropertyValue.Trim().ToLower() == "true")
                        projectOutputType = Context.ProjectOutputType.Application;
                    else if (silverlightApplicationPropertyValue.Trim().ToLower() == "false")
                        projectOutputType = Context.ProjectOutputType.Library;
                }
            }

            return projectOutputType;
        }

        static string GetPropertyFromCSProj(string propertyName, string configurationName, EnvDTE.Project proj)
        {
            // Credits: https://social.msdn.microsoft.com/Forums/vstudio/en-US/d3ee9c26-02ca-45cc-b930-02cbf09f18b6/modify-custom-property-without-modifying-csproj-file?forum=vsx

            object service = GetService(proj.DTE, typeof(IVsSolution));
            IVsSolution solution = (IVsSolution)service;
            IVsHierarchy hierarchy;
            int result = solution.GetProjectOfUniqueName(proj.UniqueName, out hierarchy);
            if (result == 0)
            {
                IVsBuildPropertyStorage buildPropertyStorage = hierarchy as IVsBuildPropertyStorage;
                if (buildPropertyStorage != null)
                {
                    string value;
                    buildPropertyStorage.GetPropertyValue(propertyName, configurationName, (uint)_PersistStorageType.PST_PROJECT_FILE, out value);
                    return value ?? "";
                }
            }
            return "";
        }

        static string GetProjectTypeGuids(EnvDTE.Project proj)
        {
            // Credits: https://www.mztools.com/articles/2007/MZ2007016.aspx

            string projectTypeGuids = "";

            object service = GetService(proj.DTE, typeof(IVsSolution));
            IVsSolution solution = (IVsSolution)service;
            IVsHierarchy hierarchy;
            int result = solution.GetProjectOfUniqueName(proj.UniqueName, out hierarchy);
            if (result == 0)
            {
                IVsAggregatableProject aggregatableProject = hierarchy as IVsAggregatableProject;
                if (aggregatableProject != null)
                    result = aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids);
            }

            return projectTypeGuids;

        }

        static object GetService(object serviceProvider, System.Type type)
        {
            return GetService(serviceProvider, type.GUID);
        }

        static object GetService(object serviceProviderObject, System.Guid guid)
        {
            // Credits: https://www.mztools.com/articles/2007/MZ2007016.aspx

            object service = null;
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider = null;
            IntPtr serviceIntPtr;
            int hr = 0;
            Guid SIDGuid;
            Guid IIDGuid;

            SIDGuid = guid;
            IIDGuid = SIDGuid;
            serviceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)serviceProviderObject;
            hr = serviceProvider.QueryService(SIDGuid, IIDGuid, out serviceIntPtr);

            if (hr != 0)
            {
                System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(hr);
            }
            else if (!serviceIntPtr.Equals(IntPtr.Zero))
            {
                service = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(serviceIntPtr);
                System.Runtime.InteropServices.Marshal.Release(serviceIntPtr);
            }

            return service;
        }

        private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            ProjectsDataGrid.SelectAll();
        }

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Visual Studio project files (*.csproj)|*.csproj|All files (*.*)|*.*";
            dialog.Title = "Please select a Visual Studio project file.";
            if (dialog.ShowDialog() == true)
            {
                string fullPathOfCSProj = dialog.FileName;

                Microsoft.Build.Evaluation.Project project;
                string projectName;
                Context.ProjectType projectType;
                Context.ProjectOutputType projectOutputType;

                if (TryLoadProjectFromCsproj(fullPathOfCSProj, out project, out projectName, out projectType, out projectOutputType))
                {
                    // If the selected project is not of a compatible type, ask for confirmation:
                    if (projectType != Context.ProjectType.Other
                        || System.Windows.MessageBox.Show("The selected project does not appear to be of a supported type (Silverlight, WPF...). Would you like to migrate it anyway?", "Please confirm", MessageBoxButton.OKCancel) != MessageBoxResult.Cancel)
                    {
                        // Check if the selected project has already been added to the list:
                        bool alreadyAdded = false;
                        foreach (Context.ProjectModel projectModel in _context.AllProjects)
                        {
                            if (projectModel.FullPathOfCSProj.ToLower() == fullPathOfCSProj.ToLower())
                            {
                                alreadyAdded = true;
                                break;
                            }
                        }
                        if (!alreadyAdded)
                        {
                            // Create a new model for the project:
                            var newProjectModel = new Context.ProjectModel()
                            {
                                Name = projectName,
                                FullPathOfCSProj = fullPathOfCSProj,
                                ProjectLoadedWithMsBuild = project,
                                IsSelected = true,
                                DestinationName = AppendCSHTML5ToProjectName(projectName),
                                DestinationProjectType = "CSHTML5",
                                ProjectType = projectType,
                                ProjectOutputType = projectOutputType
                            };

                            // Add the project:
                            _context.AllProjects.Add(newProjectModel);
                            _context.CandidateProjectsToMigrate.Add(newProjectModel);

                            // Refresh the list of projects:
                            ProjectsDataGrid.ItemsSource = null;
                            ProjectsDataGrid.ItemsSource = _context.CandidateProjectsToMigrate;
                            
                            // Make sure the newly added project is selected:
                            ProjectsDataGrid.SelectedItems.Add(newProjectModel);
                        }
                        else
                        {
                            MessageBox.Show("The selected project is already present in the list.");
                        }
                    }
                }
            }          
        }
    }
}
