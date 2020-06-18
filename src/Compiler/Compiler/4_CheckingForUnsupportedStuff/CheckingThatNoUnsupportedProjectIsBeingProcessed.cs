#if !BRIDGE && !CSHTML5BLAZOR
extern alias DotNetForHtml5Core;
#endif
using JSIL.Proxy;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    public class CheckingThatNoUnsupportedProjectIsBeingProcessed
    {
        internal void Check(AssemblyDefinition[] assembliesDefinitions, string nameOfAssembliesThatDoNotContainUserCode, Action<string> whatToDoWhenNotSupportedMethodFound) // Explanation of the "Action<string>" parameters: the string is the error explanation to display in the output of VS ("explanationToDisplayInErrorsWindow").
        {
            HashSet<string> errorsAlreadyRaised = new HashSet<string>(); // This prevents raising multiple times the same error.
            HashSet<string> activatedFeatures = new HashSet<string>(); // Temporary cache for performance in the loop.

            foreach (AssemblyDefinition userAssembly in GetAllUserAssemblies(assembliesDefinitions, nameOfAssembliesThatDoNotContainUserCode))
            {
                bool wasCompiledWithCSharpXamlForHtml5 = false;
                if (userAssembly.HasCustomAttributes)
                {
                    foreach (var customAttribute in userAssembly.CustomAttributes)
                    {
#if BRIDGE || CSHTML5BLAZOR
                        throw new NotSupportedException();
#else
                        if (customAttribute.AttributeType.Name == typeof(DotNetForHtml5Core::CompilerVersionFriendlyNameAttribute).Name
                            || customAttribute.AttributeType.Name == typeof(DotNetForHtml5Core::CompilerVersionNumberAttribute).Name)
                        {
                            wasCompiledWithCSharpXamlForHtml5 = true;
                            break;
                        }
#endif
                    }
                }

                if (!wasCompiledWithCSharpXamlForHtml5)
                {
                    whatToDoWhenNotSupportedMethodFound(string.Format("The project '{0}' does not appear to be of type C#/XAML for HTML5. A C#/XAML for HTML5 project can only reference other C#/XAML for HTML5 projects. To fix the issue, please remove the reference to the project '{0}'.", userAssembly.Name.Name));
                }
            }
        }

        static IEnumerable<AssemblyDefinition> GetAllUserAssemblies(AssemblyDefinition[] assembliesDefinitions, string nameOfAssembliesThatDoNotContainUserCode)
        {
            HashSet<string> listOfNamesOfAssembliesThatDoNotContainUserCode = new HashSet<string>();
            if (nameOfAssembliesThatDoNotContainUserCode != null)
            {
                string[] array = nameOfAssembliesThatDoNotContainUserCode.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in array)
                {
                    listOfNamesOfAssembliesThatDoNotContainUserCode.Add(item.ToLower());
                }
            }

            foreach (AssemblyDefinition assemblyDefinition in assembliesDefinitions)
            {
                // Retain only user assemblies:
                if (!listOfNamesOfAssembliesThatDoNotContainUserCode.Contains(assemblyDefinition.Name.Name.ToLower()))
                {
                    yield return assemblyDefinition;
                }
            }
        }
    }
}
