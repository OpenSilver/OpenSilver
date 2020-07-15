using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DotNetForHtml5.MigrationWizard
{
    internal static class Processor
    {
        internal static bool StaticAbortProcessing;
        internal static Dispatcher StaticDispatcher;
        internal static TextBox StaticProgressLog;

        const string Cshtml5AppProjectTemplateName = "SLMigration.EmptyApp.zip";
        const string Cshtml5LibraryProjectTemplateName = "SLMigration.EmptyLibrary.zip";
        static readonly string[] ExtensionsOfFilesToCopy = new string[] { ".resx", ".js", ".css", ".png", ".jpg", ".gif", ".ico", ".mp4", ".ogv", ".webm", ".3gp", ".mp3", ".ogg", ".txt", ".xml", ".ttf", ".woff", ".json" };

        public static async Task StartProcessingAsync(Context context, Action onCompleted)
        {
            // We execute on a background thread so that we can update the UI:
            await Task.Run(() =>
            {
                try
                {
                    // Log:
                    AddLogEntryOnUIThread(string.Format(@"Migration Started."));
                    AddLogEntryOnUIThread("");

                    if (context.Solution != null)
                    {
                        List<string> warnings = new List<string>();

                        // Iterate through the projects to migrate:
                        foreach (Context.ProjectModel projectToMigrate in context.ProjectsToMigrate)
                        {
                            string projectName = projectToMigrate.DestinationName;

                            //DisplayMessageBoxOnUIThread(ListProjectItems(projectToMigrate.Project.ProjectItems, 1));

                            // Create the new project:
                            string templateName = (projectToMigrate.ProjectOutputType == Context.ProjectOutputType.Application ? Cshtml5AppProjectTemplateName : Cshtml5LibraryProjectTemplateName);
                            CreateProject((Solution2)context.Solution, projectName, projectName, templateName, "CSharp");

                            // Find the newly created project:
                            EnvDTE.Project destinationProject = null;
                            foreach (EnvDTE.Project p in context.Solution.Projects)
                            {
                                if (p.Name == projectName)
                                {
                                    destinationProject = p;
                                    break;
                                }
                            }

                            if (destinationProject != null)
                            {
                                // Get some information about the project:
                                string sourceProjectFullPath;
                                if (projectToMigrate.ProjectLoadedWithEnvDTE != null)
                                    sourceProjectFullPath = projectToMigrate.ProjectLoadedWithEnvDTE.Properties.Item("FullPath").Value.ToString();
                                else
                                    sourceProjectFullPath = Path.GetDirectoryName(projectToMigrate.FullPathOfCSProj);
                                string sourceProjectFullPathNormalized = Path.GetFullPath(sourceProjectFullPath); // This will canonicalise the path (deal with multiple directory seperators, etc.)
                                string destinationProjectFullPath = destinationProject.Properties.Item("FullPath").Value.ToString();
                                string destinationProjectFullPathNormalized = Path.GetFullPath(destinationProjectFullPath); // This will canonicalise the path (deal with multiple directory seperators, etc.)

                                // Delete the default files that are in the new projects:
                                if (projectToMigrate.ProjectOutputType == Context.ProjectOutputType.Application)
                                {
                                    DeleteFileInProject(destinationProject, "App.xaml"); // Note: this will also automatically delete "App.xaml.cs".
                                    DeleteFileInProject(destinationProject, "MainPage.xaml"); // Note: this will also automatically delete "MainPage.xaml.cs".
                                }
                                else
                                {
                                    DeleteFileInProject(destinationProject, "Class1.cs");
                                }

                                // Get the root project items:
                                IEnumerable<ProjectItemWrapper> projectItems;
                                if (projectToMigrate.ProjectLoadedWithEnvDTE != null)
                                    projectItems = ProjectItemWrapper.GetProjectItemsFromProject(projectToMigrate.ProjectLoadedWithEnvDTE);
                                else if (projectToMigrate.ProjectLoadedWithMsBuild != null)
                                    projectItems = ProjectItemWrapper.GetProjectItemsFromProject(projectToMigrate.ProjectLoadedWithMsBuild);
                                else
                                    throw new InvalidOperationException("Project is null.");

                                // Copy files and folders:
                                CopyFilesAndFoldersRecursively(projectItems, destinationProject.ProjectItems, context.HowToDealWithCSharpFiles);
                            }
                            else
                            {
                                DisplayMessageBoxOnUIThread(string.Format("The project named '{0}' could not be found. Please report this issue to support@cshtml5.com", projectName));
                            }

                            if (StaticAbortProcessing)
                                break;
                        }

                        //todo: recreate the references (and use AbortProcessing): https://mhusseini.wordpress.com/2013/05/29/get-project-references-from-envdte-project/
                    }

                    // Log:
                    AddLogEntryOnUIThread("");
                    if (StaticAbortProcessing)
                        AddLogEntryOnUIThread(@"Migration Aborted.");
                    else
                    {
                        AddLogEntryOnUIThread(@"Migration Completed."
                            + Environment.NewLine
                            + Environment.NewLine
                            + @"The operation has completed successfully." + Environment.NewLine
                            + @"Please note that, as written in the" + Environment.NewLine
                            + @"Introduction, this Migration Wizard only" + Environment.NewLine
                            + @"helps to prepare the projects, folders," + Environment.NewLine
                            + @"and files. The rest of the migration" + Environment.NewLine
                            + @"still requires manual work, as explained" + Environment.NewLine
                            + @"in the Silverlight Migration Guide," + Environment.NewLine
                            + @"available at:"
                            + Environment.NewLine
                            + Environment.NewLine
                            + @"http://cshtml5.com/links/migrating-from-silverlight.aspx"
                            + Environment.NewLine
                            + Environment.NewLine
                            + @"You can now close this window. Please" + Environment.NewLine
                            + @"visit the URL above for more information."
                            );
                    }
                }
                catch (Exception ex)
                {
                    string errorDescription = "The error below has occurred. Please report this error to: support@cshtml5.com"
                        + Environment.NewLine
                        + Environment.NewLine
                        + ex.ToString();
                    DisplayMessageBoxOnUIThread(errorDescription);
                    AddLogEntryOnUIThread("");
                    AddLogEntryOnUIThread(errorDescription);
                    AddLogEntryOnUIThread("");
                    AddLogEntryOnUIThread("Migration Aborted.");
                }

                // Done (go back to UI thread):
                StaticDispatcher.BeginInvoke((Action)(() =>
                {
                    onCompleted();
                }));
            });
        }

        static void CopyFilesAndFoldersRecursively(IEnumerable<ProjectItemWrapper> sourceProjectItems, EnvDTE.ProjectItems destinationProjectItems, Context.CopyOrAddAsLink howToDealWithCSharpFiles, int level = 1)
        {
            if (!StaticAbortProcessing)
            {
                foreach (ProjectItemWrapper sourceItem in sourceProjectItems)
                {
                    string sourceItemName = sourceItem.GetName();
                    string sourceItemFullPath = sourceItem.GetFullPath();

                    //---------------------
                    // If it is a folder:
                    //---------------------
                    if (FoldersHelper.IsAFolder(sourceItemFullPath))
                    {
                        // If it is NOT the "Properties" folder (which exists by default in all projects):
                        if (level != 1 || sourceItemName.ToLower() != "properties")
                        {
                            // Create a new folder with the same name in the destination project:
                            EnvDTE.ProjectItem newFolder = destinationProjectItems.AddFolder(sourceItemName);

                            // Log:
                            AddLogEntryOnUIThread(string.Format(@"Created folder ""{0}"".", sourceItemName));

                            // Continue the recursion:
                            CopyFilesAndFoldersRecursively(sourceItem.GetChildItems(), newFolder.ProjectItems, howToDealWithCSharpFiles, level + 1);
                        }
                    }
                    //---------------------
                    // If it is a file:
                    //---------------------
                    else
                    {
                        EnvDTE.ProjectItem newFile = null;
                        if (sourceItemFullPath.ToLower().EndsWith(".cs"))
                        {
                            //---------------------
                            // C# file:
                            //---------------------

                            // Check if the file exists (this can be the case for example if we previously copied a file such as "UserControl.xaml", which automatically copies its child "UserControl.xaml.cs"):
                            EnvDTE.ProjectItem existingItem = FindProjectItemOrNull(sourceItemName, destinationProjectItems);

                            // Copy the file or add it as link:
                            if (howToDealWithCSharpFiles == Context.CopyOrAddAsLink.Copy)
                            {
                                // Copy only if the file is not already there:
                                if (existingItem == null)
                                {
                                    newFile = destinationProjectItems.AddFromFileCopy(sourceItemFullPath);
                                }

                                // Log:
                                AddLogEntryOnUIThread(string.Format(@"Copied file ""{0}"".", sourceItemName));
                            }
                            else if (howToDealWithCSharpFiles == Context.CopyOrAddAsLink.AddAsLink)
                            {
                                // Delete the copied file in order to replace it with a linked file (this can happen for example if we previously copied a file such as "UserControl.xaml", which automatically copies its child "UserControl.xaml.cs", so we need to delete this child):
                                if (existingItem != null)
                                    existingItem.Delete();

                                // Add the file "as link":
                                newFile = destinationProjectItems.AddFromFile(sourceItemFullPath);

                                // Log:
                                AddLogEntryOnUIThread(string.Format(@"Added file ""{0}"" as link.", sourceItemName));
                            }
                            else
                                throw new NotSupportedException();
                        }
                        else if (sourceItemFullPath.ToLower().EndsWith(".xaml")
                            || DoesFileHaveOneOfTheseExtensions(sourceItemFullPath, ExtensionsOfFilesToCopy)) // JPG, PNG, etc.
                        {
                            //---------------------
                            // XAML file:
                            //---------------------

                            // Copy the file:
                            newFile = destinationProjectItems.AddFromFileCopy(sourceItemFullPath);

                            // Log:
                            AddLogEntryOnUIThread(string.Format(@"Copied file ""{0}"".", sourceItemName));
                        }

                        // Continue the recursion:
                        if (newFile != null)
                        {
                            CopyFilesAndFoldersRecursively(sourceItem.GetChildItems(), newFile.ProjectItems, howToDealWithCSharpFiles, level + 1);
                        }
                    }

                    if (StaticAbortProcessing)
                        break;
                }
            }
        }

        static bool DoesFileHaveOneOfTheseExtensions(string fileName, IEnumerable<string> extensions)
        {
            foreach (var extension in extensions)
            {
                if (fileName.ToLower().EndsWith(extension.ToLower()))
                    return true;
            }
            return false;
        }

        static void CreateProject(Solution2 solution2, string projectSubFolder, string projectName, string projectTemplateName, string language)
        {
            // Credits: https://www.mztools.com/articles/2013/MZ2013022.aspx

            string solutionFileFullName;
            string solutionFolderFullName;
            string projectTemplateFileFullName;
            string projectFolderFullName;

            try
            {
                // Get the full name of the solution file
                solutionFileFullName = solution2.FileName;

                // Get the full name of the solution folder
                solutionFolderFullName = System.IO.Path.GetDirectoryName(solutionFileFullName);

                // Compose the full name of the project folder
                projectFolderFullName = System.IO.Path.Combine(solutionFolderFullName, projectSubFolder);
                if (!(projectFolderFullName.EndsWith("\\")))
                {
                    projectFolderFullName += "\\";
                }

                // Get the project template
                projectTemplateFileFullName = solution2.GetProjectTemplate(projectTemplateName, language);

                // Add the project
                solution2.AddFromTemplate(projectTemplateFileFullName, projectFolderFullName, projectName, false);

                // Log:
                AddLogEntryOnUIThread(string.Format(@""));
                AddLogEntryOnUIThread(string.Format(@"==============================================="));
                AddLogEntryOnUIThread(string.Format(@"Created project ""{0}"".", projectName));
                AddLogEntryOnUIThread(string.Format(@"==============================================="));
                AddLogEntryOnUIThread(string.Format(@""));
            }
            catch (Exception ex)
            {
                DisplayMessageBoxOnUIThread(ex.ToString());
            }
        }

        static bool DeleteFileInProject(EnvDTE.Project project, string fileName)
        {
            EnvDTE.ProjectItems projectItems = project.ProjectItems;
            try
            {
                projectItems.Item(fileName).Delete();
                return true;
            }
            catch (Exception ex)
            {
                DisplayMessageBoxOnUIThread(string.Format("Unable to delete '{0}'.", fileName) + Environment.NewLine + Environment.NewLine + ex.ToString());
                return false;
            }
        }

        static void AddLogEntryOnUIThread(string text)
        {
            if (StaticDispatcher != null && StaticProgressLog != null)
            {
                StaticDispatcher.BeginInvoke((Action)(() =>
                {
                    StaticProgressLog.AppendText(text + Environment.NewLine);
                    StaticProgressLog.ScrollToEnd();
                }));
            }
        }

        static EnvDTE.ProjectItem FindProjectItemOrNull(string name, EnvDTE.ProjectItems parentCollection)
        {
            foreach (EnvDTE.ProjectItem projectItem in GetFilesIncludingSubFiles(parentCollection))
            {
                string projectItemName = projectItem.Name;
                if (projectItemName.ToLower() == name.ToLower())
                    return projectItem;
            }
            return null;
        }

        /// <summary>
        /// Returns the ProjectItems contained in the passed collection, as well as the "sub project items", which
        /// are the project items that are dependent on other project items. For example, "MainPage.xaml.cs" is
        /// a sub project item of "MainPage.xaml".
        /// </summary>
        /// <param name="parentCollection">The collection of project items.</param>
        /// <returns></returns>
        static IEnumerable<EnvDTE.ProjectItem> GetFilesIncludingSubFiles(EnvDTE.ProjectItems parentCollection)
        {
            foreach (EnvDTE.ProjectItem projectItem in parentCollection)
            {
                if (!FoldersHelper.IsAFolder(projectItem))
                {
                    yield return projectItem;
                    foreach (EnvDTE.ProjectItem subProjectItem in GetFilesIncludingSubFiles(projectItem.ProjectItems))
                    {
                        yield return subProjectItem;
                    }
                }
            }
        }

        static void DisplayMessageBoxOnUIThread(string text)
        {
            if (StaticDispatcher != null)
            {
                StaticDispatcher.BeginInvoke((Action)(() =>
                {
                    MessageBox.Show(text);
                }));
            }
        }

        static IEnumerable<EnvDTE.ProjectItem> TraverseProjectItems(EnvDTE.ProjectItems projectItems)
        {
            foreach (EnvDTE.ProjectItem projectItem in projectItems)
            {
                yield return projectItem;
                foreach (EnvDTE.ProjectItem childProjectItem in TraverseProjectItems(projectItem.ProjectItems))
                {
                    yield return childProjectItem;
                }
            }
        }

        static string ListProjectItems(EnvDTE.ProjectItems projectItems, int level)
        {
            string result = "";
            foreach (EnvDTE.ProjectItem projectItem in projectItems)
            {
                result += projectItem.Name + " {" + projectItem.Properties.Item("FullPath").Value.ToString() + "}" + " (" + level.ToString() + ")" + Environment.NewLine;
                if (projectItem.ProjectItems != null)
                {
                    result += ListProjectItems(projectItem.ProjectItems, level + 1);
                }
            }
            return result;
        }
    }
}
