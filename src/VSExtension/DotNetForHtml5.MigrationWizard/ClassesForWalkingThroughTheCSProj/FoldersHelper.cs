using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.MigrationWizard
{
    internal static class FoldersHelper
    {
        public static bool IsAFolder(string fullPath)
        {
            return fullPath.EndsWith(@"\");
        }

        public static bool IsAFolder(EnvDTE.ProjectItem projectItem)
        {
            string fullPath = projectItem.Properties.Item("FullPath").Value.ToString();
            return IsAFolder(fullPath);
        }
    }
}
