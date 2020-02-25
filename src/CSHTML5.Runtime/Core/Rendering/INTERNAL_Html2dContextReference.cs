﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



//#define CHECK_THAT_ID_EXISTS 
//#define PERFORMANCE_ANALYSIS

#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSHTML5.Internal
{
    // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.
#if BRIDGE
    [External] //we exclude this class
#else
    [JSIgnore]
#endif

#if CSHTML5NETSTANDARD
    public class INTERNAL_Html2dContextReference : DynamicObject
#else
    internal class INTERNAL_Html2dContextReference : DynamicObject
#endif
    {
        static Dictionary<string, INTERNAL_Html2dContextReference> IdToInstance = new Dictionary<string, INTERNAL_Html2dContextReference>();

        public static INTERNAL_Html2dContextReference GetInstance(string elementId)
        {
            if (IdToInstance.ContainsKey(elementId))
                return IdToInstance[elementId];
            else
            {
                var newInstance = new INTERNAL_Html2dContextReference(elementId);
                IdToInstance[elementId] = newInstance;
                return newInstance;
            }
        }

        string _domElementUniqueIdentifier;

        // Note: It's important that the constructor stays Private because we need to recycle the instances that correspond to the same ID using the "GetInstance" public static method, so that each ID always corresponds to the same instance. This is useful to ensure that private fields such as "_display" work propertly.
        private INTERNAL_Html2dContextReference(string elementId)
        {
            _domElementUniqueIdentifier = elementId;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            string methodName = binder.Name;
            result = InvokeMethod(methodName, args);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string propertyName = binder.Name;
            string propertyValue = (string)value;
            SetPropertyValue(propertyName, propertyValue);
            return true;
        }

        void SetPropertyValue(string propertyName, string propertyValue)
        {
            string javaScriptCodeToExecute = "var element = document.getElementById(\"" + _domElementUniqueIdentifier + "\");if (element) { element.getContext('2d')." + propertyName + " = \"" + propertyValue + "\"; } ";
            //INTERNAL_SimulatorPerformanceOptimizer.QueueJavaScriptCode(javaScriptCodeToExecute);
            INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
        }

        object InvokeMethod(string methodName, object[] args)
        {
            string methodArgumentsFormattedForJavaScript = string.Join(", ", args.Select(x => INTERNAL_HtmlDomManager.ConvertToStringToUseInJavaScriptCode(x)));
            string javaScriptCodeToExecute =
                string.Format(@"
var element = document.getElementById(""{0}"");
var returnValue;
if (element) {{
    returnValue = element.getContext('2d')[""{1}""]({2});
}} 
returnValue;", _domElementUniqueIdentifier, methodName, methodArgumentsFormattedForJavaScript);
            var result = CSHTML5.Interop.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
            return result;
            //INTERNAL_SimulatorPerformanceOptimizer.QueueJavaScriptCode(javaScriptCodeToExecute);
            //result = null;
        }

        public string fillStyle { set { SetPropertyValue("fillStyle", value); } }
        public string strokeStyle { set { SetPropertyValue("strokeStyle", value); } }
        public string lineWidth { set { SetPropertyValue("lineWidth", value); } }


        public void translate(params object[] args) { InvokeMethod("translate", args); }
        public void rotate(params object[] args) { InvokeMethod("rotate", args); }
        public void scale(params object[] args) { InvokeMethod("scale", args); }

        public void save(params object[] args) { InvokeMethod("save", args); }
        public void restore(params object[] args) { InvokeMethod("restore", args); }

        public void fill(params object[] args) { InvokeMethod("fill", args); }
        public void stroke(params object[] args) { InvokeMethod("stroke", args); }
        public void setLineDash(params object[] args) { InvokeMethod("setLineDash", args); }

        public void beginPath(params object[] args) { InvokeMethod("beginPath", args); }
        public void closePath(params object[] args) { InvokeMethod("closePath", args); }
        public void createLinearGradient(params object[] args) { InvokeMethod("createLinearGradient", args); }

        public void arc(params object[] args) { InvokeMethod("arc", args); }
        public void rect(params object[] args) { InvokeMethod("rect", args); }

        public void moveTo(params object[] args) { InvokeMethod("moveTo", args); }
        public void lineTo(params object[] args) { InvokeMethod("lineTo", args); }
        public void bezierCurveTo(params object[] args) { InvokeMethod("bezierCurveTo", args); }
        public void quadraticCurveTo(params object[] args) { InvokeMethod("quadraticCurveTo", args); }
    }
}
