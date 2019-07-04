
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Diagnostics;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endif

//-----------------------------------------------------------------------------------------------
// Credits: Silverlight 3 Toolkit + Nick Lightfoot contribution (cf. CSHTML5 ZenDesk ticket #575)
//-----------------------------------------------------------------------------------------------

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines a content decorator that can stretch and scale a single child to
    /// fill the available space.
    /// </summary>
    [ContentProperty("Child")]
    public class Viewbox : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.Viewbox" /> class.
        /// </summary>
        public Viewbox()
        {
            // Set default style:
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(Viewbox);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultViewboxStyle.GetDefaultStyle());
#endif

            this.Loaded += OnLoaded;
            this.SizeChanged += OnSizeChanged;

            // Load the default template
            //Template = DefaultTemplate = XamlReader.Load(DefaultTemplateMarkup) as ControlTemplate;
            //ApplyTemplate();
            //IsTabStop = false;         
        }

        /// <summary>
        /// Gets or sets the element of the Viewbox that will render the child.
        /// </summary>
        private ContentPresenter ChildElement { get; set; }

        /// <summary>
        /// Gets or sets the Canvas element so that the child does not force its size on the parent.
        /// </summary>
        private Canvas RootCanvas { get; set; }

        /// <summary>
        /// Gets or sets the transformation on the ChildElement used to scale the
        /// Child content.
        /// </summary>
        private ScaleTransform Scale { get; set; }

        /// <summary>
        /// Gets or sets the single child element of a Viewbox element.
        /// </summary>
        /// <value>
        /// The single child element of a Viewbox element.
        /// </value>
        /// <remarks>
        /// Child must be an alias of ContentControl.Content property to ensure 
        /// continuous namescope, ie, named element within Viewbox can be found.
        /// </remarks>
        public UIElement Child
        {
            get { return Content as UIElement; }
            set { Content = value; }
        }

        /// <summary>
        /// Gets or sets the Stretch mode, which determines how content
        /// fits into the available space. The default is Stretch.Uniform.
        /// </summary>
        /// <value>
        /// A Stretch mode, which determines how content fits in the
        /// available space. The default is Stretch.Uniform.
        /// </value>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        /// <summary>
        /// Identifies the Viewbox.Stretch dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the Viewbox.Stretch dependency
        /// property.
        /// </value>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                "Stretch",
                typeof(Stretch),
                typeof(Viewbox),
                new PropertyMetadata(Stretch.Uniform));

        /// <summary>
        /// Gets or sets the StretchDirection, which determines how scaling
        /// is applied to the contents of a Viewbox. The default is
        /// StretchDirection.Both.
        /// </summary>
        /// <value>
        /// A StretchDirection, which determines how scaling is applied to the
        /// contents of a Viewbox. The default is StretchDirection.Both.
        /// </value>
        public StretchDirection StretchDirection
        {
            get { return (StretchDirection)GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the Viewbox.StretchDirection dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the Viewbox.StretchDirection dependency property.
        /// </value>
        public static readonly DependencyProperty StretchDirectionProperty =
            DependencyProperty.Register(
                "StretchDirection",
                typeof(StretchDirection),
                typeof(Viewbox),
                new PropertyMetadata(StretchDirection.Both, OnStretchDirectionPropertyChanged) { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never });

        /// <summary>
        /// StretchDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">Viewbox that changed its StretchDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnStretchDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Viewbox viewBox = (Viewbox)d;
            if (e.NewValue != e.OldValue)
            {
                // The StretchDirection property affects measuring
                viewBox.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Builds the visual tree for the Viewbox control when a new
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            // Get the element that contains the content:
            ChildElement = GetTemplateChild("Child") as ContentPresenter;
            Debug.Assert(ChildElement != null, "The required template part Child was not found!");

            // Get the root of the template, which is chosen so that it does not force its size on the content:
            RootCanvas = GetTemplateChild("RootCanvas") as Canvas;
            Debug.Assert(RootCanvas != null, "The required template part RootCanvas was not found!");

            // Create the transformation to scale the container
            ChildElement.RenderTransform = Scale = new ScaleTransform();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => // (This fixes the issue in the MatrixView in the STAR application, where the Viewbox scale was incorrect until the window was resized. Note that when doing step-by-step debugging it worked properly)
                {
                    if (Content is FrameworkElement)
                    {
                        UpdateScale(Content as FrameworkElement);
                    }
                });
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (IsLoaded && newContent is FrameworkElement)
            {
                UpdateScale(newContent as FrameworkElement);
            }
            base.OnContentChanged(oldContent, newContent);
        }

        protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(IsLoaded && Content is FrameworkElement)
            {
                UpdateScale(Content as FrameworkElement);
            }
        }

        public void InvalidateMeasure()
        {
            if (IsLoaded && Content is FrameworkElement)
            {
                UpdateScale(Content as FrameworkElement);
            }
        }

        private void UpdateScale(FrameworkElement content)
        {
            if (Scale != null && RootCanvas != null)
            {
                Size contentSize = content.INTERNAL_GetActualWidthAndHeight();

                Size actualSize = this.INTERNAL_GetActualWidthAndHeight();
                double availableSizeX = actualSize.Width;
                double availableSizeY = actualSize.Height;

                // If the Viewbox has a size equal to 0, it likely means that its size is not given by the parent. Therefore we should set the size of the Viewbox to be the same as the size of the child:
                if (availableSizeX == 0)
                    availableSizeX = double.PositiveInfinity;
                if (availableSizeY == 0)
                    availableSizeY = double.PositiveInfinity;

                bool isConstrainedWidth = availableSizeX != double.PositiveInfinity;
                bool isConstrainedHeight = availableSizeY != double.PositiveInfinity;

                Size availableSize = new Size(availableSizeX, availableSizeY);
                Size scale = ComputeScaleFactor(availableSize, contentSize);

                Scale.ScaleX = scale.Width;
                Scale.ScaleY = scale.Height;

                if (isConstrainedWidth)
                    RootCanvas.Width = double.NaN;
                else
                    RootCanvas.Width = contentSize.Width * scale.Width;

                if (isConstrainedHeight)
                    RootCanvas.Height = double.NaN;
                else
                    RootCanvas.Height = contentSize.Height * scale.Height;
            }
        }

        /// <summary>
        /// Compute the scale factor of the Child content.
        /// </summary>
        /// <param name="availableSize">
        /// Available size to fill with content.
        /// </param>
        /// <param name="contentSize">Desired size of the content.</param>
        /// <returns>Width and Height scale factors.</returns>
        private Size ComputeScaleFactor(Size availableSize, Size contentSize)
        {
            double scaleX = 1.0;
            double scaleY = 1.0;

            bool isConstrainedWidth = availableSize.Width != double.PositiveInfinity;
            bool isConstrainedHeight = availableSize.Height != double.PositiveInfinity;

            //bool isConstrainedWidth = !double.IsPositiveInfinity(availableSize.Width);
            //bool isConstrainedHeight = !double.IsPositiveInfinity(availableSize.Height);

            Stretch stretch = Stretch;

            // Don't scale if we shouldn't stretch or the scaleX and scaleY are both infinity.
            if ((stretch != Stretch.None) && (isConstrainedWidth || !isConstrainedHeight))
            {
                // Compute the individual scaleX and scaleY scale factors
                scaleX = contentSize.Width == 0.0 ? 0.0 : (availableSize.Width / contentSize.Width);
                scaleY = contentSize.Height == 0.0 ? 0.0 : (availableSize.Height / contentSize.Height);

                // Make the scale factors uniform by setting them both equal to
                // the larger or smaller (depending on infinite lengths and the
                // Stretch value)
                if (!isConstrainedWidth)
                {
                    scaleX = scaleY;
                }
                else if (!isConstrainedHeight)
                {
                    scaleY = scaleX;
                }
                else
                {
                    // (isConstrainedWidth && isConstrainedHeight)
                    switch (stretch)
                    {
                        case Stretch.Uniform:
                            // Use the smaller factor for both
                            scaleX = scaleY = Math.Min(scaleX, scaleY);
                            break;
                        case Stretch.UniformToFill:
                            // Use the larger factor for both
                            scaleX = scaleY = Math.Max(scaleX, scaleY);
                            break;
                        case Stretch.Fill:
                        default:
                            break;
                    }
                }

                // Prevent scaling in an undesired direction
                switch (StretchDirection)
                {
                    case StretchDirection.UpOnly:
                        scaleX = Math.Max(1.0, scaleX);
                        scaleY = Math.Max(1.0, scaleY);
                        break;
                    case StretchDirection.DownOnly:
                        scaleX = Math.Min(1.0, scaleX);
                        scaleY = Math.Min(1.0, scaleY);
                        break;
                    case StretchDirection.Both:
                    default:
                        break;
                }
            }

            return new Size(scaleX, scaleY);
        }

        ///// <summary>
        ///// XAML markup used to define the write-once Viewbox template.
        ///// </summary>
        //private const string DefaultTemplateMarkup =
        //    "<ControlTemplate " +
        //      "xmlns=\"http://schemas.microsoft.com/client/2007\" " +
        //      "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
        //        "<ContentPresenter x:Name=\"Child\" " +
        //          "HorizontalAlignment=\"{TemplateBinding HorizontalAlignment}\" " +
        //          "VerticalAlignment=\"{TemplateBinding VerticalAlignment}\"/>" +
        //    "</ControlTemplate>";

        ///// <summary>
        ///// Measures the child element of a Viewbox to prepare for arranging
        ///// it during the ArrangeOverride pass.
        ///// </summary>
        ///// <remarks>
        ///// Viewbox measures it's child at an infinite constraint; it allows the child to be however large it so desires.
        ///// The child's returned size will be used as it's natural size for scaling to Viewbox's size during Arrange.
        ///// </remarks>
        ///// <param name="availableSize">
        ///// An upper limit Size that should not be exceeded.
        ///// </param>
        ///// <returns>The target Size of the element.</returns>
        ///// 
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    Size size = new Size();
        //    if (Child != null)
        //    {
        //        Debug.Assert(ChildElement != null, "The required template part ChildElement was not found!");
        //
        //        // Get the child's desired size
        //        ChildElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        //        Size desiredSize = ChildElement.DesiredSize;
        //
        //        // Determine how much we should scale the child
        //        Size scale = ComputeScaleFactor(availableSize, desiredSize);
        //        Debug.Assert(!double.IsPositiveInfinity(scale.Width), "The scale scaleX should not be infinite.");
        //        Debug.Assert(!double.IsPositiveInfinity(scale.Height), "The scale scaleY should not be infinite.");
        //
        //        // Determine the desired size of the Viewbox
        //        size.Width = scale.Width * desiredSize.Width;
        //        size.Height = scale.Height * desiredSize.Height;
        //    }
        //    return size;
        //}

        ///// <summary>
        ///// Arranges the content of a Viewbox element.
        ///// Viewbox always sets the child to its desired size.  It then computes and applies a transformation
        ///// from that size to the space available: Viewbox's own input size less child margin.
        ///// </summary>
        ///// <param name="finalSize">
        ///// The Size this element uses to arrange its child content.
        ///// </param>
        ///// <returns>
        ///// The Size that represents the arranged size of this Viewbox element
        ///// and its child.
        ///// </returns>
        //protected override Size ArrangeOverride(Size finalSize)
        //{
        //    Debug.Assert(ChildElement != null, "The required template part ChildElement was not found!");
        //    if (Child != null)
        //    {
        //        // Determine the scale factor given the final size
        //        Size desiredSize = ChildElement.DesiredSize;
        //        Size scale = ComputeScaleFactor(finalSize, desiredSize);
        //
        //        // Scale the ChildElement by the necessary factor
        //        Debug.Assert(Scale != null, "Scale should not be null!");
        //        Scale.ScaleX = scale.Width;
        //        Scale.ScaleY = scale.Height;
        //
        //        // Position the ChildElement to fill the ChildElement
        //        Rect originalPosition = new Rect(0, 0, desiredSize.Width, desiredSize.Height);
        //        ChildElement.Arrange(originalPosition);
        //
        //        // Determine the final size used by the Viewbox
        //        finalSize.Width = scale.Width * desiredSize.Width;
        //        finalSize.Height = scale.Height * desiredSize.Height;
        //    }
        //    return finalSize;
        //}

    }

}

