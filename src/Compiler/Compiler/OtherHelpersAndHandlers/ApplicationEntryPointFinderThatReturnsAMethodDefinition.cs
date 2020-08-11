

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



#if !BRIDGE && !CSHTML5BLAZOR
extern alias DotNetForHtml5Core;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class ApplicationEntryPointFinderThatReturnsAMethodDefinition
    {
        public static void GetEntryPoint(string pathOfAssemblyThatContainsEntryPoint, List<Mono.Cecil.AssemblyDefinition> assemblyDefinitions, out Mono.Cecil.MethodDefinition outputMethodDefinition)
        {
            // Get the assembly that is supposed to contain the entry point, ie. a class that inherits from "Application":
            string applicationClassFullName;
            string assemblyName;
            string assemblyFullName;
            ApplicationEntryPointFinder.GetFullNameOfClassThatInheritsFromApplication(pathOfAssemblyThatContainsEntryPoint, out applicationClassFullName, out assemblyName, out assemblyFullName);

            // Then, find the constructor of that EntryPoint in the AssemblyDefinitions:
            outputMethodDefinition = null;
            foreach (Mono.Cecil.AssemblyDefinition assemblyDefinition in assemblyDefinitions)
            {
                if (assemblyDefinition.FullName == assemblyFullName)
                {
                    foreach (Mono.Cecil.ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
                    {
                        foreach (Mono.Cecil.TypeDefinition typeDefinition in moduleDefinition.Types)
                        {
                            if (typeDefinition.FullName == applicationClassFullName)
                            {
                                foreach (Mono.Cecil.MethodDefinition methodDefinition in typeDefinition.Methods)
                                {
                                    if (methodDefinition.IsConstructor)
                                    {
                                        outputMethodDefinition = methodDefinition;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
