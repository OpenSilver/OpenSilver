using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    internal static class CheckingThatNoAssemblyReferenceIsMissing
    {
        public static bool Check(Microsoft.Build.Framework.ITaskItem[] references, string requiredAssembliesAsString, ILogger logger)
        {
            string[] requiredAssemblies = requiredAssembliesAsString != null ? requiredAssembliesAsString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };

            List<string> requiredAssembliesThatAreNotReferenced = new List<string>();

            foreach (string requiredAssembly in requiredAssemblies)
            {
                string assemblyFullPath = Path.GetFullPath(requiredAssembly);
                string requiredAssemblyNameLowercase = GetAssemblyNameFromFileNameWithPath(assemblyFullPath).ToLower();
                bool found = false;
                foreach (Microsoft.Build.Framework.ITaskItem taskItem in (references ?? new Microsoft.Build.Framework.ITaskItem[] { }))
                {
                    if (GetAssemblyNameFromIdentity(taskItem.GetMetadata("identity")).ToLower() == requiredAssemblyNameLowercase) //Note: "GetMetadata" and "GetAssemblyName" never return null.
                        found = true;
                }
                if (!found)
                    requiredAssembliesThatAreNotReferenced.Add(File.Exists(assemblyFullPath) ? assemblyFullPath : requiredAssembly);
            }

            if (requiredAssembliesThatAreNotReferenced.Count > 0)
            {
                logger.WriteError(string.Format("Please add a reference to the following assembl{0}: {1}", requiredAssembliesThatAreNotReferenced.Count > 1 ? "ies" : "y", string.Join(", ", requiredAssembliesThatAreNotReferenced)));
                return false;
            }
            else
                return true;
        }

        static string GetAssemblyNameFromIdentity(string assemblyIdentity)
        {
            // Never returns null
            if (assemblyIdentity != null)
                return assemblyIdentity.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
            else
                return string.Empty;
        }

        static string GetAssemblyNameFromFileNameWithPath(string assemblyFileNameWithPath)
        {
            return Path.GetFileNameWithoutExtension(assemblyFileNameWithPath);
        }
    }

}
