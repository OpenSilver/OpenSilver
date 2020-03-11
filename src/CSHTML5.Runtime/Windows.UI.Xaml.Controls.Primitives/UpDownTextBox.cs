

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
using Windows.System;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Primitive control - TextBox with UpPressed and DownPressed events for use in a NumericUpDown
    /// to make up and down keys work to increment and decrement the values.
    /// </summary>
    public partial class UpDownTextBox : TextBox
    {
        #region UpPressed event
        /// <summary>
        /// UpPressed event property.
        /// </summary>
        public event EventHandler UpPressed;

        /// <summary>
        /// Raises UpPressed event.
        /// </summary>
        private void RaiseUpPressed()
        {
            var handler = this.UpPressed;

            if (handler != null)
            {
                var args = EventArgs.Empty;
                handler(this, args);
            }
        }
        #endregion

        #region DownPressed event
        /// <summary>
        /// DownPressed event property.
        /// </summary>
        public event EventHandler DownPressed;

        /// <summary>
        /// Raises DownPressed event.
        /// </summary>
        private void RaiseDownPressed()
        {
            var handler = this.DownPressed;

            if (handler != null)
            {
                var args = EventArgs.Empty;
                handler(this, args);
            }
        }
        #endregion

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
#if MIGRATION
        protected override void OnKeyDown(KeyEventArgs e)
#else
        protected override void OnKeyDown(KeyRoutedEventArgs e)
#endif
        {
            // Overriding OnKeyDown seems to be the only way to prevent selection to change when you press these keys.
#if MIGRATION
            if (e.Key == Key.Up)
#else
            if (e.Key == VirtualKey.Up)
#endif
            {
                this.RaiseUpPressed();
                e.Handled = true;
                return;
            }

#if MIGRATION
            if (e.Key == Key.Down)
#else
            if (e.Key == VirtualKey.Down)
#endif
            {
                this.RaiseDownPressed();
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }
    }
}