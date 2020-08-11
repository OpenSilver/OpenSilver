

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
    public class UnionTypeModel : TSType
    {
        public string Name { get { return this.ToString(); } set {} }
        public List<TSType> Types { get; set; }
        public int Count
        {
            get
            {
                return this.Types.Count();
            }
        }

        public UnionTypeModel()
        {
            this.Types = new List<TSType>();
        }

        public override string ToString()
        {
            string typeList = "";

            for (int i = 0; i < this.Types.Count; ++i)
            {
                if (i > 0)
                    typeList += ", ";
                typeList += this.Types[i].ToString();
            }

            return "UnionType<" + typeList + ">";
        }

        public string New(string jsObj)
        {
            return this.ToString() + ".FromJavaScriptInstance(" + jsObj + ")";
        }
    }
}
