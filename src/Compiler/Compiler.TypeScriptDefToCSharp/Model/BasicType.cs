

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
    public class BasicType : TSType
    {
        public static readonly BasicType Object = new BasicType() { Name = "object" };

        public string Name { get; set; }

        public BasicType()
        {
        }

        public BasicType(XElement elem)
        {
            this.Name = elem.Attribute("text").Value;
            switch (this.Name)
            {
                case "boolean":
                case "Boolean":
                    this.Name = "bool";
                    break;
                case "number":
                case "Number":
                    this.Name = "double";
                    break;
                case "Function":
                    this.Name = "Action";
                    break;
                // Transform into "object"
                #region JS API Types to "object"
                case "any":
                case "Error":
                case "this":
                case "null":
                    this.Name = "IJSObject";
                    break;
                #endregion
                //case "Date":
                //    this.Name = "DateTime";
                //    break;
                //case "Text":
                //    this.Name = "string";
                //    break;
                //case "Int8Array":
                //    this.Name = "List<System.Char>";
                //    break;
                //case "Uint8Array":
                //case "Uint8ClampedArray":
                //    this.Name = "List<System.Byte>";
                //    break;
                //case "Int16Array":
                //    this.Name = "List<System.Int16>";
                //    break;
                //case "Uint16Array":
                //    this.Name = "List<System.Uint16>";
                //    break;
                //case "Int32Array":
                //    this.Name = "List<System.Int32>";
                //    break;
                //case "Uint32Array":
                //    this.Name = "List<System.Uint32>";
                //    break;
                //case "Float32Array":
                //    this.Name = "List<System.Single>";
                //    break;
                //case "Float64Array":
                //    this.Name = "List<System.Double>";
                //    break;
            }
        }

        public override string ToString()
        {
            if (!(Tool.IsBasicType(this)))
                this.Name = "IJSObject";
            return this.Name;
        }

        public string New(string jsObj)
        {
            string res;

            switch (this.ToString())
            {
                case "double":
                    res = "Convert.ToDouble(" + jsObj + ")";
                    break;
                case "string":
                    res = "Convert.ToString(" + jsObj + ")";
                    break;
                case "bool":
                    res = "Convert.ToBoolean(" + jsObj + ")";
                    break;
                case "Action":
                    res = "new Action(() => { Interop.ExecuteJavaScript(\"$0()\", " + jsObj + "); })";
                    break;
                case "IJSObject":
                    res = "new JSObject(" + jsObj + ")";
                    break;
                case "object":
                    res = jsObj;
                    break;
                default:
                    res = "JSObject.FromJavaScriptInstance<" + this.ToString() + ">(" + jsObj + ")";
                    break;
            }

            return res;
        }
    }
}
