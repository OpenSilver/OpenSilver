#region copyright
// Copyright (c) rubicon IT GmbH

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
#endregion

using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;

namespace DotNetForHtml5.MigrationWizard.Utils
{
    static class AllSolutionProjectsSearcher
    {
        public static List<Project> GetAllProjects(Solution solution)
        {
            List<Project> projectList = new List<Project>();
            PopulateProjectsList(solution, projectList);

            return projectList;
        }

        private static void PopulateProjectsList(Solution solution, List<Project> projectList)
        {
            foreach (Project project in solution)
            {
                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    projectList.AddRange(GetSolutionFolderProjects(project));
                else if (IsLoaded(project))
                    projectList.Add(project);
            }
        }

        private static bool IsLoaded(Project project)
        {
            return string.Compare(EnvDTE.Constants.vsProjectKindUnmodeled, project.Kind, StringComparison.OrdinalIgnoreCase) != 0;
        }

        private static IEnumerable<Project> GetSolutionFolderProjects(Project project)
        {
            List<Project> list = new List<Project>();
            for (var i = 1; i <= project.ProjectItems.Count; i++)
            {
                var subProject = project.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    list.AddRange(GetSolutionFolderProjects(subProject));
                }
                else if (IsLoaded(project))
                {
                    list.Add(subProject);
                }
            }
            return list;
        }
    }
}
