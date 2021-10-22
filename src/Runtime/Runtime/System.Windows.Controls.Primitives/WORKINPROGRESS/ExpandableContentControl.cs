

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
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a control with a single piece of content that expands or
    /// collapses in a sliding motion to a specified desired size.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class ExpandableContentControl : ContentControl
    {
        #region public ExpandDirection RevealMode
        /// <summary>
        /// Gets or sets the direction in which the ExpandableContentControl
        /// content window opens.
        /// </summary>
        [OpenSilver.NotImplemented]
        public ExpandDirection RevealMode
        {
            get => (ExpandDirection)GetValue(RevealModeProperty);
            set => SetValue(RevealModeProperty, value);
        }

        /// <summary>
        /// Identifies the RevealMode dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RevealModeProperty =
            DependencyProperty.Register(
                "RevealMode",
                typeof(ExpandDirection),
                typeof(ExpandableContentControl),
                new PropertyMetadata(ExpandDirection.Down, OnRevealModePropertyChanged));

        /// <summary>
        /// RevealModeProperty property changed handler.
        /// </summary>
        /// <param name="d">ExpandableContentControl that changed its RevealMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRevealModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion public ExpandDirection RevealMode

        #region public double Percentage
        /// <summary>
        /// Gets or sets the relative percentage of the content that is
        /// currently visible. A percentage of 1 corresponds to the complete
        /// TargetSize.
        /// </summary>
        [OpenSilver.NotImplemented]
        public double Percentage
        {
            get => (double)GetValue(PercentageProperty);
            set => SetValue(PercentageProperty, value);
        }

        /// <summary>
        /// Identifies the Percentage dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register(
                "Percentage",
                typeof(double),
                typeof(ExpandableContentControl),
                new PropertyMetadata(0.0, OnPercentagePropertyChanged));

        /// <summary>
        /// PercentageProperty property changed handler.
        /// </summary>
        /// <param name="d">Page that changed its Percentage.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnPercentagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion public double Percentage
    }
}
