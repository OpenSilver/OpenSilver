using OpenSilver;

namespace System.Windows.Input;

public partial class MouseEventArgs
{
    private enum JsCall
    {
        [JsCall("void;{sEvent}.doNotReroute = true")]
        DoNotRerouteTrue,
        [JsCall("{sEvent}.doNotReroute == true")]
        TestDoNotReroute,

        [JsCall("{sEvent}.shiftKey || false")]
        ShiftKey,
        [JsCall("{sEvent}.altKey || false")]
        AltKey,
        [JsCall("{sEvent}.ctrlKey || false")]
        CtrlKey,

        [JsCall("{sEvent}.type")]
        GetType,

        [JsCall("{sEvent}.changedTouches[0].pageX + '|' + {sEvent}.changedTouches[0].pageY")]
        GetTouchXY,

        [JsCall("{sEvent}.pageX + '|' + {sEvent}.pageY")]
        GetPageXY,

        [JsCall("({sElement}.getBoundingClientRect().left - document.body.getBoundingClientRect().left) + '|' + ({sElement}.getBoundingClientRect().top - document.body.getBoundingClientRect().top)")]
        SetPointerAbsolutePos,

    }
}