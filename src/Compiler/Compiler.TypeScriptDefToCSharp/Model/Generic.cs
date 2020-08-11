

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
    public class Generic : TSType
    {
        public string Name { get; set; }
        public List<TSType> TypeList { get; set; }
        public Generic()
        {
            this.TypeList = new List<TSType>();
        }

        public Generic(XElement elem, TypeScriptDefContext context)
            : this()
        {
            this.Name = elem.Element("generic").Attribute("text").Value;

            if (elem.Element("type") != null)
            {
                var types = Tool.NewType(elem.Element("type"), context);

                if (types is UnionTypeModel)
                    this.TypeList = ((UnionTypeModel)types).Types;
                else
                    this.TypeList.Add((TSType)types);
            }
        }

        public override string ToString()
        {
            if (this.TypeList.Any())
            {
                string param = "";

                for (int i = 0; i < this.TypeList.Count; ++i)
                {
                    if (i > 0)
                        param += ", ";
                    param += this.TypeList[i].ToString();
                }
                return this.Name + "<" + param + ">";
            }
            else
                return this.Name;
        }

        public string New(string jsObj)
        {
            return "JSObject.FromJavaScriptInstance<" + this.ToString() + ">(" + jsObj + ")";
        }
    }
}
