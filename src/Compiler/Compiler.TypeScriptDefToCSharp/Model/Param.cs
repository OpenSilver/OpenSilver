using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeScriptDefToCSharp.Model
{
    public class Param : Variable
    {
        public bool Spread { get; set; }

        public Param(Container<Declaration> super)
            : base(super)
        {
        }

        public Param(XElement elem, Container<Declaration> super, bool spread, TypeScriptDefContext context)
            : base(elem, super, context)
        {
            this.Spread = spread;
        }

        public override string ToString()
        {
            string type = this.Type.ToString();
            string res;

            if (type == "double" || type == "bool")
                type += "?";

            if (this.Spread)
                return "params " + type + "[] " + Tool.ClearKeyWord(this.Name);
            else
                res = type + " " + Tool.ClearKeyWord(this.Name);

            if (this.Optional)
                res += " = null";
            return res;
        }
    }
}
