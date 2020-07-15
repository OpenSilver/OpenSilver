using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner
{
    internal static class SolutionProjectsHelper
    {
        public static IEnumerable<Project> GetAllProjectsInSolution(Solution solution)
        {
            foreach (Project project in solution.Projects)
            {
                if (project != null)
                {
                    if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        // Recursion:
                        foreach (var projectInSubfolder in GetProjectsInSolutionFolder(project))
                        {
                            yield return projectInSubfolder;
                        }
                    }
                    else
                    {
                        yield return project;
                    }
                }
            }
        }

        static IEnumerable<Project> GetProjectsInSolutionFolder(Project solutionFolder)
        {
            foreach (ProjectItem projectItem in solutionFolder.ProjectItems)
            {
                if (projectItem != null && projectItem.SubProject != null)
                {
                    var subProject = projectItem.SubProject;
                    if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        // Recursion:
                        foreach (Project projectInSubfolder in GetProjectsInSolutionFolder(subProject))
                        {
                            yield return projectInSubfolder;
                        }
                    }
                    else
                    {
                        yield return subProject;
                    }
                }
            }
        }

        public static bool DoesProjectAReferenceProjectB(Project projectA, Project projectB)
        {
            foreach (Project referencedProject in GetReferencedProjets(projectA))
            {
                if (referencedProject.UniqueName == projectB.UniqueName)
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<Project> GetReferencedProjets(Project project, bool includeTheProjectItselfAmondTheResults = false)
        {
            if (project.Object is VSLangProj.VSProject)
            {
                VSLangProj.VSProject vsproject = (VSLangProj.VSProject)project.Object;

                foreach (VSLangProj.Reference reference in vsproject.References)
                {
                    Project referencedProject = reference.SourceProject;
                    if (referencedProject != null)
                    {
                        yield return referencedProject;
                    }
                }
            }
            if (includeTheProjectItselfAmondTheResults)
            {
                yield return project;
            }

        }
    }
}
