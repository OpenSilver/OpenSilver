using System.Windows.Media;

namespace System.Windows
{
    public abstract partial class UIElement
    {
		[OpenSilver.NotImplemented]
        public event DragEventHandler DragEnter;
		
		[OpenSilver.NotImplemented]
		public event DragEventHandler DragLeave;
		
		[OpenSilver.NotImplemented]
		public event DragEventHandler Drop;

        [OpenSilver.NotImplemented]
        public event DragEventHandler DragOver;

        /// <summary>
        /// Gets or sets the brush used to alter the opacity of 
        /// regions of this object.
        /// </summary>
        /// <returns>
        /// A brush that describes the opacity applied to this 
        /// object. The default is null.
        /// </returns>
		[OpenSilver.NotImplemented]
        public Brush OpacityMask
        {
            get { return (Brush)GetValue(OpacityMaskProperty); }
            set { SetValue(OpacityMaskProperty, value); }
        }

        /// <summary>
        /// Identifies the OpacityMask dependency property.
        /// </summary>
		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty OpacityMaskProperty = 
            DependencyProperty.Register("OpacityMask", 
                                        typeof(Brush), 
                                        typeof(UIElement), 
                                        null);

        /// <summary>
        /// Gets or sets a value that indicates that rendered 
        /// content should be cached when possible.
        /// </summary>
        /// <returns>
        /// A value that indicates that rendered content should be 
        /// cached when possible. If you specify a value of 
        /// <see cref="Media.CacheMode" />, rendering operations from 
        /// <see cref="UIElement.RenderTransform" /> and 
        /// <see cref="UIElement.Opacity" /> execute on the graphics 
        /// processing unit (GPU), if available. The default is null, 
        /// which does not enable a cached composition mode. 
        /// </returns>
		[OpenSilver.NotImplemented]
        public CacheMode CacheMode
        {
            get { return (CacheMode)this.GetValue(UIElement.CacheModeProperty); }
            set { this.SetValue(UIElement.CacheModeProperty, (DependencyObject)value); }
        }

        /// <summary>Identifies the <see cref="UIElement.CacheMode" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="UIElement.CacheMode" /> dependency property.</returns>
		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty CacheModeProperty =
            DependencyProperty.Register("CacheMode", 
                                        typeof(CacheMode), 
                                        typeof(UIElement), 
                                        null);

		[OpenSilver.NotImplemented]
        public Projection Projection
        {
            get { return (Projection)this.GetValue(UIElement.ProjectionProperty); }
            set { this.SetValue(UIElement.ProjectionProperty, value); }
        }

		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty ProjectionProperty =
            DependencyProperty.Register("Projection", 
                                        typeof(Projection), 
                                        typeof(UIElement), 
                                        null);
    }
}
