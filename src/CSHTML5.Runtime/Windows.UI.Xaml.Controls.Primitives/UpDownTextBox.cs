
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