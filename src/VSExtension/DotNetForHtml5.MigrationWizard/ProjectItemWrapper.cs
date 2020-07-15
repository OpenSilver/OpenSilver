using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.MigrationWizard
{
    /// <summary>
    /// This will wrap either an EnvDTE.ProjectItem or a Microsoft.Build.Evaluation.ProjectItem, depending on how the project was loaded (via the current solution or via the "Browse..." button)
    /// </summary>
    internal class ProjectItemWrapper
    {
        //EnvDTE.Project _projectLoadedWithEnvDTE;
        EnvDTE.ProjectItem _projectItemInCaseOfProjectLoadedWithEnvDTE;

        //Microsoft.Build.Evaluation.Project _projectLoadedWithMsBuild;
        Microsoft.Build.Evaluation.ProjectItem _projectItemInCaseOfProjectLoadedWithMsBuild;
        FileWalker _fileWalkerInCaseOfProjectLoadedWithMsBuild;
        FolderWalker _folderWalkerInCaseOfProjectLoadedWithMsBuild;

        public ProjectItemWrapper(EnvDTE.ProjectItem projectItemLoadedWithEnvDTE)
        {
            _projectItemInCaseOfProjectLoadedWithEnvDTE = projectItemLoadedWithEnvDTE;
        }

        public ProjectItemWrapper(Microsoft.Build.Evaluation.ProjectItem projectItemLoadedWithMsBuild, FileWalker fileWalker)
        {
            _projectItemInCaseOfProjectLoadedWithMsBuild = projectItemLoadedWithMsBuild;
            _fileWalkerInCaseOfProjectLoadedWithMsBuild = fileWalker;
        }

        public ProjectItemWrapper(FolderWalker folderWalker)
        {
            _folderWalkerInCaseOfProjectLoadedWithMsBuild = folderWalker;
        }

        public string GetName()
        {
            if (_projectItemInCaseOfProjectLoadedWithEnvDTE != null)
            {
                return _projectItemInCaseOfProjectLoadedWithEnvDTE.Name;
            }
            else if (_projectItemInCaseOfProjectLoadedWithMsBuild != null)
            {
                string fullPath = _projectItemInCaseOfProjectLoadedWithMsBuild.GetMetadataValue("FullPath");
                return System.IO.Path.GetFileName(fullPath);
            }
            else if (_folderWalkerInCaseOfProjectLoadedWithMsBuild != null)
            {
                return _folderWalkerInCaseOfProjectLoadedWithMsBuild.Name;
            }
            else
                throw new InvalidOperationException("Project item is null.");
        }

        public string GetFullPath()
        {
            if (_projectItemInCaseOfProjectLoadedWithEnvDTE != null)
            {
                return _projectItemInCaseOfProjectLoadedWithEnvDTE.Properties.Item("FullPath").Value.ToString();
            }
            else if (_projectItemInCaseOfProjectLoadedWithMsBuild != null)
            {
                return _projectItemInCaseOfProjectLoadedWithMsBuild.GetMetadataValue("FullPath");
            }
            else if (_folderWalkerInCaseOfProjectLoadedWithMsBuild != null)
            {
                return _folderWalkerInCaseOfProjectLoadedWithMsBuild.FullPath;
            }
            else
                throw new InvalidOperationException("Project item is null.");
        }

        public IEnumerable<ProjectItemWrapper> GetChildItems()
        {
            if (_projectItemInCaseOfProjectLoadedWithEnvDTE != null)
            {
                foreach (EnvDTE.ProjectItem childProjectItem in _projectItemInCaseOfProjectLoadedWithEnvDTE.ProjectItems)
                {
                    yield return new ProjectItemWrapper(childProjectItem);
                }
            }
            else if (_projectItemInCaseOfProjectLoadedWithMsBuild != null)
            {
                yield break;
            }
            else if (_folderWalkerInCaseOfProjectLoadedWithMsBuild != null)
            {
                foreach (ProjectItemWrapper projectItemWrapper in GetChildItemsFromFolder(_folderWalkerInCaseOfProjectLoadedWithMsBuild))
                {
                    yield return projectItemWrapper;
                }
            }
            else
                throw new InvalidOperationException("Both the project item and _folderWalkerInCaseOfProjectLoadedWithMsBuild are null.");
        }

        public static IEnumerable<ProjectItemWrapper> GetProjectItemsFromProject(EnvDTE.Project projectLoadedWithEnvDTE)
        {
            foreach (EnvDTE.ProjectItem childProjectItem in projectLoadedWithEnvDTE.ProjectItems)
            {
                yield return new ProjectItemWrapper(childProjectItem);
            }
        }

        static HashSet<string> AllowedItemTypes = new HashSet<string>() { "folder", "content", "compile", "page", "applicationdefinition", "resource", "embeddedresource" };

        public static IEnumerable<ProjectItemWrapper> GetProjectItemsFromProject(Microsoft.Build.Evaluation.Project projectLoadedWithMsBuild)
        {
            // Projects loaded with MsBuild have a different way of iterating through the files.
            // Instead of returning only the items in the root, and then you recursively get the items in the sub-folders,
            // we can only get the full list of all the items at once, and we should deduce the folders from their path.
            // Therefore we have developed a class named "FileTreeWalker" which takes as input the full list of items,
            // and return a sort of file system, where you can get the root files, and then the folders, etc.

            //-------------------------------------
            // We initialize the FileTreeWalker by reading the list of items from the CSPROJ and passing that list to the FileTreeWalker:
            //-------------------------------------

            // Get the list of files from the CSPROJ:
            List<Tuple<string, object>> projectItemsPathsAndReference = new List<Tuple<string, object>>();
            foreach (Microsoft.Build.Evaluation.ProjectItem projectItem in projectLoadedWithMsBuild.Items)
            {
                // Keep only the items that may correspond to files or folders (not the <Reference>mscorlic</reference> and stuff like that)
                if (AllowedItemTypes.Contains(projectItem.ItemType.ToLower()))
                {
                    //--------------------
                    // Determine the file path relative to the project root:
                    //--------------------
                    string filePathRelativeToProjectRoot;

                    // If the file was added "As Link", we need to use the value of the "Link" metadata to know the path in the project structure:
                    string valueOfLinkMetadata = projectItem.GetMetadataValue("Link");
                    if (!string.IsNullOrEmpty(valueOfLinkMetadata))
                        filePathRelativeToProjectRoot = valueOfLinkMetadata;
                    else
                        filePathRelativeToProjectRoot = projectItem.EvaluatedInclude;

                    projectItemsPathsAndReference.Add(new Tuple<string, object>(filePathRelativeToProjectRoot, projectItem));
                }
            }

            // Initialize the FileTreeWalker:
            FileTreeWalker fileTreeWalker = new FileTreeWalker(projectItemsPathsAndReference);

            // Return the root items (files and folders):
            FolderWalker rootFolder = fileTreeWalker.GetRootFolder();
            return GetChildItemsFromFolder(rootFolder);
        }

        static public IEnumerable<ProjectItemWrapper> GetChildItemsFromFolder(FolderWalker folderWalker)
        {
            // Return the files:
            // IMPORTANT: we sort the files by name so that the files ".XAML" are returned before their code-behind ".XAML.CS". This order is important because when we copy the former, the latter gets copied as well. So when we arrive to the code-behind, we skip it because we are able to check that it already exists. If, on the contrary, we did the other way around, we would get an error when copying the ".XAML" because it says that it cannot copy the ".XAML.CS" because it already exists.
            foreach (FileWalker fileWalker in folderWalker.GetFiles(sortByName: true))
            {
                Microsoft.Build.Evaluation.ProjectItem projectItem = (Microsoft.Build.Evaluation.ProjectItem)fileWalker.UserState;

                yield return new ProjectItemWrapper(projectItem, fileWalker);
            }

            // And also return then the sub-folders:
            foreach (FolderWalker subFolderWalker in folderWalker.GetFolders())
            {
                yield return new ProjectItemWrapper(subFolderWalker);
            }
        }
    }
}
