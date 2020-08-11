

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
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class AssemblyMissingReferenceChecker : Task // AppDomainIsolatedTask
    {
        public Microsoft.Build.Framework.ITaskItem[] References { get; set; }

        public string RequiredAssemblies { get; set; }

        public override bool Execute()
        {
            if (!string.IsNullOrEmpty(RequiredAssemblies))
            {
                ILogger logger = new LoggerThatUsesTaskOutput(this);
                string operationName = "C#/XAML for HTML5: AssemblyMissingReferenceChecker";
                try
                {
                    return CheckingThatNoAssemblyReferenceIsMissing.Check(References, RequiredAssemblies, logger);
                }
                catch (Exception ex)
                {
                    logger.WriteError(operationName + " failed: " + ex.ToString());
                    return false;
                }
            }
            else
                return true;
        }

    }
}
