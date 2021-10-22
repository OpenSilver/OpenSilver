

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Base class for all controls that have popup functionality.
    /// </summary>
    [OpenSilver.NotImplemented]
    public abstract class Picker : Control, IUpdateVisualState
    {
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            throw new NotImplementedException();
        }

        #region public ClickMode PopupButtonMode
        /// <summary>
        /// Gets or sets the button event that causes the popup portion of the 
        /// Picker control to open.
        /// </summary>
        [OpenSilver.NotImplemented]
        public ClickMode PopupButtonMode
        {
            get => (ClickMode)GetValue(PopupButtonModeProperty);
            set => SetValue(PopupButtonModeProperty, value);
        }

        /// <summary>
        /// Identifies the PopupButtonMode dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PopupButtonModeProperty =
            DependencyProperty.Register(
                "PopupButtonMode",
                typeof(ClickMode),
                typeof(Picker),
                new PropertyMetadata(ClickMode.Release, OnPopupButtonModePropertyChanged));

        /// <summary>
        /// PopupButtonModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Picker that changed its PopupButtonMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPopupButtonModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion public ClickMode PopupButtonMode
    }
}
