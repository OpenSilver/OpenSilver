extern alias wpf;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DotNetForHtml5.Compiler
{
    //[LoadInSeparateAppDomain]
    //[Serializable]
    public class AfterXamlPreprocessor : Task // AppDomainIsolatedTask
    {
        [Required]
        public bool IsSecondPass { get; set; }

        [Required]
        public string Flags { get; set; }

        public override bool Execute()
        {
            return Execute(IsSecondPass, Flags, new LoggerThatUsesTaskOutput(this));
        }

        public static bool Execute(bool isSecondPass, string flagsString, ILogger logger)
        {
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
                    ReflectionOnSeparateAppDomainHandler.Current.Dispose(); // Note: this is not supposed to be null because it was instantiated in the "BeforeXamlPreprocessor" task.


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
