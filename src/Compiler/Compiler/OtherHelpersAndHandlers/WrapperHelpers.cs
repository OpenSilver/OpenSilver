using System;
using System.IO;
using System.Reflection;

namespace DotNetForHtml5.Compiler
{
    internal static class WrapperHelpers
    {
        internal static string AppendDateToLibraryFileName(string libraryFileName, string indexHtmlFile)
        {
            // This method is useful to prevent caching of the library by appending the date:
            string stringToReplace = libraryFileName + "\"";
            string replaceWith = libraryFileName + String.Format("?{0:yyyyMdHHmm}", DateTime.Now) + "\"";
            if (!indexHtmlFile.Contains(stringToReplace))
                throw new Exception("String not found in index.html: " + stringToReplace);
            return indexHtmlFile.Replace(stringToReplace, replaceWith);
        }

        internal static string ReadTextFileFromEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string embeddedResourceFullName = GetEmbeddedRessourceFullNameFromFileName(fileName, assembly);
            using (Stream stream = assembly.GetManifestResourceStream(embeddedResourceFullName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        static string GetEmbeddedRessourceFullNameFromFileName(string fileName, Assembly assembly)
        {
            foreach (string embeddedResourceFullName in assembly.GetManifestResourceNames())
            {
                if (embeddedResourceFullName.EndsWith("." + fileName))
                    return embeddedResourceFullName;
            }
            throw new Exception("Embedded resource not found: " + fileName);
        }
    }
}
