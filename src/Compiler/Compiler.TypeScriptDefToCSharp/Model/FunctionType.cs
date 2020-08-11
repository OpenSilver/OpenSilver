

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
    // This describe a callback type, known as Action<> or Func<> in C#
    public class FunctionType : TSType
    {
        // Unused but needed for the TSType implementation
        public string Name { get { return ""; } set { } }
        // List of the function type parameters
        public List<Param> Params { get; set; }
        public TSType ReturnType { get; set; }

        public FunctionType()
        {
            this.Params = new List<Param>();
        }

        public FunctionType(XElement elem, TypeScriptDefContext context)
            : this()
        {
            var Params = elem.Element("paramlist");

            if (Params != null)
            {
                var ParamListElem = Params.Elements();
                bool Spread = false;

                foreach (XElement e in ParamListElem)
                {
                    if (e.Name.LocalName == "spreadop")
                        Spread = true;
                    else
                    {
                        Param p = new Param(e, null, false, context);

                        p.Spread = Spread;
                        this.Params.Add(p);
                        Spread = false;
                    }

                }
            }

            this.ReturnType = Tool.NewType(elem.Element("type"), context);
        }

        public override string ToString()
        {
            string res;

            if (this.ReturnType != null)
                res = "Func";
            else
                res = "Action";

            if (this.ReturnType != null || this.Params.Any())
                res += "<";

            for (int i = 0; i < this.Params.Count(); i++)
            {
                if (i > 0)
                    res += ", ";
                res += this.Params[i].Type.ToString();
            }

            if (this.ReturnType != null)
            {
                if (this.Params.Any())
                    res += ", ";
                res += this.ReturnType.ToString();
            }

            if (this.ReturnType != null || this.Params.Any())
                res += ">";

            return res;
        }

        public string New(string jsObj)
        {
            string res = "";

            res += "new " + this.ToString() + "((";
            for (int i = 0; i < this.Params.Count; i++)
            {
                if (i > 0)
                    res += ", ";
                res += "arg" + (i + 1);
            }
            res += ") =>" + Environment.NewLine;
            res += "    {" + Environment.NewLine;
            res += "        ";

            string _jsObj = "";

            _jsObj += "Interop.ExecuteJavaScript(\"$0(";
            for (int i = 0; i < this.Params.Count; i++)
            {
                if (i > 0)
                    _jsObj += ", ";
                _jsObj += "$" + (i + 1);
            }
            _jsObj += ")\", " + jsObj;
            for (int i = 0; i < this.Params.Count; i++)
            {
                _jsObj += ", arg" + (i + 1);
                if (!(this.Params[i].Type is FunctionType))
                    _jsObj += ".ToJavaScriptObject()";
            }
            _jsObj += ")";

            if (this.ReturnType != null)
                res += "return " + this.ReturnType.New(_jsObj);
            else
                res += _jsObj;

            res += ";" + Environment.NewLine;
            res += "    })";

            return res;
        }
    }
}
