using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSilver
{
    [System.AttributeUsage(AttributeTargets.Field)]
    public class JsCallAttribute : System.Attribute {
        private string _jsCall;

        public string JS => _jsCall;

        public JsCallAttribute(string jsCall) {
            this._jsCall = jsCall;
        }
    }
}
