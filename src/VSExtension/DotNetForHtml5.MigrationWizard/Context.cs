using EnvDTE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.MigrationWizard
{
    public class Context
    {
        public Context(Solution solution)
        {
            this.Solution = solution;
            this.AllProjects = new List<ProjectModel>();
            this.CandidateProjectsToMigrate = new ObservableCollection<ProjectModel>();
        }

        public Solution Solution { get; private set; }

        internal CopyOrAddAsLink HowToDealWithCSharpFiles { get; set; }

        internal List<ProjectModel> AllProjects { get; set; }

        internal ObservableCollection<ProjectModel> CandidateProjectsToMigrate { get; set; }

        internal List<ProjectModel> ProjectsToMigrate { get; set; }

        internal void RaiseMigrationCompletedEvent()
        {
            if (MigrationCompleted != null)
                MigrationCompleted(this, new EventArgs());
        }

        internal void RaiseMigrationStartedEvent()
        {
            if (MigrationStarted != null)
                MigrationStarted(this, new EventArgs());
        }

        internal event EventHandler MigrationCompleted;
        internal event EventHandler MigrationStarted;

        public class ProjectModel
        {
            public string Name { get; set; }
            public string FullPathOfCSProj { get; set; }
            public bool IsSelected { get; set; }

            public string DestinationName { get; set; }
            public string DestinationProjectType { get; set; }

            public ProjectType ProjectType { get; set; }
            public ProjectOutputType ProjectOutputType { get; set; }

            public Project ProjectLoadedWithEnvDTE { get; set; } // This is for projects that are currently loaded in the solution that is open in VS.
            public Microsoft.Build.Evaluation.Project ProjectLoadedWithMsBuild { get; set; } // This is for projects that have been selected via the "Browse..." button.

            public FileTreeWalker FileWalkerInCaseOfProjectLoadedWithMsBuild { get; set; }
        }

        public enum ProjectType
        {
            Other, Silverlight, WPF
        }

        public enum ProjectOutputType
        {
            Unknown, Application, Library
        }

        public enum CopyOrAddAsLink
        {
            Copy, AddAsLink
        }
    }
}

