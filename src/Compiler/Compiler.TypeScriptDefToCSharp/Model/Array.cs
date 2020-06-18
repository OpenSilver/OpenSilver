using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScriptDefToCSharp.Model
{
    public class Array : TSType
    {
        public string Name
        {
            get
            {
                return this.Type.Name + "[]";
            }
            set { }
        }

        public TSType Type { get; private set; }

        public Array(TSType type)
        {
            this.Type = type;
        }

        public string New(string jsObj)
        {
            return "JSObject.FromJavaScriptInstance<" + this.ToString() + ">(" + jsObj + ")";
        }

        public override string ToString()
        {
            return "JSArray<" + this.Type.ToString() + ">";
        }
    }
}
