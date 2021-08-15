﻿

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

#if MIGRATION
using System.Windows.Input;
#else
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Displays a message box.
    /// </summary>
    public static class MessageBox
    {
        static Func<string, string, bool, bool> _codeToShowTheMessageBoxWithTitleAndButtons;
        /// <summary>
        /// Intended to be called by the Simulator to inject the code to display a MessageBox with a title and button.
        /// </summary>
        public static Func<string, string, bool, bool> INTERNAL_CodeToShowTheMessageBoxWithTitleAndButtons // Intended to be called by the "Emulator" (Simulator) project to inject the code to display a MessageBox.
        {
            set
            {
                _codeToShowTheMessageBoxWithTitleAndButtons = value;
            }
        }

        /// <summary>
        /// Displays a message box that contains the specified text and an OK button.
        /// </summary>
        /// <param name="msg">A System.String that specifies the text to display.</param>
        /// <returns>System.Windows.MessageBoxResult.OK in all cases.</returns>
        public static MessageBoxResult Show(string msg)
        {
            return Show(msg, "", MessageBoxButton.OK);
        }

        /// <summary>
        /// Displays a message box that contains the specified text, title bar caption, and response buttons.
        /// </summary>
        /// <param name="messageBoxText">The message to display.</param>
        /// <param name="button">A value that indicates the button or buttons to display.</param>
        /// <returns>A value that indicates the user's response to the message.</returns>
        public static MessageBoxResult Show(string messageBoxText, MessageBoxButton button)
        {
            return Show(messageBoxText, "", button);
        }

                /// <summary>
        /// Displays a message box that contains the specified text, title bar caption, and response buttons.
        /// </summary>
        /// <param name="messageBoxText">The message to display.</param>
        /// <param name="caption">The title of the message box.</param>
        /// <returns>System.Windows.MessageBoxResult.OK in all cases.</returns>
        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(messageBoxText, caption, MessageBoxButton.OK);
        }

        /// <summary>
        /// Displays a message box that contains the specified text, title bar caption, and response buttons.
        /// </summary>
        /// <param name="messageBoxText">The message to display.</param>
        /// <param name="caption">The title of the message box.</param>
        /// <param name="button">A value that indicates the button or buttons to display.</param>
        /// <returns>A value that indicates the user's response to the message.</returns>
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            //-----------------
            // Release the Pointer Capture if any
            //-----------------

            // Note: releasing the pointer capture is useful because when we display a MessageBox while the pointer is pressed,
            // the "PointerReleased" event is not called. Therefore, some controls like the "RepeatButton" will think that the user
            // is still pressing the button, and never stops doing so. By releasing the pointer capture, controls like the "RepeatButton"
            // can know that the user is no longer pressing the button by listening to "OnPointerCaptureLost".

            if (Pointer.INTERNAL_captured != null)
            {
#if MIGRATION
                Pointer.INTERNAL_captured.ReleaseMouseCapture();
#else
                Pointer.INTERNAL_captured.ReleasePointerCapture();
#endif
            }

            //todo: raise the "PointerReleased" event as well.


            //-----------------
            // Display the message box
            //-----------------

            if (_codeToShowTheMessageBoxWithTitleAndButtons != null)
            {
                //-----------------
                // Running in the Simulator
                //-----------------

                bool showCancelButton = button == MessageBoxButton.OKCancel;
                bool isOK = _codeToShowTheMessageBoxWithTitleAndButtons(messageBoxText, caption ?? "", showCancelButton);
                return (isOK ? MessageBoxResult.OK : MessageBoxResult.Cancel);
            }
            else
            {
                //-----------------
                // Running in JS/HTML5
                //-----------------

                string messageIncludingTitleIfAny;

                // Concatenate the title and the body, because in HTML the sync message box cannot have a title:
                if (!string.IsNullOrEmpty(caption))
                    messageIncludingTitleIfAny = caption.ToUpper() + Environment.NewLine + Environment.NewLine + messageBoxText + Environment.NewLine;
                else
                    messageIncludingTitleIfAny = Environment.NewLine + messageBoxText + Environment.NewLine;

                bool isOK = true;
                if (button == MessageBoxButton.OK)
                {
                    CSHTML5.Interop.ExecuteJavaScript("alert($0)", messageIncludingTitleIfAny);
                }
                else if (button == MessageBoxButton.OKCancel)
                {
                    isOK = (bool)CSHTML5.Interop.ExecuteJavaScript("confirm($0)", messageIncludingTitleIfAny);
                }
                else
                {
                    throw new NotSupportedException("Buttons other than OK and Cancel are not supported in the MessageBox.");
                }
                return (isOK ? MessageBoxResult.OK : MessageBoxResult.Cancel);
            }
        }
    }
}
