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

#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
	public sealed partial class CompositeTransform : Transform
	{
		/// <summary>
		/// Identifies the <see cref="CompositeTransform.CenterX"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty CenterXProperty =
			DependencyProperty.Register(
				nameof(CenterX),
				typeof(double),
				typeof(CompositeTransform),
				new PropertyMetadata(0.0));

		/// <summary>
		/// Gets or sets the x-coordinate of the center point for all transforms specified
		/// by the <see cref="CompositeTransform"/>.
		/// </summary>
		public double CenterX
        {
			get => (double)GetValue(CenterXProperty);
			set => SetValue(CenterXProperty, value);
        }

		/// <summary>
		/// Identifies the <see cref="CompositeTransform.CenterY"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty CenterYProperty =
			DependencyProperty.Register(
				nameof(CenterY),
				typeof(double),
				typeof(CompositeTransform),
				new PropertyMetadata(0.0));

		/// <summary>
		/// Gets or sets the y-coordinate of the center point for all transforms specified
		/// by the <see cref="CompositeTransform"/>.
		/// </summary>
		public double CenterY
        {
			get => (double)GetValue(CenterYProperty);
			set => SetValue(CenterYProperty, value);
        }
	}
}
#endif