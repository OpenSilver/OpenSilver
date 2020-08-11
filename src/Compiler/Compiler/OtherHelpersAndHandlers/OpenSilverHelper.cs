

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



using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler.OtherHelpersAndHandlers
{
    public static class OpenSilverHelper
    {
        public static string ReferencePathsString(ITaskItem[] references)
        {
            if (references == null)
            {
                return string.Empty;
            }
            string referencePathsString = string.Empty;

            foreach (Microsoft.Build.Framework.ITaskItem taskItem in references)
            {
                string referenceFilePath = taskItem.GetMetadata("HintPath");

                if (referenceFilePath == string.Empty) // if task has not an hint path it's a user/package assembly
                {
                    referenceFilePath = taskItem.GetMetadata("identity");
                }

                referencePathsString += referenceFilePath + ";";

                string pdbFile = Path.ChangeExtension(referenceFilePath, ".pdb");
                if (File.Exists(pdbFile)) // if the .pdb exists add it

                    referencePathsString += pdbFile + ";";
                string xmlFile = Path.ChangeExtension(referenceFilePath, ".xml");
                if (File.Exists(xmlFile)) // if the .xml exists add it
                    referencePathsString += xmlFile + ";";
            }
            referencePathsString = referencePathsString.Substring(0, referencePathsString.Length - 1);

            return referencePathsString;
        }
    }
}
