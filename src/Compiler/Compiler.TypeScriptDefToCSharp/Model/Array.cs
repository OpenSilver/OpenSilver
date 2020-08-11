

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
