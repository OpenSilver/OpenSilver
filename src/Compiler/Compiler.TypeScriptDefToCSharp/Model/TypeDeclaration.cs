

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeScriptDefToCSharp.Model
{
    public abstract class TypeDeclaration : Declaration, TSType
    {
        // We keep the TSType name distinct from the Declaration name,
        // because the former should always be the full name to avoid ambiguity
        string TSType.Name { get; set; }

        public TypeDeclaration(Container<Declaration> super)
            : base(super) { }

        public TypeDeclaration(XElement elem, Container<Declaration> super, TypeScriptDefContext context)
            : this(super)
        {
            ((TSType)this).Name = TypeDeclaration.GetFullName(elem);
        }

        string TSType.ToString()
        {
            return ((TSType)this).Name;
        }

        public abstract string New(string jsObj);

        // Dictionary holding all TypeDeclaration instances, identified by their full name.
        // It is used on one hand to avoid redundant checks of the XML declaration file,
        // and on the other hand to make all variables reference the same types,
        // so that the types can easily be modified dynamically during parsing
        private static readonly Dictionary<string, TypeDeclaration> Instances = new Dictionary<string, TypeDeclaration>();

        // Retrieve the full name of an XML type declaration
        private static string GetFullName(XElement elem)
        {
            // First we retrieve the local name
            string name = Tool.GetIdent(elem);
            // Then we retrieve and prepend the parent's full name
            XElement parent;
            do
            {
                parent = elem.Ancestors().FirstOrDefault(e =>
                    TypeDeclaration.IsTypeDeclarationContainer(e)
                    && e.Name.LocalName != "start" // To ignore the global namespace
                );
                if (parent != null)
                {
                    name = Tool.GetIdent(parent) + "." + name;
                    elem = parent;
                }
            }
            while (parent != null);

            return name;
        }

        // Return whether a given XML element is associated with a
        // class of the Model which implements Container<TypeDeclaration>
        private static bool IsTypeDeclarationContainer(XElement elem)
        {
            switch (elem.Name.LocalName)
            {
                case "start": // Global namespace
                case "namespace":
                case "class":
                case "interface":
                    return true;
                default:
                    return false;
            }
        }

        // Return the TypeDeclaration class associated with a given XML element,
        // or null if there is no mapping
        private static Type GetClass(XElement elem)
        {
            switch (elem.Name.LocalName)
            {
                case "class":
                    return typeof(Model.Class);
                case "interface":
                    return typeof(Model.Class);
                case "enum":
                    return typeof(Model.Enum);
                default:
                    return null;
            }
        }

        // Return the TypeDeclaration instance associated with a given XML type declaration,
        // or a newly created one that is added to the dictionary if it doesn't exist
        private static TypeDeclaration GetInstance(XElement elem, Container<Declaration> super, TypeScriptDefContext context)
        {
            Type typeDeclarationClass = TypeDeclaration.GetClass(elem);
            string typeDeclarationName = TypeDeclaration.GetFullName(elem);
            try
            {
                var inst = TypeDeclaration.Instances[typeDeclarationName];

                // We update the declaration's Super if it wasn't specified previously
                // (needed for type declarations instanciated because of a variable of that type)
                if (inst.Super == null)
                    inst.Super = super;

                return inst;
            }
            catch (KeyNotFoundException)
            {
                var inst = (TypeDeclaration)Activator.CreateInstance(typeDeclarationClass, elem, super, context);
                TypeDeclaration.Instances[typeDeclarationName] = inst;

                // We add content after updating the dictionary to avoid cycles
                // when declarations refer to their ancestors
                if (inst is Container<Declaration>)
                    ((Container<Declaration>)inst).AddContent(elem, context);

                return inst;
            }
        }

        // Check if a given type name is declared somewhere in a given XML subtree,
        // and return the associated TypeDeclaration instance if it is, null otherwise
        private static TypeDeclaration New(string[] name, XElement root, Container<Declaration> super, TypeScriptDefContext context)
        {
            // We create a list of the elements we're going to inspect
            var elems = new List<XElement>();
            // First we inspect the root itself (except if it is the global namespace)
            if (root.Name.LocalName != "start")
                elems.Add(root);
            // If there is no declaration directly in the root, we inspect its children
            IEnumerable<XElement> children;
            // We need to handle the special case of namespace elements,
            // which do not hold their declarations as direct children
            if (root.Name.LocalName == "namespace")
                children = root.Element("namespacecontent").Elements();
            else
                children = root.Elements();
            elems.AddRange(children);

            // Then for each element in the list
            foreach (XElement e in elems)
            {
                // First we make sure that it correponds either to a TypeDeclaration or a Container<TypeDeclaration>
                Type typeDeclarationClass = TypeDeclaration.GetClass(e);
                bool isTypeDeclarationContainer = TypeDeclaration.IsTypeDeclarationContainer(e);

                if (typeDeclarationClass != null || isTypeDeclarationContainer)
                {
                    // The element identifier must be at most as long as the type name
                    string[] ident = Tool.GetIdent(e).Split('.');
                    if (ident.Length <= name.Length)
                    {
                        // If the hole identifier matches the beginning of the name
                        int match = ident.ToList().FirstMismatchIndex(name.ToList());
                        if (match == ident.Length)
                        {
                            // Either the beginning is actually the type (local) name...
                            if (name.Length == 1)
                            {
                                // ...and if the element is associated with a TypeDeclaration class,
                                // it means we have found a valid declaration, so we return an instance of said class
                                if (typeDeclarationClass != null)
                                    return TypeDeclaration.GetInstance(e, super, context);
                            }
                            // Or the beginning is just the name of an ancestor container...
                            else if (isTypeDeclarationContainer)
                            {
                                // ...and therefore we call the method recursively on the element
                                var res = TypeDeclaration.New(name.Skip(match).ToArray(), e, super, context);
                                // If we found a valid declaration during the recursive call, we return it
                                if (res != null)
                                    return res;
                                // Otherwise there might be another container with the same name, so we keep searching.
                                // For example it is possible that class a.b does not contain the declaration,
                                // while namespace a.b, which is going to be merged with class a.b by the compiler, does contain it.
                            }
                        }
                    }
                }
            }

            // If there is no element containing a declaration, we return null
            return null;
        }

        // Take either an XML type declaration or a type.dotident element, and return the associated
        // TypeDeclaration instance if it is declared somewhere in the XML, null otherwise
        public static TypeDeclaration New(XElement elem, Container<Declaration> super, TypeScriptDefContext context)
        {
            // If the argument passed is an XML type declaration, we don't search
            // in the XML and directly return the associated instance
            if (TypeDeclaration.GetClass(elem) != null)
                return TypeDeclaration.GetInstance(elem, super, context);

            string[] name = elem.Attribute("text").Value.Split('.');
            XElement parent;
            TypeDeclaration typeDeclaration = null;

            do
            {
                // We check if there is a declaration in the current namespace
                parent = elem.Ancestors().FirstOrDefault(e => TypeDeclaration.IsTypeDeclarationContainer(e));
                if (parent != null)
                {
                    typeDeclaration = TypeDeclaration.New(name, parent, super, context);
                    elem = parent;
                }
            }
            // If not, we check in the parent namespace, and if not in the parent of the parent,
            // and so on until we reach the root namespace
            while (typeDeclaration == null && parent != null);

            return typeDeclaration;
        }
    }
}