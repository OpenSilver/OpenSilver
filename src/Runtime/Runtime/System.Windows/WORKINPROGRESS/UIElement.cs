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
            get { return (CacheMode)GetValue(CacheModeProperty); }
            set { SetValueInternal(CacheModeProperty, value); }
        }

        /// <summary>Identifies the <see cref="CacheMode" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="CacheMode" /> dependency property.</returns>
		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty CacheModeProperty =
            DependencyProperty.Register(nameof(CacheMode), 
                                        typeof(CacheMode), 
                                        typeof(UIElement), 
                                        null);

		[OpenSilver.NotImplemented]
        public Projection Projection
        {
            get { return (Projection)GetValue(ProjectionProperty); }
            set { SetValueInternal(ProjectionProperty, value); }
        }

		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty ProjectionProperty =
            DependencyProperty.Register(nameof(Projection), 
                                        typeof(Projection), 
                                        typeof(UIElement), 
                                        null);
    }
}
