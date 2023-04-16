
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
using System.Globalization;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using OpenSilver.Compiler.Common;
using ILogger = OpenSilver.Compiler.Common.ILogger;

namespace OpenSilver.Compiler
{
    public class AfterXamlPreprocessor : Task
    {
        [Required]
        public bool IsSecondPass { get; set; }

        [Required]
        public string Flags { get; set; }

        public override bool Execute()
        {
            return ExecuteImpl(IsSecondPass, Flags, new LoggerThatUsesTaskOutput(this));
        }

        private static bool ExecuteImpl(bool isSecondPass, string flagsString, ILogger logger)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string passNumber = (isSecondPass ? "2" : "1");
            string operationName = string.Format("C#/XAML for HTML5: AfterXamlPreprocessor (pass {0})", passNumber);
            try
            {
                using (var executionTimeMeasuring = new ExecutionTimeMeasuring())
                {
                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + " started.");

                    //-----------------------------------------------------
                    // Note: we dispose the static instance of the "ReflectionOnSeparateAppDomainHandler" that was created in the "BeforeXamlPreprocessor" task.
                    // Disposing it allows to free any hooks on the user app DLL's.
                    //-----------------------------------------------------

                    // Dispose the static instance of the "ReflectionOnSeparateAppDomainHandler":
                    AssembliesInspector.Current.Dispose(); // Note: this is not supposed to be null because it was instantiated in the "BeforeXamlPreprocessor" task.


                    bool isSuccess = true;

                    //------- DISPLAY THE PROGRESS -------
                    logger.WriteMessage(operationName + (isSuccess ? " completed in " + executionTimeMeasuring.StopAndGetTimeInSeconds() + " seconds." : " failed."));

                    return isSuccess;
                }
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.ToString());
                return false;
            }
        }
    }
}
