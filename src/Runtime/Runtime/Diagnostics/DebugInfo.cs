
/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenSilver.Diagnostics
{
    public class DebugInfo
    {
        private static string GetAssemblyBuildInfo()
        {
            var listOfAttributeKeys = new HashSet<string> { "SourceRevisionId", "BuildDate", "MachineName" };

            var sb = new StringBuilder();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var attributes = Attribute.GetCustomAttributes(assembly, typeof(AssemblyMetadataAttribute))
                    .OfType<AssemblyMetadataAttribute>().Where(a => listOfAttributeKeys.Contains(a.Key)).ToList();
                if (!attributes.Any())
                {
                    continue;
                }

                sb.AppendLine(assembly.FullName + ":");

                foreach (var att in attributes)
                {
                    sb.AppendLine(att.Key + " - " + att.Value);
                }
            }

            return sb.ToString();
        }

        [Microsoft.JSInterop.JSInvokable]
        public static void PrintDebugInfo()
        {
            var buildInfo = GetAssemblyBuildInfo();
            Console.WriteLine(string.IsNullOrEmpty(buildInfo) ? "Can not find any info" : buildInfo);
        }
    }
}
