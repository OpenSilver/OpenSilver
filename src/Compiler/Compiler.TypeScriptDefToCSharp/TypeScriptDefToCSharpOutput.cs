using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScriptDefToCSharp
{
    public class TypeScriptDefToCSharpOutput
    {
        public TypeScriptDefToCSharpOutput()
        {
            TypeScriptDefinitionFiles = new List<TypeScriptDefinitionFile>();
        }

        public List<TypeScriptDefinitionFile> TypeScriptDefinitionFiles { get; set; }

        public TypeScriptDefinitionFile ExtractFileByName(string fileName)
        {
            foreach (var file in TypeScriptDefinitionFiles)
            {
                if (fileName == file.FileName)
                {
                    TypeScriptDefinitionFiles.Remove(file);
                    return file;
                }
            }
            return null;
        }
    }

    public class TypeScriptDefinitionFile
    {
        public TypeScriptDefinitionFile()
        {
            CSharpGeneratedFiles = new List<string>();
        }

        public string FileName { get; set; }
        public string FileContentHash { get; set; }
        public List<string> CSharpGeneratedFiles { get; set; }
    }


}
