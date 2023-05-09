

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


//#define CHECK_THAT_ID_EXISTS 
//#define PERFORMANCE_ANALYSIS

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using OpenSilver.Internal;

namespace CSHTML5.Internal
{
    // Note: this class is intented to be used by the Simulator only, not when compiled to JavaScript.
    public class INTERNAL_Html2dContextReference : DynamicObject
    {
        private readonly string _id;

        // Note: It's important that the constructor stays Private because we need to recycle the instances that correspond to the same ID using the "GetInstance" public static method, so that each ID always corresponds to the same instance. This is useful to ensure that private fields such as "_display" work propertly.
        internal INTERNAL_Html2dContextReference(string elementId)
        {
            _id = elementId;
        }

        [Obsolete("This method is not meant to be used outside of OpenSilver.")]
        public static INTERNAL_Html2dContextReference GetInstance(string elementId)
            => new INTERNAL_Html2dContextReference(elementId);

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) => throw new NotSupportedException();

        public override bool TrySetMember(SetMemberBinder binder, object value) => throw new NotSupportedException();

        void SetPropertyValue(string propertyName, string propertyValue)
            => OpenSilver.Interop.ExecuteJavaScriptFastAsync($"document.set2dContextProperty(\"{_id}\",\"{propertyName}\",\"{propertyValue}\");");

        void InvokeMethod(string methodName, object[] args)
            => InvokeMethodImpl(methodName, string.Join(", ", args.Select(x => INTERNAL_HtmlDomManager.ConvertToStringToUseInJavaScriptCode(x))));

        void InvokeMethod(string methodName) => InvokeMethodImpl(methodName, string.Empty);

        void InvokeMethod(string methodName, double arg0) => InvokeMethodImpl(methodName, arg0.ToInvariantString());

        void InvokeMethod(string methodName, double arg0, double arg1)
            => InvokeMethodImpl(methodName, $"{arg0.ToInvariantString()}, {arg1.ToInvariantString()}");

        void InvokeMethod(string methodName, double arg0, double arg1, double arg2, double arg3)
            => InvokeMethodImpl(methodName, $"{arg0.ToInvariantString()}, {arg1.ToInvariantString()}, {arg2.ToInvariantString()}, {arg3.ToInvariantString()}");

        void InvokeMethod(string methodName, double arg0, double arg1, double arg2, double arg3, double arg4, double arg5)
            => InvokeMethodImpl(methodName, $"{arg0.ToInvariantString()}, {arg1.ToInvariantString()}, {arg2.ToInvariantString()}, {arg3.ToInvariantString()}, {arg4.ToInvariantString()}, {arg5.ToInvariantString()}");

        void InvokeMethod(string methodName, double[] args)
            => InvokeMethodImpl(methodName, string.Join(", ", args.Select(x => x.ToInvariantString())));

        void InvokeMethodImpl(string methodName, string args)
            => OpenSilver.Interop.ExecuteJavaScriptFastAsync($"document.invoke2dContextMethod(\"{_id}\", \"{methodName}\", \"{args}\");");

#pragma warning disable IDE1006 // Naming Styles
        public string fillStyle { set { SetPropertyValue("fillStyle", value); } }
        public string strokeStyle { set { SetPropertyValue("strokeStyle", value); } }
        public string lineWidth { set { SetPropertyValue("lineWidth", value); } }
        public string lineDashOffset { set { SetPropertyValue("lineDashOffset", value); } }


        public void transform(double m11, double m12, double m21, double m22, double dx, double dy) { InvokeMethod("transform", m11, m12, m21, m22, dx, dy); }
        public void translate(double x, double y) { InvokeMethod("translate", x, y); }
        public void rotate(double angle) { InvokeMethod("rotate", angle); }
        public void scale(double x, double y) { InvokeMethod("scale", x, y); }

        public void save() { InvokeMethod("save"); }
        public void restore() { InvokeMethod("restore"); }

        public void fill(params object[] args) { InvokeMethod("fill", args); }
        public void stroke(params object[] args) { InvokeMethod("stroke", args); }
        public void setLineDash(params double[] args) { InvokeMethod("setLineDash", args); }

        public void beginPath() { InvokeMethod("beginPath"); }
        public void closePath() { InvokeMethod("closePath"); }
        public void createLinearGradient(double x0, double y0, double x1, double y1) { InvokeMethod("createLinearGradient", x0, y0, x1, y1); }

        public void arc(double centerX, double centerY, double radius, double startAngle, double endAngle, bool counterClockwise = false)
        {
            OpenSilver.Interop.ExecuteJavaScriptFastAsync(@$"
document.getElementById('{_id}').getContext('2d').arc({centerX.ToInvariantString()}, {centerY.ToInvariantString()}, {radius.ToInvariantString()}, 
{startAngle.ToInvariantString()}, {endAngle.ToInvariantString()}, {INTERNAL_HtmlDomManager.ConvertToStringToUseInJavaScriptCode(counterClockwise)});");
        }

        public void ellipse(params object[] args) { InvokeMethod("ellipse", args); }
        public void rect(double x, double y, double width, double height) { InvokeMethod("rect", x, y, width, height); }

        public void moveTo(double x, double y) { InvokeMethod("moveTo", x, y); }
        public void lineTo(double x, double y) { InvokeMethod("lineTo", x, y); }
        public void bezierCurveTo(double cp1x, double cp1y, double cp2x, double cp2y, double x, double y) { InvokeMethod("bezierCurveTo", cp1x, cp1y, cp2x, cp2y, x, y); }
        public void quadraticCurveTo(double cpx, double cpy, double x, double y) { InvokeMethod("quadraticCurveTo", cpx, cpy, x, y); }
#pragma warning restore IDE1006 // Naming Styles
    }
}
