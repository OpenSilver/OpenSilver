using OpenSilver;

namespace CSHTML5.Internal;

public partial class INTERNAL_Html2dContextReference
{
    private enum JsCall
    {
        [JsCall("void;document.set2dContextProperty({_id},{propertyName},{propertyValue});")]
        SetPropertyValue,

        [JsCall("void;document.invoke2dContextMethod({_id}, {methodName}, {args});")]
        InvokeMethod,
    }
}