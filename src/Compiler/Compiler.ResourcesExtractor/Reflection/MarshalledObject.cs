
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenSilver.Compiler.Common;

namespace OpenSilver.Compiler.Resources
{
    public class MarshalledObject : MarshalledObjectBase, IMarshalledObject
    {
        private const string ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES = "The specified assembly is not in the list of loaded assemblies.";

        public Dictionary<string, byte[]> GetManifestResources(string assemblySimpleName, HashSet<string> supportedExtensionsLowerCase)
        {
            if (TryGetAssembly(assemblySimpleName, out Assembly assembly))
            {
                var manifestResourceNames = assembly.GetManifestResourceNames();
                var resourceFiles = (from fn in manifestResourceNames where supportedExtensionsLowerCase.Contains(GetExtension(fn.ToLower())) select fn).ToArray();
                var result = new Dictionary<string, byte[]>();

                foreach (var resourceFile in resourceFiles)
                {
                    var stream = assembly.GetManifestResourceStream(resourceFile);
                    if (stream == null)
                        throw new FileNotFoundException("No manifest resource stream named " + resourceFile);

                    using (stream)
                    {
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        result[resourceFile] = buffer;
                    }
                }

                return result;
            }
            else
            {
                throw new Exception(ASSEMBLY_NOT_IN_LIST_OF_LOADED_ASSEMBLIES);
            }
        }

        private static string GetExtension(string str)
        {
            try
            {
                return Path.GetExtension(str);
            }
            catch
            {
                //It is possible that resource does not have an extension
                return null;
            }
        }
    }
}
