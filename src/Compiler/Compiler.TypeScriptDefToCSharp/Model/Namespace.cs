

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
    public class Namespace : Declaration, Declaration.Container<Declaration>
    {
        public HashSet<Declaration> Declarations { get; set; }
        // Imports are used with "using" in C#
        public List<Import> Imports { get; set; }
        // Used for elements directly put into the namespace (which is not possible in C#)
        public Class StaticClass { get; set; }
        // Parent namespace (if exists)
        new public Namespace Super { get; set; }

        public Namespace(Namespace super, TypeScriptDefContext context) : base(super)
        {
            this.Declarations = new HashSet<Declaration>();
            this.Imports = new List<Import>();
            this.StaticClass = new Class(this) { Static = true };

            // Those are basic usings to allow good working
            this.Imports.Add(new Import() { Alias = "CSHTML5", Name = "CSHTML5" }); // Allow Interop calls
            this.Imports.Add(new Import() { Alias = "System", Name = "System" });
            this.Imports.Add(new Import() { Alias = "System.Collections.Generic", Name = "System.Collections.Generic" });

            // Those are usings that can be useful here
            this.Imports.Add(new Import() { Alias = "AnonymousTypes", Name = "AnonymousTypes" });
            this.Imports.Add(new Import() { Alias = "ToJavaScriptObjectExtender", Name = "ToJavaScriptObjectExtender" });
            this.Imports.Add(new Import() { Alias = "TypeScriptDefinitionsSupport", Name = "TypeScriptDefinitionsSupport" });

            // Those are aliases that are needed to compile, but they can also be added in the CSHTML5.Core
            //this.Imports.Add(new Import() { Alias = "EventListenerOrEventListenerObject", Name = "UnionType<webworker_generated.EventListener, webworker_generated.EventListenerObject>" });
            //this.Imports.Add(new Import() { Alias = "AlgorithmIdentifier", Name = "UnionType<System.String, webworker_generated.Algorithm>" });
            //this.Imports.Add(new Import() { Alias = "IDBKeyPath", Name = "System.String" });
            //this.Imports.Add(new Import() { Alias = "IDBValidKey", Name = "UnionType<System.Int32, System.Double, System.String, System.DateTime, webworker_generated.IDBArrayKey>" });
            //this.Imports.Add(new Import() { Alias = "BufferSource", Name = "System.Collections.Generic.List<object>" });
        }

        public Namespace(XElement elem, Namespace super, TypeScriptDefContext context)
            : this(elem, super, Tool.GetIdent(elem), context)
        {

        }
        
        // The Namespace is the only Model element where we don't parse it into the constructor
        // This is to avoid duplicating a same Namespace
        public Namespace(XElement elem, Namespace super, string name, TypeScriptDefContext context) : this(super, context)
        {
            this.Super = super;
            this.Name = name;

            // The static class is always named [NamespaceNames]Class
            this.StaticClass.Name = name + "Class";
            if (elem != null)
                this.AddContent(elem.Element("namespacecontent"), context);
        }

        // Create if needed a namespace
        // If there is already ns1.ns2 in this namespace
        // and you call it with ns1.ns2.ns3.ns4
        // it will only create ns3 and ns4 and add the content into ns4
        public void AddChildNamespace(XElement elem, TypeScriptDefContext context)
        {
            // Get the full name of the added namespace (ex: "ns1.ns2")
            var Name = Tool.GetIdent(elem);
            // Split the full name into an array
            var name = Name.Split('.');
            // Temporary variables
            Container<Declaration> tmp = this;
            Class cls = null;
            Namespace ns = null;

            // While there are names in the array
            while (name.Count() > 0)
            {
                // Check if there is a class in tmp matching the first remaining name
                cls = tmp.OfType<Class>().FirstOrDefault(t =>
                    t.Name == name[0]
                );
                // If there is one, it means the added namespace references one
                // of its child classes, so tmp become this class
                if (cls != null)
                {
                    tmp = cls;
                }
                // If there is no matching class and tmp is a namespace
                else if (tmp is Namespace)
                {
                    // Search for a matching namespace in tmp
                    ns = tmp.OfType<Namespace>().FirstOrDefault(n =>
                        n.Name == name[0]
                    );
                    // If there isn't one
                    if (ns == null)
                    {
                        // Create it
                        ns = new Namespace(null, (Namespace)tmp, name[0], context);
                        ((Namespace)tmp).Declarations.Add(ns);
                    }
                    // Now we are sure that the namespace exists, so tmp become this one
                    tmp = ns;
                }
                // We remove the first name (the one we created if needed)
                name = name.Skip(1).ToArray();
            }

            // Now that there is no more name in the array
            // tmp is either the class referenced by the added namespace
            // or the added namespace itself
            // In either case we add its content
            tmp.AddContent(elem.Element("namespacecontent"), context);
        }

        // Parse and add content to this namespace
        public void AddContent(XElement content, TypeScriptDefContext context)
        {
#if LOG_VERBOSE
            context.Logger.WriteMessage("AddContent: '" + this.Name + "'");
#endif
            foreach (XElement elem in content.Elements())
            {
                switch (elem.Name.LocalName)
                {
                    case "import":
                        this.Imports.Add(new Import(elem));
                        break;
                    case "namespace":
                        this.AddChildNamespace(elem, context); // We don't add it directly because maybe the namespace already exists
                        break;
                    // Variable directly in the namespace
                    case "variable":
                        this.StaticClass.Properties.Add(new Property(elem, this.StaticClass, context) { Static = true });
                        break;
                    // Function directly in the namespace
                    case "function":
                        this.StaticClass.Methods.Add(new Method(elem, this.StaticClass, context) { Static = true });
                        break;
                    case "enum":
                        this.Declarations.Add(TypeDeclaration.New(elem, this, context));
                        break;
                    case "interface":
                        this.Declarations.Add(TypeDeclaration.New(elem, this, context));
                        break;
                    case "class":
                        this.Declarations.Add(TypeDeclaration.New(elem, this, context));
                        break;
                }
            }
        }

        public override string ToString()
        {
            return this.FullName(".");
        }

        // Exports each children list
        public override void Export(TypeScriptDefContext context)
        {
#if LOG_VERBOSE
            context.Logger.WriteMessage("Namespace Export: '" + this.FullName(".") + "'");
#endif
            foreach (Declaration d in this.Declarations)
            {
#if LOG_VERBOSE
                if (d is Class)
                {
                    context.Logger.WriteMessage("Class: '" + this.FullName(".") + "." + d.Name + "'");
                }
#endif
                d.Export(context);
            }

            // Export the static class only if there is at least a property or a method
            if (StaticClass.Properties.Any() || StaticClass.Methods.Any())
                StaticClass.Export(context);
        }

        // Get the import list of the namespace and all it's parents
        public HashSet<Import> GetImportList()
        {
            HashSet<Import> importsList;

            // Create a new Import list or get the parent list
            if (Super == null)
                importsList = new HashSet<Import>();
            else
                importsList = Super.GetImportList();

            // Loop on every local import
            foreach (var import in this.Imports)
            {
                if (importsList.Contains(import) == false)
                    importsList.Add(import);
            }

            return importsList;
        }

        // IEnumerable implementation

        IEnumerator<Declaration> IEnumerable<Declaration>.GetEnumerator()
        {
            return this.Declarations.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Declarations.GetEnumerator();
        }
    }
}
