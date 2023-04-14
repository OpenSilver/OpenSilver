
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

using System;
using System.Diagnostics;

#if MIGRATION
using System.Windows.Input;
#else
using Windows.UI.Xaml.Input;
using Key = Windows.System.VirtualKey;
using ModifierKeys = Windows.System.VirtualKeyModifiers;
using KeyEventArgs = Windows.UI.Xaml.Input.KeyRoutedEventArgs;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class TextBox
    {
        private void HandleKeyDown(KeyEventArgs e)
        {
            bool handled = false;

            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Down:
                case Key.Up:
                    handled = NavigateInDirection(e);
                    break;

                case Key.PageDown:
                case Key.PageUp:
                    handled = NavigateByPage(e);
                    break;

                case Key.Home:
                    handled = NavigateToStart(e);
                    break;

                case Key.End:
                    handled = NavigateToEnd(e);
                    break;

                case Key.Delete:
                    handled = CaretPosition < _textViewHost.View.GetText().Length || SelectionLength > 0;
                    break;

                case Key.Back:
                    handled = CaretPosition > 0 || SelectionLength > 0;
                    break;

                case Key.C:
                case Key.X:
                    handled = IsCtrlDown(e) && SelectionLength > 0;
                    break;

                case Key.A:
                    handled = IsCtrlDown(e) && SelectionLength < _textViewHost.View.GetText().Length;
                    break;

                case Key.V:
                case Key.Y:
                case Key.Z:
                    handled = IsCtrlDown(e);
                    break;
            }

            if (handled)
            {
                e.Handled = true;
                e.Cancellable = false;
            }
        }

        private bool NavigateInDirection(KeyEventArgs e)
        {
            if (!IsShiftDown(e) && !IsCtrlDown(e) && SelectionLength > 0)
            {
                return true;
            }

            switch (e.Key)
            {
                case Key.Left:
                case Key.Up:
                    return CaretPosition > 0;

                case Key.Right:
                case Key.Down:
                    return CaretPosition < _textViewHost.View.GetText().Length;

                default:
                    Debug.Assert(false);
                    return false;
            }
        }

        private bool NavigateByPage(KeyEventArgs e)
        {
            if (IsCtrlDown(e))
            {
                return false;
            }

            bool forward = e.Key == Key.PageDown;
            if (!IsShiftDown(e) && SelectionLength > 0)
            {
                Select(forward ? _textViewHost.View.GetText().Length : 0, 0);
                return true;
            }

            return forward ? (CaretPosition < _textViewHost.View.GetText().Length) : (CaretPosition > 0);
        }

        private bool NavigateToStart(KeyEventArgs e)
        {
            if (!IsShiftDown(e) && SelectionLength > 0)
            {
                return true;
            }

            if (IsCtrlDown(e))
            {
                return CaretPosition > 0;
            }
            else
            {
                int caretIndex = CaretPosition;
                if (caretIndex > 0)
                {
                    string text = _textViewHost.View.GetText();
                    return !IsNewLine(text[caretIndex - 1]);
                }

                return false;
            }
        }

        private bool NavigateToEnd(KeyEventArgs e)
        {
            if (!IsShiftDown(e) && SelectionLength > 0)
            {
                return true;
            }

            if (IsCtrlDown(e))
            {
                return CaretPosition < _textViewHost.View.GetText().Length;
            }
            else
            {
                int caretIndex = CaretPosition;
                string text = _textViewHost.View.GetText();
                if (caretIndex < text.Length)
                {
                    return !IsNewLine(text[caretIndex]);
                }

                return false;
            }
        }

        private static bool IsNewLine(char c)
        {
            return c == '\n' || c == '\r';
        }

        private static bool IsCtrlDown(KeyEventArgs e)
            => (e.KeyModifiers & ModifierKeys.Control) == ModifierKeys.Control;

        private static bool IsShiftDown(KeyEventArgs e)
            => (e.KeyModifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
    }
}
