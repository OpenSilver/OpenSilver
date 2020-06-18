using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeScriptDefToCSharp.Model
{
    public interface TSType
    {
        string Name { get; set; }

        string New(string jsObj);
        string ToString();
    }
}
