#if WORKINPROGRESS

using System.Collections.Generic;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Linq;
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
        
        public Rect VisualBounds { get; protected set; }

        public bool IsMeasureValid { get; private set; }
        public bool IsArrangeValid { get; private set; }

        public Rect PreviousFinalRect { get; private set; }
        public Size PreviousAvailableSize { get; private set; }
        private Size previousDesiredSize;
        private int disableMeasureInvalidationRequests;
        private IDisposable disableMeasureInvalidationToken;
        private int visualLevel;
        public int VisualLevel
        {
            get
            {
                if (visualLevel == -1)
                {
                    visualLevel = (INTERNAL_VisualParent as UIElement) != null ? (INTERNAL_VisualParent as UIElement).VisualLevel + 1 : 0;
                }

                return visualLevel;
            }
        }
        public UIElement()
        {
            DesiredSize = Size.Zero;
            PreviousFinalRect = Rect.Empty;
            PreviousAvailableSize = Size.Infinity;
            previousDesiredSize = Size.Empty;
            IsMeasureValid = false;
            IsArrangeValid = false;
            visualLevel = -1;

            disableMeasureInvalidationToken = new Disposable(() => disableMeasureInvalidationRequests--);
        }
        private IDisposable DisableMeasureInvalidation()
        {
            disableMeasureInvalidationRequests++;
            return disableMeasureInvalidationToken;
        }
        internal void RaiseLayoutUpdated()
        {
            OnLayoutUpdated();
        }
        protected virtual void OnLayoutUpdated()
        {
            //
        }
        public void Arrange(Rect finalRect)
        {
            using (System.Windows.Threading.Dispatcher.INTERNAL_GetCurrentDispatcher().DisableProcessing())
            {
                using (DisableMeasureInvalidation())
                {
                    if (Visibility != Visibility.Visible || IsArrangeValid && finalRect.IsClose(PreviousFinalRect))
                    {
                        //Console.WriteLine($"Arrange previousFinalRect {this}");
                        LayoutManager.Current.RemoveArrange(this);
                        return;
                    }

                    if (!IsMeasureValid)
                    {
                        Measure(finalRect.Size);
                    }

                    ArrangeCore(finalRect);

                    PreviousFinalRect = finalRect;

                    IsArrangeValid = true;

                    // Render with new size & location
                    Render();

                    LayoutManager.Current.RemoveArrange(this);
                    LayoutManager.Current.AddUpdatedElement(this);
                }
            }
        }

        private void Render()
        {
            if (this as Window == null)
            {
                INTERNAL_HtmlDomStyleReference uiStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification((INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement);
                uiStyle.position = "absolute";
                uiStyle.left = $"{VisualBounds.Left}px";
                uiStyle.top = $"{VisualBounds.Top}px";
                uiStyle.width = $"{VisualBounds.Width}px";
                uiStyle.height = $"{VisualBounds.Height}px";
                uiStyle.marginLeft = "0";
                uiStyle.marginTop = "0";
                uiStyle.marginRight = "0";
                uiStyle.marginBottom = "0";

                if (this.INTERNAL_AdditionalOutsideDivForMargins != null && this.INTERNAL_AdditionalOutsideDivForMargins != this.INTERNAL_OuterDomElement)
                {
                    //INTERNAL_HtmlDomElementReference domElementForMargin = (INTERNAL_HtmlDomElementReference)this.INTERNAL_AdditionalOutsideDivForMargins;
                    //Console.WriteLine($"Set {domElementForMargin.UniqueIdentifier} padding&margin 0");
                    INTERNAL_HtmlDomStyleReference uiMarginStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification((INTERNAL_HtmlDomElementReference)this.INTERNAL_AdditionalOutsideDivForMargins);
                    uiMarginStyle.padding = "0";
                    uiMarginStyle.marginLeft = "0";
                    uiMarginStyle.marginTop = "0";
                    uiMarginStyle.marginRight = "0";
                    uiMarginStyle.marginBottom = "0";
                    uiMarginStyle.position = "";    // FOR Grid
                    uiMarginStyle.gridArea = "";    // FOR Grid
                }
            }
        }

        protected virtual void ArrangeCore(Rect finalRect)
        {

        }

        protected virtual Size MeasureCore(Size availableSize)
        {
            return Size.Empty;
        }

        public void Measure(Size availableSize)
        {
            using (System.Windows.Threading.Dispatcher.INTERNAL_GetCurrentDispatcher().DisableProcessing())
            {
                using (DisableMeasureInvalidation())
                {
                    if (Visibility == Visibility.Collapsed)
                    {
                        DesiredSize = Size.Zero;
                    }
                    else if (IsMeasureValid && PreviousAvailableSize.IsClose(availableSize))
                    {
                        //Console.WriteLine($"Measure previousDesiredSize {this}");
                        DesiredSize = previousDesiredSize;
                    }
                    else
                    {
                        DesiredSize = MeasureCore(availableSize);

                        PreviousAvailableSize = availableSize;
                        previousDesiredSize = DesiredSize;
                    }

                    IsMeasureValid = true;
                    LayoutManager.Current.RemoveMeasure(this);

                    INTERNAL_HtmlDomElementReference domElementReference = (INTERNAL_HtmlDomElementReference)this.INTERNAL_OuterDomElement;
                    //Console.WriteLine($"UIElemet {domElementReference.UniqueIdentifier}, DesiredSize: {DesiredSize.Width}, {DesiredSize.Height}");
                }
            }
        }

        public void InvalidateArrange()
        {
            if (!IsArrangeValid)
            {
                return;
            }

            IsArrangeValid = false;

            LayoutManager.Current.AddArrange(this);
        }
        public void InvalidateParentMeasure()
        {
            if (INTERNAL_VisualParent as UIElement != null)
            {
                (INTERNAL_VisualParent as UIElement).InvalidateMeasure();
            }
        }

        public void InvalidateMeasure()
        {
            if (disableMeasureInvalidationRequests > 0 || !IsMeasureValid)
            {
                return;
            }

            IsMeasureValid = false;

            LayoutManager.Current.AddMeasure(this);
        }

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