using OpenSilver;

namespace System.Windows.Input;

internal sealed partial class InputManager
{
    private enum JsCall
    {
        [JsCall("void;document.inputManager.addListeners({sOuter}, {isFocusable,b});")]
        AddEventListeners,
    }
}