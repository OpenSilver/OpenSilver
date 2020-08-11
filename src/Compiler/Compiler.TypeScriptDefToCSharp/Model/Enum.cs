

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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeScriptDefToCSharp.Model
{
    public class Enum : TypeDeclaration
    {
        public List<string> Values { get; set; }

        private void InitProperties()
        {
            this.Values = new List<string>();
        }

        public Enum(Container<Declaration> super)
            : base(super)
        {
            this.InitProperties();
        }

        public Enum(XElement elem, Container<Declaration> super, TypeScriptDefContext context)
            : base(elem, super, context)
        {
            this.InitProperties();

            foreach (XElement e in elem.Elements())
            {
                if (e.Name.LocalName != "ident")
                    throw new Exception();
                if (this.Name == null)
                    this.Name = e.Attribute("text").Value;
                else
                    this.Values.Add(e.Attribute("text").Value);
            }
        }

        public override string ToString()
        {
            var res = new StringBuilder();

            // We generate enums as sealed classes to allow dynamic enum values
            // based on the corresponding JS enum values
            res.AppendLine("public sealed class " + this.Name);
            res.AppendLine("{");

            // An enum value has a Name and a corresponding JS enum value
            res.AppendLine("public string Name { get; private set; }");
            res.AppendLine("public object JSValue { get; private set; }");

            res.AppendLine();

            // One static member for each enum value
            foreach (var value in this.Values)
                res.AppendLine("public static readonly " + this.Name + " " + value + ";");

            res.AppendLine();

            // The class holds a dictionary mapping every JS enum value
            // to its C# counterpart (hence the name "_reverseMapping")
            res.AppendLine("private static Dictionary<object, " + this.Name + "> _reverseMapping = new Dictionary<object, " + this.Name + ">();");

            res.AppendLine();

            // Static constructor
            res.AppendLine("static " + this.Name + "()");
            res.AppendLine("{");
            foreach (var value in this.Values)
                // First we retrieve the JS enum values by converting the JS objects to strings
                res.AppendLine("var jsValue_" + value + " = JSObject.Helper_ConvertTo<string>(" + "Interop.ExecuteJavaScript(\"" + this.SkippedFullName(".", 1) + "." + value + "\")" + ");");
            res.AppendLine();
            foreach (var value in this.Values)
                // Then we initialize the C# enum values from the JS enum values
                res.AppendLine(value + " = new " + this.Name + "(\"" + value + "\", jsValue_" + value + ");");
            res.AppendLine();
            foreach (var value in this.Values)
                // Finally we fill the aforementioned dictionary
                res.AppendLine("_reverseMapping.Add(jsValue_" + value + ", " + value + ");");
            res.AppendLine("}");

            res.AppendLine();

            // Instance constructor to fill the properties
            res.AppendLine("private " + this.Name + "(string name, object jsValue)");
            res.AppendLine("{");
            res.AppendLine("this.Name = name;");
            res.AppendLine("this.JSValue = jsValue;");
            res.AppendLine("}");

            res.AppendLine();

            // Explicit cast operator to get C# enum values from JS objects using the dictionary
            res.AppendLine("public static explicit operator " + this.Name + "(JSObject o)");
            res.AppendLine("{");
            res.AppendLine("return _reverseMapping[JSObject.Helper_ConvertTo<string>(o.UnderlyingJSInstance)];");
            res.AppendLine("}");

            res.AppendLine();

            // ToString method which just returns the Name
            res.AppendLine("public override string ToString()");
            res.AppendLine("{");
            res.AppendLine("return this.Name;");
            res.AppendLine("}");

            res.AppendLine("}");

            return res.ToString();
        }

        public override void Export(TypeScriptDefContext context)
        {
            var res = new StringBuilder();
            var imports = this.FirstAncestor<Namespace>().GetImportList();

            res.Append(string.Join(Environment.NewLine, imports));
            if (imports.Any())
                res.AppendLine(Environment.NewLine);

            res.Append("namespace ")
                .AppendLine(Super.FullName("."))
                .AppendLine("{");

            res.Append(this.ToString())
                .AppendLine("}");

            // Get the destination path (namespace structure)
            string path = Super.FullName("\\") + "\\";
            // Get the full relative file path
            string fullPath = context.OutputDirectory + path + this.Name + ".cs";

            // Split the full path
            string[] tmp = fullPath.Split('\\');
            // Remove the filename from the path
            tmp = tmp.Take(tmp.Count() - 1).ToArray();
            // If needed, create the destination folder
#if LOG_VERBOSE
            context.Logger.WriteMessage("enum: " + fullPath);
#endif
            Directory.CreateDirectory(string.Join("\\", tmp));

            File.WriteAllText(fullPath, Tool.ReIndent(res.ToString()));
            context.CurrentGeneratedFiles.Add(fullPath);
        }

        public override string New(string jsObj)
        {
            return "(" + ((TSType)this).ToString() + ")" + "(new JSObject(" + jsObj + "))";
        }
    }
}
