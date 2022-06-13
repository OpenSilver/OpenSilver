

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
    /// Represents a control that displays a header and has a collapsible
    /// content window.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class AccordionItem : HeaderedContentControl, IUpdateVisualState
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

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty AccordionButtonStyleProperty =
            DependencyProperty.Register("AccordionButtonStyle", typeof(Style), typeof(AccordionItem), new PropertyMetadata());

        [OpenSilver.NotImplemented]
        public Style AccordionButtonStyle 
        {
            get { return (Style)GetValue(AccordionButtonStyleProperty); }
            set { SetValue(AccordionButtonStyleProperty, value); }
        }


        [OpenSilver.NotImplemented]
        public bool IsSelected { get; set; }

        /// <summary>
        /// Occurs when the accordionItem is selected.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event RoutedEventHandler Selected;
    }
}
