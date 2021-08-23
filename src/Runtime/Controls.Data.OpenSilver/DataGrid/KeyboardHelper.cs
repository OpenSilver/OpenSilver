// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.


#if MIGRATION
using System.Windows.Input;
#else
using Windows.UI.Xaml.Input;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal static class KeyboardHelper
    {
        public static void GetMetaKeyState(out bool ctrl, out bool shift)
        {
            ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

            // The Apple key on a Mac is supposed to behave like the CTRL key on a PC for
            // things like multi-select, select-all, and grid navigation.  To allow for this,
            // we set the CTRL to true if the Apple key is pressed.
            ctrl |= (Keyboard.Modifiers & ModifierKeys.Apple) == ModifierKeys.Apple;

            shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
        }

        public static void GetMetaKeyState(out bool ctrl, out bool shift, out bool alt)
        {
            GetMetaKeyState(out ctrl, out shift);
            alt = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
        }        
    }
}
