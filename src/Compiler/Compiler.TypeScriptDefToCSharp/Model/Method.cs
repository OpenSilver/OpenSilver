

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
    public class Method : Function
    {
        public bool IsConstructor { get; set; }

        public Method(Container<Declaration> super)
            : base(super)
        {
        }

        public Method(XElement elem, Container<Declaration> super, TypeScriptDefContext context)
            : base(elem, super, context)
        {
            this.IsConstructor = (this.Name == "constructor");
        }

        public override string ToString()
        {
            var res = new StringBuilder();

            // Normal method
            if (!this.IsConstructor)
            {
                res.Append("public ")
                   .Append(this.Static ? "static " : "")
                   .Append(base.ToString());
            }
            // Constructor
            else
            {
                var paramList = string.Join(", ", this.Params.Select(p => p.ToString()));

                // Partial initialization method so that we can extend this constructor later
                res.Append("partial void Initialize(")
                   .Append(paramList)
                   .AppendLine(");");

                // Static method returning a newly instanciated, native JS instance
                // for custom instanciation scenarios where we need to manipulate directly said instance
                res.Append("public static object New(")
                   .Append(paramList)
                   .AppendLine(")")
                   .AppendLine("{")
                   .Append("return Interop.ExecuteJavaScript(\"new ")
                   .Append(this.Super.SkippedFullName(".", 1))
                   .Append("(")
                   .Append(string.Join(", ", Enumerable.Range(0, this.Params.Count).Select(i => "$" + i)))
                   .Append(")\"");
                if (this.Params.Any())
                    res.Append(", ")
                       .Append(string.Join(", ", this.GetJSParamsValues()));
                res.AppendLine(");")
                   .AppendLine("}");

                // Constructor
                res.Append("public ")
                   .Append(((Class)this.Super).Name)
                   .Append("(")
                   .Append(paramList)
                   .AppendLine(")")
                   .AppendLine("{");
                // Underlying JS instance initialization (using the New static method)
                res.Append("this.UnderlyingJSInstance = ")
                   .Append(((TSType)this.Super).Name)
                   .Append(".New(")
                   .Append(string.Join(", ", this.Params.Select(p => p.Name)))
                   .AppendLine(");");
                // Call to the Initialize method
                res.Append("this.Initialize(")
                   .Append(string.Join(", ", this.Params.Select(p => p.Name)))
                   .AppendLine(");")
                   .AppendLine("}");
            }

            return res.ToString();
        }
    }
}
