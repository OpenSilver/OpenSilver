
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

using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Defines a content decorator that can stretch and scale a single child to fill
    /// the available space.
    /// </summary>
    [TemplatePart(Name = ChildElementName, Type = typeof(ContentPresenter))]
    [ContentProperty(nameof(Child))]
    public class Viewbox : ContentControl
    {
        private const string ChildElementName = "Child";

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewbox" /> class.
        /// </summary>
        public Viewbox()
        {
            DefaultStyleKey = typeof(Viewbox);
            IsTabStop = false;
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// Gets or sets the element of the Viewbox that will render the child.
        /// </summary>
        private ContentPresenter ChildElement { get; set; }

        /// <summary>
        /// Gets or sets the transformation on the ChildElement used to scale the
        /// Child content.
        /// </summary>
        private ScaleTransform Scale { get; set; }

        /// <summary>
        /// Gets or sets the single child element of a <see cref="Viewbox"/> element.
        /// </summary>
        /// <returns>
        /// The single child element of a <see cref="Viewbox"/> element.
        /// </returns>
        public UIElement Child
        {
            get => (UIElement)GetValue(ChildProperty);
            set => SetValueInternal(ChildProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Child"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildProperty =
            DependencyProperty.Register(
                nameof(Child),
                typeof(UIElement),
                typeof(Viewbox),
                new PropertyMetadata(OnChildChanged));

        private static void OnChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Viewbox)d).Content = e.NewValue;
        }

        /// <summary>
        /// Gets or sets the <see cref="Stretch"/> mode, which determines how content
        /// fits into the available space.
        /// </summary>
        /// <value>
        /// A <see cref="Stretch"/> mode, which determines how content fits in the
        /// available space. The default is <see cref="Stretch.Uniform"/>.
        /// </value>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValueInternal(StretchProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Stretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                nameof(Stretch),
                typeof(Stretch),
                typeof(Viewbox),
                new FrameworkPropertyMetadata(Stretch.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsValidStretchValue);

        /// <summary>
        /// Check whether the passed in object value is a valid Stretch enum value.
        /// </summary>
        /// <param name="o">The object typed value to be checked.</param>
        /// <returns>True if o is a valid Stretch enum value, false o/w.</returns>
        private static bool IsValidStretchValue(object o)
        {
            Stretch s = (Stretch)o;
            return s == Stretch.None || s == Stretch.Uniform || s == Stretch.Fill || s == Stretch.UniformToFill;
        }

        /// <summary>
        /// Gets or sets the <see cref="Controls.StretchDirection"/>, which determines how
        /// scaling is applied to the contents of a <see cref="Viewbox"/>.
        /// </summary>
        /// <value>
        /// A <see cref="Controls.StretchDirection"/>, which determines how scaling is applied
        /// to the contents of a <see cref="Viewbox"/>. The default is <see cref="StretchDirection.Both"/>.
        /// </value>
        public StretchDirection StretchDirection
        {
            get => (StretchDirection)GetValue(StretchDirectionProperty);
            set => SetValueInternal(StretchDirectionProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StretchDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchDirectionProperty =
            DependencyProperty.Register(
                nameof(StretchDirection),
                typeof(StretchDirection),
                typeof(Viewbox),
                new FrameworkPropertyMetadata(StretchDirection.Both, FrameworkPropertyMetadataOptions.AffectsMeasure),
                IsValidStretchDirectionValue);

        /// <summary>
        /// Check whether the passed in object value is a valid StretchDirection enum value.
        /// </summary>
        /// <param name="o">The object typed value to be checked.</param>
        /// <returns>True if o is a valid StretchDirection enum value, false o/w.</returns>
        private static bool IsValidStretchDirectionValue(object o)
        {
            StretchDirection sd = (StretchDirection)o;
            return sd == StretchDirection.UpOnly || sd == StretchDirection.DownOnly || sd == StretchDirection.Both;
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="Viewbox"/> control when a new
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Get the element that contains the content:
            ChildElement = GetTemplateChild(ChildElementName) as ContentPresenter;
            Debug.Assert(ChildElement != null, "The required template part Child was not found!");

            // Create the transformation to scale the container
            if (ChildElement != null)
            {
                ChildElement.RenderTransform = Scale = new ScaleTransform();
            }
        }

        protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        /// <summary>
        /// This is a helper function that computes scale factors depending on a target size and a content size
        /// </summary>
        /// <param name="availableSize">Size into which the content is being fitted.</param>
        /// <param name="contentSize">Size of the content, measured natively (unconstrained).</param>
        /// <param name="stretch">Value of the Stretch property on the element.</param>
        /// <param name="stretchDirection">Value of the StretchDirection property on the element.</param>
        internal static Size ComputeScaleFactor(Size availableSize,
                                                Size contentSize,
                                                Stretch stretch,
                                                StretchDirection stretchDirection)
        {
            // Compute scaling factors to use for axes
            double scaleX = 1.0;
            double scaleY = 1.0;

            bool isConstrainedWidth = !double.IsPositiveInfinity(availableSize.Width);
            bool isConstrainedHeight = !double.IsPositiveInfinity(availableSize.Height);

            if ((stretch == Stretch.Uniform || stretch == Stretch.UniformToFill || stretch == Stretch.Fill)
                 && (isConstrainedWidth || isConstrainedHeight))
            {
                // Compute scaling factors for both axes
                scaleX = contentSize.Width.IsZero() ? 0.0 : availableSize.Width / contentSize.Width;
                scaleY = contentSize.Height.IsZero() ? 0.0 : availableSize.Height / contentSize.Height;

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
                    // If not preserving aspect ratio, then just apply transform to fit
                    switch (stretch)
                    {
                        case Stretch.Uniform:       //Find minimum scale that we use for both axes
                            scaleX = scaleY = Math.Min(scaleX, scaleY);
                            break;

                        case Stretch.UniformToFill: //Find maximum scale that we use for both axes
                            scaleX = scaleY = Math.Max(scaleX, scaleY);
                            break;

                        case Stretch.Fill:          //We already computed the fill scale factors above, so just use them
                            break;
                    }
                }

                //Apply stretch direction by bounding scales.
                //In the uniform case, scaleX=scaleY, so this sort of clamping will maintain aspect ratio
                //In the uniform fill case, we have the same result too.
                //In the fill case, note that we change aspect ratio, but that is okay
                switch (stretchDirection)
                {
                    case StretchDirection.UpOnly:
                        if (scaleX < 1.0) scaleX = 1.0;
                        if (scaleY < 1.0) scaleY = 1.0;
                        break;

                    case StretchDirection.DownOnly:
                        if (scaleX > 1.0) scaleX = 1.0;
                        if (scaleY > 1.0) scaleY = 1.0;
                        break;

                    case StretchDirection.Both:
                        break;

                    default:
                        break;
                }
            }
            //Return this as a size now
            return new Size(scaleX, scaleY);
        }

        /// <summary>
        /// Measures the child element of a Viewbox to prepare for arranging
        /// it during the ArrangeOverride pass.
        /// </summary>
        /// <remarks>
        /// Viewbox measures it's child at an infinite constraint; it allows the child to be however large it so desires.
        /// The child's returned size will be used as it's natural size for scaling to Viewbox's size during Arrange.
        /// </remarks>
        /// <param name="availableSize">
        /// An upper limit Size that should not be exceeded.
        /// </param>
        /// <returns>The target Size of the element.</returns>
        /// 
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size();
            if (Child != null)
            {
                Debug.Assert(ChildElement != null, "The required template part ChildElement was not found!");

                // Get the child's desired size
                ChildElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size desiredSize = ChildElement.DesiredSize;

                // Determine how much we should scale the child
                Size scale = ComputeScaleFactor(availableSize, desiredSize, Stretch, StretchDirection);
                Debug.Assert(!double.IsPositiveInfinity(scale.Width), "The scale scaleX should not be infinite.");
                Debug.Assert(!double.IsPositiveInfinity(scale.Height), "The scale scaleY should not be infinite.");

                // Determine the desired size of the Viewbox
                size.Width = scale.Width * desiredSize.Width;
                size.Height = scale.Height * desiredSize.Height;
            }
            return size;
        }

        /// <summary>
        /// Arranges the content of a Viewbox element.
        /// Viewbox always sets the child to its desired size.  It then computes and applies a transformation
        /// from that size to the space available: Viewbox's own input size less child margin.
        /// </summary>
        /// <param name="finalSize">
        /// The Size this element uses to arrange its child content.
        /// </param>
        /// <returns>
        /// The Size that represents the arranged size of this Viewbox element
        /// and its child.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.Assert(ChildElement != null, "The required template part ChildElement was not found!");
            if (Child != null)
            {
                // Determine the scale factor given the final size
                Size desiredSize = ChildElement.DesiredSize;
                Size scale = ComputeScaleFactor(finalSize, desiredSize, Stretch, StretchDirection);

                // Scale the ChildElement by the necessary factor
                Debug.Assert(Scale != null, "Scale should not be null!");
                Scale.ScaleX = scale.Width;
                Scale.ScaleY = scale.Height;

                // Position the ChildElement to fill the ChildElement
                Rect originalPosition = new Rect(0, 0, desiredSize.Width, desiredSize.Height);
                ChildElement.Arrange(originalPosition);

                // Determine the final size used by the Viewbox
                finalSize.Width = scale.Width * desiredSize.Width;
                finalSize.Height = scale.Height * desiredSize.Height;
            }
            return finalSize;
        }
    }
}
