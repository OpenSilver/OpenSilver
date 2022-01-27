

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
    /// Base class for all controls that provide value manipulation with a 
    /// Spinner and a text box.
    /// </summary>
    /// <remarks>
    /// This non generic base class is used to specify default template,
    /// and simulate covariance among sub classes of UpDownBase&lt;T&gt;.
    /// </remarks>
    [OpenSilver.NotImplemented]
    public class UpDownBase : Control
    {

        #region Template Parts Name Constants
        /// <summary>
        /// Name constant for Text template part.
        /// </summary>
        internal const string ElementTextName = "Text";

        /// <summary>
        /// Name constant for Spinner template part.
        /// </summary>
        internal const string ElementSpinnerName = "Spinner";

        /// <summary>
        /// Name constant for SpinnerStyle property.
        /// </summary>
        internal const string SpinnerStyleName = "SpinnerStyle";
        #endregion 

        #region public Style SpinnerStyle
        /// <summary>
        /// Gets or sets the Style that is applied to the spinner.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Style SpinnerStyle
        {
            get => (Style)GetValue(SpinnerStyleProperty);
            set => SetValue(SpinnerStyleProperty, value);
        }

        /// <summary>
        /// Identifies the SpinnerStyle dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SpinnerStyleProperty =
            DependencyProperty.Register(
                "SpinnerStyle",
                typeof(Style),
                typeof(UpDownBase),
                new PropertyMetadata(null, OnSpinnerStylePropertyChanged));

        /// <summary>
        /// Property changed callback for SpinnerStyleProperty.
        /// </summary>
        /// <param name="d">UpDownBase whose SpinnerStyleProperty changed.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSpinnerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion public Style SpinnerStyle
    }
}
