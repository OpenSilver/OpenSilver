

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
    public class Import
    {
        public string Alias { get; set; }
        public string Name { get; set; }

        public Import()
        {
        }

        public Import(XElement elem)
        {
            var Child = elem.Elements();

            // This method will handle the following TypeScript Definitions code:
            //      import Graphic = require("lib/graphic");
            //
            // This code produces the following XML:
            //      <import>
            //        <dotident text="Graphic" />
            //        <k_require />
            //        <dotident_with_additional_chars_allowed text="lib/graphic" />
            //      </import>
            //
            // In this example, we want "Graphic" to be considered as the "Alias",
            // and "lib/graphic" to be considered as the "Name", in order to generate
            // the following C# code:
            //      using Graphic = lib.graphic;
            //
            // Note: in the current implementation, TypeScript "modules" are considered
            // the same thing as TypeScript "namespaces". For information on the original
            // difference between the two, refer to:
            // http://stackoverflow.com/questions/38582352/module-vs-namespace-import-vs-require-typescript


            XAttribute attribute = null;
            foreach (XElement e in Child)
            {
                if (this.Alias == null)
                {
                    attribute = e.Attribute("text");
                    if (attribute != null)
                        this.Alias = Tool.RemoveDotIdentAdditionalChars(attribute.Value); // Note: we remove the unsupported characters because the TypeScript module name may contain some (such as forward slashes and dashes).
                }
                else
                {
                    attribute = e.Attribute("text");
                    if (attribute != null)
                        this.Name = Tool.RemoveDotIdentAdditionalChars(attribute.Value); // Note: we remove the unsupported characters because the TypeScript module name may contain some (such as forward slashes and dashes).
                }
            }
            if (this.Alias == this.Name)
                this.Alias = null;
        }

        public override string ToString()
        {
            string alias = "";

            if (this.Alias == this.Name)
                this.Alias = null;
            if (this.Alias != null)
                alias = this.Alias + " = ";

            return "using " + alias + this.Name + ";";
        }

        public override bool Equals(object obj)
        {
            return obj is Import && this.Alias == ((Import)obj).Alias && this.Name == ((Import)obj).Name;
        }

        public override int GetHashCode()
        {
            return (new Tuple<string, string>(this.Name, this.Alias)).GetHashCode();
        }
    }
}
