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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using CSHTML5.Internal;
using OpenSilver.Internal;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides a framework of common APIs for objects that participate in UI and
    /// programmatic layout. FrameworkElement also defines APIs related to data binding,
    /// object tree, and object lifetime feature areas.
    /// </summary>
    public abstract partial class FrameworkElement : UIElement
    {
        //static bool _theWarningAboutMarginsHasAlreadyBeenDisplayed = false;

        //--------------------------------------
        // Note: this is a "partial" class. This file handles anything related to Size and Alignment. Please refer to the other file for the rest of the FrameworkElement implementation.
        //--------------------------------------

        protected virtual void OnAfterApplyHorizontalAlignmentAndWidth()
        {
        }

        protected virtual void OnAfterApplyVerticalAlignmentAndWidth()
        {
        }

        #region Height property

        /// <summary>
        /// Gets or sets the suggested height of a FrameworkElement.
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)
                {
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) => (value is double) ?
                            !double.IsNaN((double)value) ? ((double)value).ToInvariantString() + "px" : "auto" :
                            throw new InvalidOperationException(
                                string.Format("Error when trying to set FrameworkElement.Height: expected double, got '{0}'.",
                                    value.GetType().FullName)),
                        UIElement = (UIElement)instance,
                        Name = new List<string> { "height" },
                        ApplyAlsoWhenThereIsAControlTemplate = true
                    }
                },
                IsWidthHeightValid);

        #endregion


        #region Width property

        /// <summary>
        /// Gets or sets the width of a FrameworkElement.
        /// </summary>
        [TypeConverter(typeof(LengthConverter))]
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    double.NaN,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)
                {
                    GetCSSEquivalent = (instance) => new CSSEquivalent
                    {
                        Value = (inst, value) => (value is double) ?
                            !double.IsNaN((double)value) ? ((double)value).ToInvariantString() + "px" : "auto" :
                            throw new InvalidOperationException(
                                string.Format("Error when trying to set FrameworkElement.Width: expected double, got '{0}'.",
                                    value.GetType().FullName)),
                        UIElement = (UIElement)instance,
                        Name = new List<string> { "width" },
                        ApplyAlsoWhenThereIsAControlTemplate = true
                    }
                },
                IsWidthHeightValid);

        #endregion


        #region HorizontalAlignment and Width handling

        /// <summary>
        /// Gets or sets the horizontal alignment characteristics that are applied to
        /// a FrameworkElement when it is composed in a layout parent, such as a panel
        /// or items control.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalAlignment"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalAlignment),
                typeof(HorizontalAlignment),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion


        #region VerticalAlignment and Height handling

        /// <summary>
        /// Gets or sets the vertical alignment characteristics that are applied to a
        /// FrameworkElement when it is composed in a parent object such as a panel or
        /// items control.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FrameworkElement.VerticalAlignment"/> dependency 
        /// property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalAlignment),
                typeof(VerticalAlignment),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion


        #region Margin

        /// <summary>
        /// Gets or sets the outer margin of a FrameworkElement.
        /// </summary>
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Margin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(
                nameof(Margin),
                typeof(Thickness),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
                IsMarginValid);

        private static bool IsMarginValid(object value)
        {
            Thickness m = (Thickness)value;
            return Thickness.IsValid(m, true, false, false, false);
        }
        #endregion


        #region MinHeight

        /// <summary>
        /// Gets or sets the minimum height constraint of a FrameworkElement.
        /// </summary>
        public double MinHeight
        {
            get { return (double)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(
                nameof(MinHeight),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
                IsMinWidthHeightValid);

        #endregion


        #region MinWidth

        /// <summary>
        /// Gets or sets the minimum width constraint of a FrameworkElement.
        /// </summary>
        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                nameof(MinWidth),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
                IsMinWidthHeightValid);

        #endregion


        #region MaxHeight

        /// <summary>
        /// Gets or sets the maximum height constraint of a FrameworkElement.
        /// </summary>
        public double MaxHeight
        {
            get { return (double)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaxHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(
                nameof(MaxHeight),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    double.PositiveInfinity,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
                IsMaxWidthHeightValid);

        #endregion


        #region MaxWidth

        /// <summary>
        /// Gets or sets the maximum width constraint of a FrameworkElement.
        /// </summary>
        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaxWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                nameof(MaxWidth),
                typeof(double),
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    double.PositiveInfinity,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange),
                IsMaxWidthHeightValid);

        #endregion

        private static bool IsWidthHeightValid(object value)
        {
            double v = (double)value;
            return double.IsNaN(v) || (v >= 0.0d && !double.IsPositiveInfinity(v));
        }

        internal static bool IsMinWidthHeightValid(object value)
        {
            double v = (double)value;
            return !double.IsNaN(v) && v >= 0.0d && !double.IsPositiveInfinity(v);
        }

        internal static bool IsMaxWidthHeightValid(object value)
        {
            double v = (double)value;
            return !double.IsNaN(v) && v >= 0.0;
        }
        
        #region ActualWidth / ActualHeight

        private static PropertyMetadata _actualWidthMetadata = new ReadOnlyPropertyMetadata(0d, GetActualWidth);

        private static readonly DependencyPropertyKey ActualWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ActualWidth),
                typeof(double),
                typeof(FrameworkElement),
                _actualWidthMetadata);

        private static object GetActualWidth(DependencyObject d)
        {
            FrameworkElement fe = (FrameworkElement)d;
            return fe.HasWidthEverChanged ? fe.RenderSize.Width : 0d;
        }

        /// <summary>
        /// Identifies the <see cref="ActualWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the rendered width of a <see cref="FrameworkElement"/>.
        /// </summary>
        /// <returns>
        /// The width, in pixels, of the object. The default is 0. The default might be 
        /// encountered if the object has not been loaded and undergone a layout pass.
        /// </returns>
        public double ActualWidth => RenderSize.Width;

        private static PropertyMetadata _actualHeightMetadata = new ReadOnlyPropertyMetadata(0d, GetActualHeight);

        private static readonly DependencyPropertyKey ActualHeightPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ActualHeight),
                typeof(double),
                typeof(FrameworkElement),
                _actualHeightMetadata);

        private static object GetActualHeight(DependencyObject d)
        {
            FrameworkElement fe = (FrameworkElement)d;
            return fe.HasHeightEverChanged ? fe.RenderSize.Height : 0d;
        }

        /// <summary>
        /// Identifies the <see cref="ActualHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualHeightProperty = ActualHeightPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the rendered height of a <see cref="FrameworkElement"/>.
        /// </summary>
        /// <returns>
        /// The height, in pixels, of the object. The default is 0. The default might be
        /// encountered if the object has not been loaded and undergone a layout pass.
        /// </returns>
        public double ActualHeight => RenderSize.Height;

        /// <summary>
        /// Gets the rendered size of a FrameworkElement.
        /// </summary>
        [Obsolete(Helper.ObsoleteMemberMessage + " Use FrameworkElement.RenderSize instead.")]
        public Size ActualSize => RenderSize;

        #endregion


        #region ContextMenu

        /// <summary>
        /// Gets or sets the context menu element that should appear whenever the context 
        /// menu is requested through user interface (UI) from within this element.
        /// </summary>
        public ContextMenu ContextMenu
        {
            get { return (ContextMenu)GetValue(ContextMenuProperty); }
            set { SetValue(ContextMenuProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ContextMenu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.Register(
                nameof(ContextMenu),
                typeof(ContextMenu),
                typeof(FrameworkElement),
                new PropertyMetadata(null, OnContextMenuChanged));

        private static void OnContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContextMenuService.SetContextMenu(d, (ContextMenu)e.NewValue);
        }

        /// <summary>
        /// Occurs when any context menu on the element is opened.
        /// </summary>
        public event ContextMenuEventHandler ContextMenuOpening;

        internal void OnContextMenuOpening(double x, double y)
            => ContextMenuOpening?.Invoke(this, new ContextMenuEventArgs(x, y));

        #endregion
    }
}