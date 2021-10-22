

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
    public partial class NumericUpDown
    {
        #region DecimalPlaces
        /// <summary>
        /// Gets or sets the number of decimal places that are displayed in the 
        /// NumericUpDown. 
        /// </summary>
        /// <remarks>
        /// The default value is zero.
        /// 
        /// DecimalPlaces decides output format of Value property.
        /// It is implemented via formatString field and FormatValue override.
        /// </remarks>
        [OpenSilver.NotImplemented]
        public int DecimalPlaces
        {
            get => (int)GetValue(DecimalPlacesProperty);
            set => SetValue(DecimalPlacesProperty, value);
        }

        /// <summary>
        /// Identifies the DecimalPlaces dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(
                "DecimalPlaces",
                typeof(int),
                typeof(NumericUpDown),
                new PropertyMetadata(0, OnDecimalPlacesPropertyChanged));

        /// <summary>
        /// DecimalPlacesProperty property changed handler.
        /// </summary>
        /// <param name="d">NumericUpDown that changed its DecimalPlaces.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs for DecimalPlaces property.</param>
        private static void OnDecimalPlacesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion DecimalPlaces


    }
}
