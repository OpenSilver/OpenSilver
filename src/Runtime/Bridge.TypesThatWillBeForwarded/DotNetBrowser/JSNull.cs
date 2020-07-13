using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetBrowser
{
    public class JSNull : JSValue
    {
        //public JSNull();

        public override bool IsNull() { throw new NotImplementedException(); }
        public override string ToString() { throw new NotImplementedException(); }
    }
}
