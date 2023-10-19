
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

using System.Windows.Input;

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Primitive control - TextBox with UpPressed and DownPressed events for use in a NumericUpDown
    /// to make up and down keys work to increment and decrement the values.
    /// </summary>
    public class UpDownTextBox : TextBox
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
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Overriding OnKeyDown seems to be the only way to prevent selection to change when you press these keys.
            if (e.Key == Key.Up)
            {
                this.RaiseUpPressed();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Down)
            {
                this.RaiseDownPressed();
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }
    }
}