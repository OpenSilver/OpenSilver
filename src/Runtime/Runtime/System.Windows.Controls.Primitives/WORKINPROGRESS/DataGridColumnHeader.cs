

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


#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    public partial class DataGridColumnHeader
    {
        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorBrush" /> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="P:System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorBrush" /> dependency property.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SeparatorBrushProperty = DependencyProperty.Register(nameof(SeparatorBrush), typeof(Brush), typeof(DataGridColumnHeader), null);

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Media.Brush" /> used to paint the column header separator lines.
        /// </summary>
        /// <returns>
        /// The brush used to paint column header separator lines.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Brush SeparatorBrush
        {
            get => GetValue(SeparatorBrushProperty) as Brush;
            set => SetValue(SeparatorBrushProperty, value);
        }
    }
}
