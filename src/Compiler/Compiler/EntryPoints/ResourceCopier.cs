

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
    //--------------------------------------------------------
    // This class was commented because it is obsolete.
    // It has been replaced with the class "ExtractingAndCopyingResources".
    //--------------------------------------------------------

    /*
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class ResourceCopier : Task // AppDomainIsolatedTask
    {
        public const string NAME_OF_FOLDER_THAT_CONTAINS_RESOURCES = "Resources";

        public string Resource { get; set; } //todo: rename as "Resources" (plural)?

        public string OutputDirectory { get; set; }

        public override bool Execute()
        {
            if (!string.IsNullOrEmpty(Resource) && !string.IsNullOrEmpty(OutputDirectory))
                return Execute(Resource, OutputDirectory, new LoggerThatUsesTaskOutput(this));
            else
                return true;
        }

        public static bool Execute(string resourcesSeparatedBySemiColons, string outputDirectory, ILogger logger)
        {
            string operationName = "C#/XAML for HTML5: ResourceCopier";
            try
            {
                string[] resourcesAsArray = resourcesSeparatedBySemiColons.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string resource in resourcesAsArray)
                {
                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage("Copying resource: " + resource);

                    // Check that source file was found:
                    if (!File.Exists(resource))
                        throw new Exception("File not found: " + resource);

                    // Create destination folders hierarchy:
                    string destinationFile = Path.Combine(outputDirectory, NAME_OF_FOLDER_THAT_CONTAINS_RESOURCES + "\\", resource);
                    string destinationDirectory = Path.GetDirectoryName(destinationFile);
                    Directory.CreateDirectory(destinationDirectory);

                    // Copy the file:
                    File.Copy(resource, destinationFile);
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
        }
    }
     */
}
