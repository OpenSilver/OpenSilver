

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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;

namespace DotNetForHtml5.Compiler
{
    public class ResXProcessor : Task
    {
        [Required]
        public string SourceFile { get; set; }

        [Required]
        public string OutputFile { get; set; }

        [Required]
        public string RootNamespace { get; set; }

        [Required]
        public string SourceFileRelativePath { get; set; }


        public override bool Execute()
        {
            return Execute(SourceFile, OutputFile, SourceFileRelativePath, RootNamespace, new LoggerThatUsesTaskOutput(this));
        }


        public static bool Execute(string sourceFile, string outputFile, string sourceFileRelativePath, string rootNamespace, ILogger logger)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string operationName = "C#/XAML for HTML5: ResXProcessor";
            Console.WriteLine(operationName + " started.");
            logger.WriteMessage(operationName + " started.");
            try
            {
                // Validate input strings:
                if (string.IsNullOrEmpty(sourceFile))
                    throw new Exception(operationName + " failed because the source file argument is invalid.");
                if (string.IsNullOrEmpty(outputFile))
                    throw new Exception(operationName + " failed because the output file argument is invalid.");

                //------- DISPLAY THE PROGRESS -------
                logger.WriteMessage(operationName + " started for file \"" + sourceFile + "\". Output file: \"" + outputFile + "\"");
                //todo: do not display the output file location?


                // Read file:
                using (StreamReader sr = new StreamReader(sourceFile))
                {
                    String sourceCodeForResourcesAsJson = ConvertResources2(sr.BaseStream, sourceFile);

                    //generate the code to allow access to the resources:
                    string pathToResourceInJavaScript = "document.ResXFiles." + rootNamespace + "." + sourceFileRelativePath;
                    pathToResourceInJavaScript = pathToResourceInJavaScript.Replace('\\', '.').Replace('/', '.').Replace(' ', '_'); //todo: add other forbidden characters replacements?
                    string codeToInitializeThePath = GenerateCodeToInitializeThePath(pathToResourceInJavaScript);
                    string finalSourceCode = codeToInitializeThePath + Environment.NewLine + string.Format(@"{0} = {1};", pathToResourceInJavaScript, sourceCodeForResourcesAsJson);

                    // Create output directory:
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

                    // Save output:
                    using (StreamWriter outfile = new StreamWriter(outputFile))
                    {
                        outfile.Write(finalSourceCode);
                    }
                }

                //------- DISPLAY THE PROGRESS -------
                logger.WriteMessage(operationName + " completed.");

                return true;
            }
            catch (Exception ex)
            {
                logger.WriteError(operationName + " failed: " + ex.Message, file: sourceFile);
                return false;
            }
        }

        static string GenerateCodeToInitializeThePath(string pathToResourceInJavaScript)
        {
            string[] splittedPath = pathToResourceInJavaScript.Split('.');

            StringBuilder stringBuilder = new StringBuilder();

            string pathSoFar = "";

            for (int i = 0; i < splittedPath.Length; ++i)
            {
                string part = splittedPath[i];

                if (i > 0)
                {
                    stringBuilder.AppendLine(string.Format(@"{0}.{1} = {0}.{1} || {{}};", pathSoFar, part));
                }

                if (i == 0)
                    pathSoFar = part;
                else
                    pathSoFar = pathSoFar + "." + part;
            }

            return stringBuilder.ToString();
        }

        static string ConvertResources2(Stream resourceStream, string fileName)
        {
            //-------------------------
            // Original Credits: JSIL.org ("ResourceConverter.cs")
            //-------------------------

            var output = new StringBuilder();

            using (var reader = new ResXResourceReader(resourceStream))
            {
                output.AppendLine("{");

                bool first = true;

                var e = reader.GetEnumerator();
                while (e.MoveNext())
                {
                    if (!first)
                        output.AppendLine(",");
                    else
                        first = false;

                    var key = Convert.ToString(e.Key);
                    output.AppendFormat("    {0}: ", EscapeString(key, forJson: true));

                    var value = e.Value;

                    if (value == null)
                    {
                        output.Append("null");
                    }
                    else
                    {
                        switch (value.GetType().FullName)
                        {
                            case "System.String":
                                output.Append(EscapeString((string)value, forJson: true));
                                break;
                            case "System.Single":
                            case "System.Double":
                            case "System.UInt16":
                            case "System.UInt32":
                            case "System.UInt64":
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                                output.Append(Convert.ToString(value));
                                break;
                            default:
                                output.Append(EscapeString(Convert.ToString(value), forJson: true));
                                break;
                        }
                    }
                }

                output.AppendLine();
                output.AppendLine("}");
            }

            return output.ToString();
        }

        public static string EscapeString(string text, char quoteCharacter = '\"', bool forJson = false)
        {
            if (text == null)
                return "null";

            var sb = new StringBuilder();

            sb.Clear();
            sb.Append(quoteCharacter);

            foreach (var ch in text)
            {
                if (ch == quoteCharacter)
                    sb.Append(EscapeCharacter(ch, forJson));
                else if (ch == '\\')
                    sb.Append(@"\\");
                else if ((ch < ' ') || (ch > 127))
                    sb.Append(EscapeCharacter(ch, forJson));
                else
                    sb.Append(ch);
            }

            sb.Append(quoteCharacter);

            return sb.ToString();
        }

        public static string EscapeCharacter(char character, bool forJson)
        {
            switch (character)
            {
                case '\'':
                    return @"\'";
                case '\\':
                    return @"\\";
                case '"':
                    return "\\\"";
                case '\t':
                    return @"\t";
                case '\r':
                    return @"\r";
                case '\n':
                    return @"\n";
                default:
                    {
                        if (forJson || (character > 255))
                            return String.Format(@"\u{0:x4}", (int)character);
                        else
                            return String.Format(@"\x{0:x2}", (int)character);
                    }
            }
        }

    }
}
