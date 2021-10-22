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

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public sealed partial class SkewTransform : Transform
	{
		/// <summary>
		/// Identifies the <see cref="SkewTransform.CenterX"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty CenterXProperty =
			DependencyProperty.Register(
				nameof(CenterX),
				typeof(double),
				typeof(SkewTransform),
				new PropertyMetadata(0.0));

		/// <summary>
		/// Gets or sets the x-coordinate of the transform center.
		/// The default is 0.
		/// </summary>
        [OpenSilver.NotImplemented]
		public double CenterX
		{
			get => (double)GetValue(CenterXProperty);
			set => SetValue(CenterXProperty, value);
        }

		/// <summary>
		/// Identifies the <see cref="SkewTransform.CenterY"/> dependency property.
		/// </summary>
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty CenterYProperty =
			DependencyProperty.Register(
				nameof(CenterY),
				typeof(double),
				typeof(SkewTransform),
				new PropertyMetadata(0.0));

		/// <summary>
		/// Gets or sets the y-coordinate of the transform center.
		/// The default is 0.
		/// </summary>
        [OpenSilver.NotImplemented]
		public double CenterY
		{
			get => (double)GetValue(CenterYProperty);
			set => SetValue(CenterYProperty, value);
        }
	}
}
