
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

using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Documents;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the base class for UI elements that use a ControlTemplate to define
    /// their appearance.
    /// </summary>
    public partial class Control : FrameworkElement, IInternalControl
    {
        /// <summary>
        /// Represents the base class for UI elements that use a <see cref="ControlTemplate"/>
        /// to define their appearance.
        /// </summary>
        public Control()
        {
            // Initialize the _templateCache to the default value for TemplateProperty.
            // If the default value is non-null then wire it to the current instance.
            PropertyMetadata metadata = TemplateProperty.GetMetadata(DependencyObjectType);
            ControlTemplate defaultValue = (ControlTemplate)metadata.GetDefaultValue(this, TemplateProperty);
            if (defaultValue != null)
            {
                OnTemplateChanged(this, new DependencyPropertyChangedEventArgs(null, defaultValue, TemplateProperty, metadata));
            }
        }

        //-----------------------
        // ISENABLED (OVERRIDE)
        //-----------------------

        protected internal override void ManageIsEnabled(bool isEnabled)
        {
            base.ManageIsEnabled(isEnabled); // Useful for setting the "disabled" attribute on the DOM element.

            UpdateVisualStates();
        }

        //-----------------------
        // BACKGROUND
        //-----------------------

        /// <summary>
        /// Gets or sets a brush that provides the background of the control.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValueInternal(BackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                nameof(Background),
                typeof(Brush), 
                typeof(Control), 
                new PropertyMetadata((object)null));

        //-----------------------
        // BORDERBRUSH
        //-----------------------

        /// <summary>
        /// Gets or sets a brush that describes the border background of a control.
        /// </summary>
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValueInternal(BorderBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                nameof(BorderBrush), 
                typeof(Brush), 
                typeof(Control), 
                new PropertyMetadata((object)null));

        //-----------------------
        // BORDERTHICKNESS
        //-----------------------
        /// <summary>
        /// Gets or sets the thickness of the border.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValueInternal(BorderThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                nameof(BorderThickness), 
                typeof(Thickness), 
                typeof(Control), 
                new PropertyMetadata(new Thickness()));

        //-----------------------
        // FONTWEIGHT
        //-----------------------

        /// <summary>
        /// Gets or sets the thickness of the specified font.
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValueInternal(FontWeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(typeof(Control));

        /// <summary>
        /// Gets or sets the style in which the text is rendered.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValueInternal(FontStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontStyleProperty =
            TextElement.FontStyleProperty.AddOwner(typeof(Control));

        //-----------------------
        // FOREGROUND
        //-----------------------

        /// <summary>
        /// Gets or sets a brush that describes the foreground color.
        /// </summary>
        /// <returns>
        /// The brush that paints the foreground of the control. The default value 
        /// is <see cref="Colors.Black"/>.
        /// </returns>
        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValueInternal(ForegroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(typeof(Control));

        //-----------------------
        // FONTFAMILY
        //----------------------

        /// <summary>
        /// Gets or sets the font used to display text in the control.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValueInternal(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            TextElement.FontFamilyProperty.AddOwner(typeof(Control));

        //-----------------------
        // FONTSIZE
        //-----------------------

        /// <summary>
        /// Gets or sets the size of the text in this control.
        /// </summary>
        /// <returns>
        /// The size of the text in the <see cref="Control"/>. The default is 11 (in pixels).
        /// </returns>
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValueInternal(FontSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            TextElement.FontSizeProperty.AddOwner(typeof(Control));

        //-----------------------
        // TEXTDECORATION
        //-----------------------
        // Note: this was moved from TextBlock because it is more practical for styling.
        /// <summary>
        /// Gets or sets the text decorations (underline, strikethrough...).
        /// </summary>
        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValueInternal(TextDecorationsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="TextDecorations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty =
            Inline.TextDecorationsProperty.AddOwner(typeof(Control));

        //-----------------------
        // PADDING
        //-----------------------

        /// <summary>
        /// Gets or sets the distance between the border and its child object.
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValueInternal(PaddingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(Thickness),
                typeof(Control),
                new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));

        //-----------------------
        // HORIZONTALCONTENTALIGNMENT
        //-----------------------

        /// <summary>
        /// Gets or sets the horizontal alignment of the control's content.
        /// </summary>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValueInternal(HorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HorizontalContentAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalContentAlignment), 
                typeof(HorizontalAlignment), 
                typeof(Control),
                new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange));

        //-----------------------
        // VERTICALCONTENTALIGNMENT
        //-----------------------

        /// <summary>
        /// Gets or sets the vertical alignment of the control's content.
        /// </summary>
        public VerticalAlignment VerticalContentAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValueInternal(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="VerticalContentAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalContentAlignment), 
                typeof(VerticalAlignment), 
                typeof(Control),
                new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange));

        //-----------------------
        // TABINDEX
        //-----------------------

        /// <summary>
        /// Gets or sets a value that determines the order in which elements receive
        /// focus when the user navigates through controls by pressing the Tab key.
        /// The default value is MaxValue
        /// </summary>
        public int TabIndex
        {
            get => (int)GetValue(TabIndexProperty);
            set => SetValueInternal(TabIndexProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TabIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabIndexProperty =
            DependencyProperty.Register(
                nameof(TabIndex), 
                typeof(int), 
                typeof(Control), 
                new PropertyMetadata(int.MaxValue));

        //-----------------------
        // ISTABSTOP
        //-----------------------

        /// <summary>
        /// Gets or sets a value that indicates whether a control is included in tab
        /// navigation.
        /// </summary>
        public bool IsTabStop
        {
            get => (bool)GetValue(IsTabStopProperty);
            set => SetValueInternal(IsTabStopProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsTabStop"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTabStopProperty =
            DependencyProperty.Register(
                nameof(IsTabStop),    
                typeof(bool), 
                typeof(Control), 
                new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value that modifies how tabbing and <see cref="TabIndex"/>
        /// work for this control.
        /// </summary>
        /// <returns>
        /// A value of the enumeration. The default is <see cref="KeyboardNavigationMode.Local"/>.
        /// </returns>
        public KeyboardNavigationMode TabNavigation
        {
            get => (KeyboardNavigationMode)GetValue(TabNavigationProperty);
            set => SetValueInternal(TabNavigationProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TabNavigation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabNavigationProperty =
            DependencyProperty.Register(
                nameof(TabNavigation),
                typeof(KeyboardNavigationMode),
                typeof(Control),
                new PropertyMetadata(KeyboardNavigationMode.Local));

        //-----------------------
        // TEMPLATE
        //-----------------------

        private ControlTemplate _templateCache;

        /// <summary>
        /// Gets or sets a control template.
        /// </summary>
        public ControlTemplate Template
        {
            get { return this._templateCache; }
            set { SetValueInternal(TemplateProperty, value); }
        }

        // Internal Helper so the FrameworkElement could see this property
        internal override FrameworkTemplate TemplateInternal
        {
            get { return Template; }
        }

        // Internal Helper so the FrameworkElement could see the template cache
        internal override FrameworkTemplate TemplateCache
        {
            get { return _templateCache; }
            set { _templateCache = (ControlTemplate)value; }
        }

        /// <summary>
        /// Identifies the <see cref="Control.Template"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(
                nameof(Template), 
                typeof(ControlTemplate), 
                typeof(Control), 
                new PropertyMetadata(null, OnTemplateChanged));

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            UpdateTemplateCache(control, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);

            control.InvalidateMeasure();
        }

        /// <summary>
        /// Loads the relevant control template so that its parts can be referenced.
        /// </summary>
        /// <returns>
        /// Returns whether the visual tree was rebuilt by this call. true indicates the
        /// tree was rebuilt; false indicates that the previous visual tree was retained.
        /// </returns>
        public new bool ApplyTemplate()
        {
            return base.ApplyTemplate();
        }

        /// <summary>
        /// Retrieves the named element in the instantiated ControlTemplate visual tree.
        /// </summary>
        /// <param name="childName">The name of the element to find.</param>
        /// <returns>
        /// The named element from the template, if the element is found. Can return
        /// null if no element with name childName was found in the template.
        /// </returns>
        protected internal new DependencyObject GetTemplateChild(string childName)
        {
            return base.GetTemplateChild(childName);
        }

        //-----------------------
        // OTHER
        //-----------------------

        /// <summary>
        /// Attempts to set the focus on the control.
        /// </summary>
        /// <returns>
        /// true if focus was set to the control, or focus was already on the control.
        /// false if the control is not focusable.
        /// </returns>
        public bool Focus() =>
            KeyboardNavigation.Current.Focus(this) is UIElement uie &&
            InputManager.Current.SetFocus(uie);

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool UseSystemFocusVisuals
        {
            get => base.UseSystemFocusVisuals;
            set => base.UseSystemFocusVisuals = value;
        }

        /// <summary>
        /// If control has a scrollviewer in its style and has a custom keyboard 
        /// scrolling behavior when HandlesScrolling should return true.
        /// Then ScrollViewer will not handle keyboard input and leave it up to 
        /// the control.
        /// </summary>
        internal virtual bool HandlesScrolling => false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_visualStatesUpdater != null)
            {
                _visualStatesUpdater.Dispose();
                _visualStatesUpdater = null;
            }

            if (EnableBaseControlHandlingOfVisualStates)
            {
                _visualStatesUpdater = new VisualStateUpdater(this);
            }
        }

        /// <summary>
        /// Called before the <see cref="UIElement.GotFocus"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _isFocused = true;

            if (_isInvalid)
            {
                UpdateValidationState();
            }
        }

        /// <summary>
        /// Called before the <see cref="UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">
        /// The data for the event.
        /// </param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            _isFocused = false;

            if (_isInvalid)
            {
                UpdateValidationState();
            }
        }

        public sealed override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            return CreateDomElementInternal(parentRef, true, out domElementWhereToPlaceChildren);
        }

        /// <summary>
        /// Identifies the <see cref="CharacterSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CharacterSpacingProperty =
            TextElement.CharacterSpacingProperty.AddOwner(typeof(Control));

        /// <summary>
        /// Gets or sets the distance between characters of text in the control measured
        /// in 1000ths of the font size.
        /// </summary>
        /// <returns>
        /// The distance between characters of text in the control measured in 1000ths of
        /// the font size. The default is 0.
        /// </returns>
        public int CharacterSpacing
        {
            get => (int)GetValue(CharacterSpacingProperty);
            set => SetValueInternal(CharacterSpacingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FontStretch"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty FontStretchProperty =
            TextElement.FontStretchProperty.AddOwner(typeof(Control));

        /// <summary>
        /// Gets or sets the degree to which a font is condensed or expanded on the screen.
        /// </summary>
        /// <returns>
        /// One of the values that specifies the degree to which a font is condensed or expanded
        /// on the screen. The default is <see cref="FontStretches.Normal"/>.
        /// </returns>
        [OpenSilver.NotImplemented]
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValueInternal(FontStretchProperty, value); }
        }

        [OpenSilver.NotImplemented]
        protected virtual void OnDrop(DragEventArgs e)
        {

        }

        [OpenSilver.NotImplemented]
        protected virtual void OnDragEnter(DragEventArgs e)
        {

        }

        [OpenSilver.NotImplemented]
        protected virtual void OnDragLeave(DragEventArgs e)
        {

        }

        [OpenSilver.NotImplemented]
        protected virtual void OnTextInputUpdate(TextCompositionEventArgs e)
        {

        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            int count = VisualChildrenCount;

            if (count > 0)
            {
                UIElement child = GetVisualChild(0);
                if (child != null)
                {
                    child.Measure(availableSize);
                    return child.DesiredSize;
                }
            }

            return new Size(0.0, 0.0);
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = VisualChildrenCount;

            if (count > 0)
            {
                UIElement child = GetVisualChild(0);
                if (child != null)
                {
                    child.Arrange(new Rect(finalSize));
                }
            }
            return finalSize;
        }
    }
}
