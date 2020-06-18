using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeScriptDefToCSharp.Model
{
    // For each Anonymous Type we in fact create a class which will be in the AnonymousTypes namespace
    public class AnonymousType : TSType
    {
        public string Name
        {
            get { return Class.Name; } 
            set { Class.Name = value; } 
        }
        public Class Class { get; set; }
        // This is the ID of the Anonymous Type, it allows to differenciate every AnonymousType
        static public int AnonID = 1;
        static public List<AnonymousType> Anons = new List<AnonymousType>();

        public AnonymousType()
        {
            Anons.Add(this);
        }

        public AnonymousType(XElement elem, TypeScriptDefContext context)
            : this()
        {
            this.Class = new Class(elem, null, context);
            this.Class.Name = "AnonymousType" + AnonID++;
            this.Class.AddContent(elem, context);
            //this.Class.Super = new Namespace();
            //this.Class.Super.Name = "AnonymousTypes";
            context.CurrentGeneratedFiles.Add(context.OutputDirectory + "AnonymousTypes\\" + this.Class.Name + ".cs");
        }

        public void Export(TypeScriptDefContext context)
        {
            this.Class.Export(context);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public string New(string jsObj)
        {
            return "JSObject.FromJavaScriptInstance<" + this.ToString() + ">(" + jsObj + ")";
        }
    }
}