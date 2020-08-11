

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
    public class Class : TypeDeclaration,
        Declaration.Container<TypeDeclaration>,
        Declaration.Container<Property>,
        Declaration.Container<Method>
    {
        public string Prefix { get; set; }

        public HashSet<TypeDeclaration> TypeDeclarations { get; set; }
        public List<Property> Properties { get; set; }
        public List<Method> Methods { get; set; }
        // List of class/interface names of which this interface inherit
        public List<string> Inherit { get; set; }
        // public (true) or private (false)?
        public bool Public { get; set; }
        public bool Static { get; set; }
        // Generic args (only stored as a string, no check done on that part)
        public string GenericArg { get; set; }
        // Let us know if we can call the constructor or just create an anonymous object
        public bool HasParameterlessConstructor { get; set; }

        private void InitProperties()
        {
            this.TypeDeclarations = new HashSet<TypeDeclaration>();
            this.Properties = new List<Property>();
            this.Methods = new List<Method>();
            this.Inherit = new List<string>();
            this.Public = false;
            this.GenericArg = "";
        }

        public Class(Container<Declaration> super)
            : base(super)
        {
            this.InitProperties();
        }

        public Class(XElement elem, Container<Declaration> super, TypeScriptDefContext context)
            : base(elem, super, context)
        {
            this.InitProperties();

            this.Name = Tool.GetIdent(elem);
            if (this.Name == null)
                this.Name = "";
            // Split based on '\\' is done if the interface has name based on the .t.ds or xml file
            // if this file is in subfolder, it's name will include some '\\' (or '/', that's the same here)
            // But this doesn't seems to work right now, need to fix it
            string[] path = this.Name.Split('\\');
            // Name is in fact the last element of the path
            this.Name = path.Last();
            // We remove the name from the path
            path = path.Take(path.Count() - 1).ToArray();
            // Prefix is now the first file path (without the name)
            if (path.Count() > 0)
                this.Prefix = string.Join("\\", path) + "\\";
            else
                this.Prefix = "";

            // Parsing the "extends" parts (inheritance)
            //var Extends = elem.Element("extends");
            //if (Extends != null)
            //{
            //    foreach (XElement e in Extends.Elements())
            //    {
            //        if (e.Name.LocalName == "dotident")
            //        {
            //            var b = new BasicType(e);
            //            if (b.ToString(this.Super) != "object")
            //                this.Inherit.Add(b.ToString(this.Super));
            //        }
            //        else if (e.Name.LocalName == "generic")
            //        {
            //            var g = new Generic(e, context);
            //            this.Inherit.Add(g.ToString(this.Super));
            //        }
            //    }
            //}
        }

        // Parse and add content to this class
        public void AddContent(XElement content, TypeScriptDefContext context)
        {
            foreach (XElement e in content.Elements())
            {
                switch (e.Name.LocalName)
                {
                    // Properties
                    case "variable":
                        this.AddProperty(new Property(e, this, context));
                        break;
                    // Methods
                    case "function":
                        Method m = new Method(e, this, context);
                        if (!m.HasString)
                            this.AddMethod(m);
                        if (m.Name == "constructor" && m.Params.Count == 0)
                            this.HasParameterlessConstructor = true;
                        break;
                    // If the class is a generic and has some "generic arguments"
                    case "generic_arg":
                        this.GenericArg = e.Attribute("text").Value;
                        break;
                    // Nested interfaces
                    case "interface":
                        this.AddTypeDeclaration(TypeDeclaration.New(e, this, context));
                        break;
                    // Nested classes
                    case "class":
                        this.AddTypeDeclaration(TypeDeclaration.New(e, this, context));
                        break;
                    // Nested enums
                    case "enum":
                        this.AddTypeDeclaration(TypeDeclaration.New(e, this, context));
                        break;
                }
            }
        }

        // Add a property, and rename it if it has the same name as this class
        // (this is allowed in TypeScript because of the constructor keyword)
        private void AddProperty(Property p)
        {
            if (p.Name == this.Name)
                p.Name += "_Property";
            this.Properties.Add(p);
        }

        // Add a method, and rename it if it has the same name as this class
        // (this is allowed in TypeScript because of the constructor keyword)
        private void AddMethod(Method m)
        {
            if (m.Name == this.Name)
                m.Name += "_Method";
            this.Methods.Add(m);
        }

        // Add a type declaration, and rename it if its name conflicts with a property of this class
        private void AddTypeDeclaration(TypeDeclaration t)
        {
            var prop = this.Properties.FirstOrDefault(p => p.Name == t.Name);
            if (prop != null)
            {
                t.Name += "_Type";
                ((TSType)t).Name += "_Type";
            }
            this.TypeDeclarations.Add(t);
        }

        // Exporting a Class create a new file (like exporting Interface or Enum)
        public override void Export(TypeScriptDefContext context)
        {
            var res = new StringBuilder();
            // Get the imports of the Super Namespace and it's parents
            var imports = this.FirstAncestor<Namespace>().GetImportList();
            // Join them with a NewLine as separator (overrided imports.ToString() format it as needed)
            res.Append(string.Join(Environment.NewLine, imports));

            // If there was is at least an import, we add a NewLine (useless but pretty)
            if (imports.Any())
                res.AppendLine(Environment.NewLine);

            // Put the class in the good namespace (with full namespace's name)
            res.Append("namespace ")
               .AppendLine(this.Super.FullName("."))
                .AppendLine("{");

            // Write the class and close the namespace
            res.Append(this.ToString())
                .AppendLine("}");

            // Get the destination path (namespace structure)
            string path = context.OutputDirectory + this.Super.FullName("\\") + "\\" + this.Prefix;
            // If needed, create the destination folder
            Directory.CreateDirectory(path);

            string fileName = path + this.Name + ".cs";
            File.WriteAllText(fileName, Tool.ReIndent(res.ToString()));
            if (context.CurrentGeneratedFiles != null)
                context.CurrentGeneratedFiles.Add(fileName);
        }

        public override string ToString()
        {
            this.ClearDoublons();
            var res = new StringBuilder();

            // TODO IMPLEMENT CONDITION K_EXPORT
            if (true)
                res.Append("public ");
            if (this.Static)
                res.Append("static ");

            // Partial class so that the library designer can extend it
            res.Append("partial class" + " " + this.Name + this.GenericArg);

            // Add inheritance ("class ClassName : Inheritance1, Inheritance2, ...")
            if (!this.Static)
            {
                res.Append(" : " + string.Join(", ", this.Inherit));
                if (this.Inherit.Any())
                    res.Append(", ");
                res.Append("IJSObject");
            }

            // Add the UnderlyingJSInstance and the properties
            res.AppendLine()
               .AppendLine("{");

            if (!this.Static)
                res.AppendLine("public object UnderlyingJSInstance { get; set; }")
                   .AppendLine();

            foreach (var p in this.Properties)
                res.AppendLine(p.ToString()); // overrided Properties.ToString() format it as needed

            // If there is no parameterless constructor and if the class is not static,
            // we add one such constructor that simply instanciates an anonymous object
            if (!(this.Static || this.HasParameterlessConstructor))
                res.AppendLine("partial void Initialize();")
                   .AppendLine("public " + this.Name + "()")
                   .AppendLine("{")
                   .AppendLine("this.UnderlyingJSInstance = Interop.ExecuteJavaScript(\"new Object()\");")
                   .AppendLine("this.Initialize();")
                   .AppendLine("}")
                   .AppendLine();

            // Write its methods and nested classes/interfaces (overrided ToString() format them as needed)
            foreach (var m in this.Methods)
                res.AppendLine(m.ToString());

            foreach (var t in this.TypeDeclarations)
                res.AppendLine(t.ToString());

            // Remove last newline character for pretty printing
            if (res.ToString().EndsWith(Environment.NewLine + Environment.NewLine))
                res.Remove(res.ToString().LastIndexOf(Environment.NewLine), Environment.NewLine.Length); 

            res.AppendLine("}");

            return res.ToString();
        }

        public override string New(string jsObj)
        {
            return "JSObject.FromJavaScriptInstance<" + ((TSType)this).ToString() + ">(" + jsObj + ")";
        }

        private void ClearDoublons()
        {
            for (int i = 0; i < this.Methods.Count; ++i)
            {
                for (int j = i + 1; j < this.Methods.Count; ++j)
                {
                    if (this.Methods[i].Name == this.Methods[j].Name &&
                        this.Methods[i].Params.Count == this.Methods[j].Params.Count)
                    {
                        bool isIdentical = true;

                        for (int k = 0; k < this.Methods[i].Params.Count; ++k)
                        {
                            if (this.Methods[i].Params[k].Type.ToString() !=
                                this.Methods[j].Params[k].Type.ToString())
                            {
                                isIdentical = false;
                                break;
                            }
                        }

                        if (isIdentical)
                        {
                            this.Methods.RemoveAt(j);
                            --j;
                        }
                    }
                }
            }

            for (int i = 0; i < this.Properties.Count; ++i)
            {
                for (int j = i + 1; j < this.Properties.Count; ++j)
                {
                    if (this.Properties[i].Name == this.Properties[j].Name &&
                        this.Properties[i].Type.ToString() == this.Properties[j].Type.ToString())
                    {
                        this.Properties.RemoveAt(j);
                        --j;
                    }
                }
            }
        }

        // IEnumerable implementation

        IEnumerator<TypeDeclaration> IEnumerable<TypeDeclaration>.GetEnumerator()
        {
            return this.TypeDeclarations.GetEnumerator();
        }

        IEnumerator<Property> IEnumerable<Property>.GetEnumerator()
        {
            return this.Properties.GetEnumerator();
        }

        IEnumerator<Method> IEnumerable<Method>.GetEnumerator()
        {
            return this.Methods.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.TypeDeclarations.GetEnumerator();
        }
    }
}
