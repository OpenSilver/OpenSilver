using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner
{
    internal static class ResolvingReferencedXamlResources
    {
        static readonly XNamespace DefaultXamlNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        static readonly XNamespace xNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml"; // Used for example for "x:Name" attributes.
        
        public static IEnumerable<XElement> GetAppDotXamlResources(EnvDTE.Project currentProject, EnvDTE.Solution currentSolution, out string appDotXamlFullPath)
        {
            // Attempt to find the file "App.xaml":
            ProjectItem appDotXaml = FindAppDotXaml(currentProject, currentSolution);
            if (appDotXaml != null)
            {
                // Get the full path:
                appDotXamlFullPath = appDotXaml.Properties.Item("FullPath").Value.ToString();

                // Get the file content:
                string appDotXamlContent = File.ReadAllText(appDotXamlFullPath);

                // Remove the content of all the "HtmlPresenter" nodes, because the content may be not well formatted and may lead to a syntax error when parsing the XDocument:
                appDotXamlContent = HtmlPresenterRemover.RemoveHtmlPresenterNodes(appDotXamlContent);

                // Load the file as XML:
                XDocument xdoc = XDocument.Parse(appDotXamlContent);

                // Get the resources:
                var applicationDotResourcesNode = xdoc.Descendants(DefaultXamlNamespace + "Application.Resources").FirstOrDefault();
                if (applicationDotResourcesNode != null)
                {
                    IEnumerable<XElement> elementsInsideApplicationDotResourcesNode = applicationDotResourcesNode.Elements(); // This is either <ResourceDictionary>...</ResourceDictionary> or it is the list of resources (in case that <ResourceDictionary>...</ResourceDictionary> is implied).

                    // Determine whether the <ResourceDictionary> tag is omitted (implicit) or explicitly specified:
                    bool isTheResourceDictionaryTagExplicitlySpecified =
                        elementsInsideApplicationDotResourcesNode.Any() && elementsInsideApplicationDotResourcesNode.First().Name == (DefaultXamlNamespace + "ResourceDictionary");

                    // Change all the the resources "x:Name" into "x:Key" so that we do not get an error if there are two entries with the same "x:Name" in the whole document:
                    if (isTheResourceDictionaryTagExplicitlySpecified)
                    {
                        ChangeAllXNameIntoXKey(elementsInsideApplicationDotResourcesNode.First());
                    }
                    else
                    {
                        ChangeAllXNameIntoXKey(applicationDotResourcesNode);
                    }

                    return elementsInsideApplicationDotResourcesNode;
                }
            }

            // Failure:
            appDotXamlFullPath = null;
            return null;
        }

        static ProjectItem FindAppDotXaml(EnvDTE.Project currentProject, EnvDTE.Solution currentSolution)
        {
            // First, look for a file named "App.xaml" in the root of the current project:
            foreach (EnvDTE.ProjectItem projectItem in currentProject.ProjectItems)
            {
                string fileName = projectItem.Name;
                if (fileName.ToLowerInvariant() == "app.xaml")
                    return projectItem;
            }

            // If not found, iterate through all the referencing projects in the solution, and look there:
            foreach (Project project in SolutionProjectsHelper.GetAllProjectsInSolution(currentSolution))
            {
                // Check if the project references the current project:
                if (SolutionProjectsHelper.DoesProjectAReferenceProjectB(project, currentProject))
                {
                    // Find a file named "app.xaml" in the referenced project:
                    foreach (EnvDTE.ProjectItem projectItem in project.ProjectItems)
                    {
                        string fileName = projectItem.Name;
                        if (fileName.ToLowerInvariant() == "app.xaml")
                            return projectItem;
                    }
                }
            }

            return null;
        }

        /*
        static IEnumerable<XElement> GetResourcesUnderApplicationDotResourcesNode(XElement applicationDotResources, out XElement resourceDictionaryNode)
        {
            // First, check if the "<ResourceDictionary>" is explicitly specified, or if it is implied:
            resourceDictionaryNode = applicationDotResources.Elements(DefaultXamlNamespace + "ResourceDictionary").FirstOrDefault();
            bool isResourceDictionarySpecified = (resourceDictionaryNode != null);

            // If no "<ResourceDictionary>" is explicitly specified, it means that it is implicit, so we set it to be the parent node:
            if (!isResourceDictionarySpecified)
                resourceDictionaryNode = applicationDotResources;

            // Get the children of the ResourceDictionary:
            IEnumerable<XElement> childrenOfResourceDictionary = resourceDictionaryNode.Elements();

            // Get the resources by taking the children of the ResourceDictionary and removing the ones that correspond to ResourceDictionary properties (such as <ResourceDictionary.MergedDictionaries>):
            IEnumerable<XElement> resources = from elt in childrenOfResourceDictionary
                                              where !elt.Name.LocalName.Contains(".")
                                              select elt;

            return resources;
        }
         */

        public static void ResolveAndMergeTheMergedDictionaries(XElement element, string currentXamlFileNameAndPath, Dictionary<string, Project> assemblyNameToProjectDictionary, HashSet<string> allXamlFilesVisitedDuringRecursion, ResourcesCache resourcesCache) // "allXamlFilesVisitedDuringRecursion" is used to prevent infinite execution during the recursion due to circular references.
        {
            // First, let us put the <ResourceDictionary> nodes into a List<> so that we can modify them during the foreach without causing issues with the iterator on a collection being modified:
            List<XElement> resourceDictionaryNodes = element.Descendants(DefaultXamlNamespace + "ResourceDictionary").ToList();

            // Then, iterate through all the <ResourceDictionary> nodes and "in-place expand" the ones that reference another file:
            foreach (XElement resourceDictionaryNode in resourceDictionaryNodes)
            {
                var sourceAttribute = resourceDictionaryNode.Attributes("Source").FirstOrDefault();
                if (sourceAttribute != null)
                {
                    string uri = sourceAttribute.Value.ToString();

                    // Get the full referenced ResourceDictionary (or read from cache if any):
                    string referencedResourceDictionaryFullPath;
                    XElement resourceDictionary = GetResourceDictionary(uri, currentXamlFileNameAndPath, assemblyNameToProjectDictionary, resourcesCache, out referencedResourceDictionaryFullPath);

                    // Change all the the children "x:Name" into "x:Key" so that we do not get an error if there are two entries with the same "x:Name" in the whole document:
                    ChangeAllXNameIntoXKey(resourceDictionary);

                    // Apply the recursion to resolve the nested resource dictionary references:
                    if (!allXamlFilesVisitedDuringRecursion.Contains(referencedResourceDictionaryFullPath))
                    {
                        allXamlFilesVisitedDuringRecursion.Add(referencedResourceDictionaryFullPath);
                        ResolveAndMergeTheMergedDictionaries(resourceDictionary, referencedResourceDictionaryFullPath, assemblyNameToProjectDictionary, allXamlFilesVisitedDuringRecursion, resourcesCache);
                    }
                    else
                        throw new Exception("Circular references in ResourceDictionary files has been detected (file name: " + referencedResourceDictionaryFullPath + ").");

                    // Replace the current node with the full ResourceDictionary:
                    resourceDictionaryNode.ReplaceWith(resourceDictionary);
                }
            }
        }

        static void ChangeAllXNameIntoXKey(XElement parent)
        {
            // This method will traverse all the children and change all the the children "x:Name" into "x:Key" so that we do not get an error if there are two entries with the same "x:Name" in the whole document.
            //todo: should we also change "x:Name" into "x:Key" in nodes that are deeper inside the XAML? (such as nodes defined in resource dictionaries of controls inside the XAML) HOWEVER, if we do so, we must ONLY change "x:Name" into "x:Key" inside resource dictionaries and NOT change other element names, because those can be used for bindings (such as {Binding ElementName=...})

            foreach (XElement child in parent.Elements())
            {
                var xNameAttribute = child.Attribute(xNamespace + "Name");
                if (xNameAttribute != null)
                {
                    // Read the "x:Name" before removing it:
                    string xNameValue = xNameAttribute.Value;

                    // Remove the "x:Name":
                    xNameAttribute.Remove();

                    // Make sure an "x:Key" attribute does not already exist:
                    var xKeyAttribute = child.Attribute(xNamespace + "Key");
                    if (xKeyAttribute == null)
                    {
                        // Add "x:Key":
                        child.Add(new XAttribute(xNamespace + "Key", xNameValue));
                    }
                }
            }
        }

        static XElement GetResourceDictionary(string uri, string currentXamlFileNameAndPath, Dictionary<string, Project> assemblyNameToProjectDictionary, ResourcesCache resourcesCache, out string resourceDictionaryFullPath)
        {
            // Determine the file absolute path of the file (it includes the file name and path):
            resourceDictionaryFullPath = GetAbsolutePath(uri, currentXamlFileNameAndPath, assemblyNameToProjectDictionary);

            // Read from cache if any (this avoids reloading the files at every refresh, to improve performance:
            XElement resourceDictionaryContent;
            if (resourcesCache.ResourceDictionaryFileNameToContent.ContainsKey(resourceDictionaryFullPath))
            {
                resourceDictionaryContent = resourcesCache.ResourceDictionaryFileNameToContent[resourceDictionaryFullPath];
            }
            else
            {
                //------------------------
                // Not found in the cache
                //------------------------

                // Get the file content:
                string resourceDictionaryXaml = File.ReadAllText(resourceDictionaryFullPath);

                // Remove the content of all the "HtmlPresenter" nodes, because the content may be not well formatted and may lead to a syntax error when parsing the XDocument:
                resourceDictionaryXaml = HtmlPresenterRemover.RemoveHtmlPresenterNodes(resourceDictionaryXaml);

                // Load the file as XML:
                XDocument xdoc = XDocument.Parse(resourceDictionaryXaml);

                // Get the content:
                resourceDictionaryContent = xdoc.Root;

                // Add to cache:
                resourcesCache.ResourceDictionaryFileNameToContent.Add(resourceDictionaryFullPath, resourceDictionaryContent);
            }

            return resourceDictionaryContent;
        }

        static string GetAbsolutePath(string uri, string currentXamlFileNameAndPath, Dictionary<string, Project> assemblyNameToProjectDictionary)
        {
            // Split the URI (like /AssemblyName;component/Folder/FileName.xaml") into its parts:
            string assemblyName;
            string folderAndFileName;
            bool isPathAbsolute;
            GetInformationFromUri(uri, out assemblyName, out folderAndFileName, out isPathAbsolute);

            // Determine the absolute path:
            string finalPath;
            if (isPathAbsolute)
            {
                //---------------------
                // Absolute URI
                //---------------------

                if (assemblyName != null)
                {
                    if (assemblyNameToProjectDictionary.ContainsKey(assemblyName))
                    {
                        // Get the project associated to the Assembly Name:
                        Project project = assemblyNameToProjectDictionary[assemblyName];

                        // Get the project folder:
                        string directoryAbsolutePath = Path.GetDirectoryName(project.FullName).TrimEnd('\\') + "\\";

                        // Merge the path:
                        finalPath = Path.Combine(directoryAbsolutePath, folderAndFileName);
                    }
                    else
                        throw new Exception("The assembly name '" + assemblyName + "' specified in the URI '" + uri + "' was not found.");
                }
                else
                    throw new Exception("The following absolute path does not specify an Assembly Name: " + uri);
            }
            else
            {
                //---------------------
                // Relative URI
                //---------------------

                // Get the folder where the current XAML file is located:
                string directoryAbsolutePath = Path.GetDirectoryName(currentXamlFileNameAndPath).TrimEnd('\\') + "\\";

                // Merge the path:
                finalPath = Path.Combine(directoryAbsolutePath, folderAndFileName);
            }

            return finalPath;
        }

        static void GetInformationFromUri(string uri, out string assemblyName, out string folderAndFileName, out bool isPathAbsolute)
        {
            if (uri.ToLower().StartsWith(@"ms-appx:/"))
            {
                //----------------
                // This is the WinRT/UWP syntax for files in the app package.
                //----------------

                // Remove "ms-appx:/", "ms-appx://", and "ms-appx:///":
                uri = uri.Replace("://", ":/").Replace("://", ":/").Replace("://", ":/").Substring(9); // Note: We use "Substring" instead of "String.Replace" to remove "ms-appx:/" in order to support any case (lowercase/uppercase).

                // Fix slashes:
                uri = uri.Replace('\\', '/');

                // Get the assembly name:
                int firstSlashIndex = uri.IndexOf('/');
                if (firstSlashIndex != -1)
                {
                    assemblyName = uri.Substring(0, firstSlashIndex);
                    folderAndFileName = uri.Substring(firstSlashIndex + 1);
                }
                else
                {
                    assemblyName = null;
                    folderAndFileName = uri;
                }

                isPathAbsolute = true;
            }
            else if (uri.ToLower().Contains(@";component/"))
            {
                //----------------
                // This is the Silverlight/WPF syntax for files in the app package.
                //----------------

                string componentKeyword = @";component/";
                int indexOfComponentKeyword = uri.IndexOf(componentKeyword);
                assemblyName = uri.Substring(0, indexOfComponentKeyword);
                assemblyName = assemblyName.Trim('/', '\\');
                folderAndFileName = uri.Substring(indexOfComponentKeyword + componentKeyword.Length);
                folderAndFileName = folderAndFileName.TrimStart('/', '\\');
                isPathAbsolute = true;
            }
            else
            {
                //----------------
                // Relative path
                //----------------

                assemblyName = null;
                folderAndFileName = uri;
                isPathAbsolute = false;
            }
        }
    }
}
