

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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class XamlFilesWithoutCodeBehindHelper
    {
        internal static string GenerateClassNameFromAssemblyAndPath(string fileNameWithPathRelativeToProjectRoot, string assemblyNameWithoutExtension)
        {
            // This method is used to convert the XAML files that have no code-behind into a C# class. The name of that C# class is what this method generates.

            return GenerateClassName("/" + assemblyNameWithoutExtension + ";component/" + fileNameWithPathRelativeToProjectRoot);
        }

        internal static string GenerateClassNameFromAbsoluteUri(string absoluteUri)
        {
            // This method is used to convert the XAML files that have no code-behind into a C# class. The name of that C# class is what this method generates.

            return GenerateClassName(absoluteUri);
        }

        private static string GenerateClassName(string value)
        {
            // CREDITS: http://stackoverflow.com/questions/25139734/how-can-i-generate-a-safe-class-name-from-a-file-name

            // Note: an interesting alternative is available at: http://stackoverflow.com/questions/29303672/are-there-methods-to-convert-between-any-string-and-a-valid-variable-name-in-c-s

            // Convert to TitleCase (so that when we remove the spaces, it is easily readable):
            string className = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);

            // If file name contains invalid chars, remove them:
            Regex regex = new Regex(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]");
            className = regex.Replace(className, "__");

            // If class name doesn't begin with a letter, insert an underscore:
            if (!char.IsLetter(className, 0))
            {
                className = className.Insert(0, "_");
            }

            // Remove white space:
            className = className.Replace(" ", string.Empty);

            return className;

            //todo: ensure that 2 different file names cannot end up with the same class name.
            //todo: throw exception is invalid input (such as empty string).
        }

        /// <summary>
        /// Generates a class name from the uri for the class that will allow us to instantiate the Page defined in the file at the given Uri.
        /// This class is used in System.Windows.Controls.Frame to create an instance of the page (for example when using MyFrame.Source = new Uri("/InnerPage.xaml", UriKind.Relative);).
        /// </summary>
        /// <param name="absoluteSourceUri">The Uri from which to generate the class name.</param>
        /// <returns>The newly generated class name.</returns>
        internal static string GenerateClassFactoryNameFromAbsoluteUri_ForRuntimeAccess(string absoluteSourceUri)
        {
            // Convert to TitleCase (so that when we remove the spaces, it is easily readable):
            string className = MakeTitleCase(absoluteSourceUri);

            // If file name contains invalid chars, remove them:
            className = Regex.Replace(className, @"\W", "ǀǀ"); //Note: this is not a pipe (the thing we get with ctrl+alt+6), it is U+01C0

            // If class name doesn't begin with a letter, insert an underscore:
            if (char.IsDigit(className, 0))
            {
                className = className.Insert(0, "_");
            }

            // Remove white space:
            className = className.Replace(" ", string.Empty);

            className += "ǀǀFactory"; //Note: this is not a pipe (the thing we get with ctrl+alt+6), it is U+01C0

            return className;
        }

        static string MakeTitleCase(string str)
        {
            string result = "";
            string lowerStr = str.ToLower();
            int length = str.Length;
            bool makeUpper = true;
            int lastCopiedIndex = -1;
            //****************************
            //HOW THIS WORKS:
            //
            //  We go through all the characters of the string.
            //  If any is not an alphanumerical character, we make the next alphanumerical character uppercase.
            //  To do so, we copy the string (on which we call toLower) bit by bit into a new variable,
            //  each bit being the part between two uppercase characters, and while inserting the uppercase version of the character between each bit.
            //  then we add the end of the string.
            //****************************

            for (int i = 0; i < length; ++i)
            {
                char ch = lowerStr[i];
                if (ch >= 'a' && ch <= 'z' || ch >= '0' && ch <= '0')
                {
                    if (makeUpper && ch >= 'a' && ch <= 'z') //if we have a letter, we make it uppercase. otherwise, it is a number so we let it as is.
                    {
                        if (!(lastCopiedIndex == -1 && i == 0)) //except this very specific case, we should never have makeUpper at true while i = lastCopiedindex + 1 (since we made lowerStr[lastCopiedindex] into an uppercase letter.
                        {
                            result += lowerStr.Substring(lastCopiedIndex + 1, i - lastCopiedIndex - 1); //i - lastCopied - 1 because we do not want to copy the current index since we want to make it uppercase:
                        }
                        result += (char)(ch - 32); //32 is the difference between the lower case and the upper case, meaning that (char)('a' - 32) --> 'A'.
                        lastCopiedIndex = i;
                    }
                    makeUpper = false;
                }
                else
                {
                    makeUpper = true;
                }
            }
            //we copy the rest of the string:
            if (lastCopiedIndex < length - 1)
            {
                result += str.Substring(lastCopiedIndex + 1);
            }
            return result;


            //bool isFirst = true;
            //string[] spaceSplittedString = str.Split(' ');
            //foreach (string s in spaceSplittedString)
            //{
            //    if (isFirst)
            //    {
            //        isFirst = false;
            //    }
            //    else
            //    {
            //        result += " ";
            //    }
            //    result += MakeFirstCharUpperAndRestLower(s);
            //}
            //return result;
        }
    }
}
