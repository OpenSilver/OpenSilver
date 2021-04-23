﻿#if WORKINPROGRESS

using System.Collections.Generic;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

#if OPENSILVER
#if MIGRATION
using System.Windows.Automation.Peers;
#else
using Windows.UI.Xaml.Automation.Peers;
#endif
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public abstract partial class UIElement
    {
		[OpenSilver.NotImplemented]
        public event DragEventHandler DragEnter;
		
		[OpenSilver.NotImplemented]
		public event DragEventHandler DragLeave;
		
		[OpenSilver.NotImplemented]
		public event DragEventHandler Drop;

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



		[OpenSilver.NotImplemented]
        public Geometry Clip
        {
            get { return (Geometry)GetValue(ClipProperty); }
            set { SetValue(ClipProperty, value); }
        }

		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty ClipProperty = 
            DependencyProperty.Register("Clip", 
                                        typeof(Geometry), 
                                        typeof(UIElement), 
                                        null);

		[OpenSilver.NotImplemented]
        public Size DesiredSize { get; private set; }
		[OpenSilver.NotImplemented]
        public Size RenderSize { get; private set; }

		[OpenSilver.NotImplemented]
        public void Arrange(Rect finalRect)
        {

        }

		[OpenSilver.NotImplemented]
        public void Measure(Size availableSize)
        {

        }

		[OpenSilver.NotImplemented]
        public void InvalidateArrange()
        {

        }

		[OpenSilver.NotImplemented]
        public void InvalidateMeasure()
        {

        }

		[OpenSilver.NotImplemented]
        public void UpdateLayout()
        {

        }

#if OPENSILVER
		//
		// Summary:
		//     When implemented in a derived class, returns class-specific System.Windows.Automation.Peers.AutomationPeer
		//     implementations for the Silverlight automation infrastructure.
		//
		// Returns:
		//     The class-specific System.Windows.Automation.Peers.AutomationPeer subclass to
		//     return.
		[OpenSilver.NotImplemented]
		protected virtual AutomationPeer OnCreateAutomationPeer()
		{
			return default(AutomationPeer);
		}
#endif
    }
}

#endif