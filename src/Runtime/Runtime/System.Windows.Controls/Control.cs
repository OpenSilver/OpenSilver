
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

using OpenSilver;
using OpenSilver.Internal;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace System.Windows.Controls;

/// <summary>
/// Represents the base class for UI elements that use a ControlTemplate to define
/// their appearance.
/// </summary>
public partial class Control : FrameworkElement, IInternalControl
{
    static Control()
    {
        FocusableProperty.OverrideMetadata(typeof(Control), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

        EventManager.RegisterClassHandler<Control>(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(HandleDoubleClick), true);
        EventManager.RegisterClassHandler<Control>(MouseLeftButtonDownEvent, new MouseButtonEventHandler(HandleDoubleClick), true);
        EventManager.RegisterClassHandler<Control>(PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(HandleDoubleClick), true);
        EventManager.RegisterClassHandler<Control>(MouseRightButtonDownEvent, new MouseButtonEventHandler(HandleDoubleClick), true);
    }

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

    /// <summary>
    /// Identifies the <see cref="PreviewMouseDoubleClick"/> routed event.
    /// </summary>
    public static readonly RoutedEvent PreviewMouseDoubleClickEvent =
        EventManager.RegisterRoutedEvent(
            nameof(PreviewMouseDoubleClick),
            RoutingStrategy.Direct,
            typeof(MouseButtonEventHandler),
            typeof(Control));

    /// <summary>
    /// Occurs when a user clicks the mouse button two or more times.
    /// </summary>
    public event MouseButtonEventHandler PreviewMouseDoubleClick
    {
        add => AddHandler(PreviewMouseDoubleClickEvent, value);
        remove => RemoveHandler(PreviewMouseDoubleClickEvent, value);
    }

    /// <summary>
    /// Raises the <see cref="PreviewMouseDoubleClick"/> routed event.
    /// </summary>
    /// <param name="e">
    /// The event data.
    /// </param>
    protected virtual void OnPreviewMouseDoubleClick(MouseButtonEventArgs e) => RaiseEvent(e);

    /// <summary>
    /// Identifies the <see cref="MouseDoubleClick"/> routed event.
    /// </summary>
    public static readonly RoutedEvent MouseDoubleClickEvent =
        EventManager.RegisterRoutedEvent(
            nameof(MouseDoubleClick),
            RoutingStrategy.Direct,
            typeof(MouseButtonEventHandler),
            typeof(Control));

    /// <summary>
    /// Occurs when a mouse button is clicked two or more times.
    /// </summary>
    public event MouseButtonEventHandler MouseDoubleClick
    {
        add => AddHandler(MouseDoubleClickEvent, value);
        remove => RemoveHandler(MouseDoubleClickEvent, value);
    }

    /// <summary>
    /// Raises the <see cref="MouseDoubleClick"/> routed event.
    /// </summary>
    /// <param name="e">
    /// The event data.
    /// </param>
    protected virtual void OnMouseDoubleClick(MouseButtonEventArgs e) => RaiseEvent(e);

    private static void HandleDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            var ctrl = (Control)sender;
            var doubleClick = new MouseButtonEventArgs(
                e.ChangedButton,
                e.ButtonState,
                e.IsTouchEvent,
                e.KeyModifiers,
                e._pointerAbsoluteX,
                e._pointerAbsoluteY);

            if (e.RoutedEvent == PreviewMouseLeftButtonDownEvent || e.RoutedEvent == PreviewMouseRightButtonDownEvent)
            {
                doubleClick.RoutedEvent = PreviewMouseDoubleClickEvent;
                doubleClick.Source = e.OriginalSource; // Set OriginalSource because initially is null
                doubleClick.OverrideSource(e.Source);
                ctrl.OnPreviewMouseDoubleClick(doubleClick);
            }
            else
            {
                doubleClick.RoutedEvent = MouseDoubleClickEvent;
                doubleClick.Source = e.OriginalSource; // Set OriginalSource because initially is null
                doubleClick.OverrideSource(e.Source);
                ctrl.OnMouseDoubleClick(doubleClick);
            }

            // If MouseDoubleClick event is handled - we delegate the state to original MouseButtonEventArgs
            if (doubleClick.Handled)
            {
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Gets or sets a brush that provides the background of the control.
    /// </summary>
    public Brush Background
    {
        get => (Brush)GetValue(BackgroundProperty);
        set => SetValueInternal(BackgroundProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Background"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackgroundProperty =
        Panel.BackgroundProperty.AddOwner(
            typeof(Control),
            new FrameworkPropertyMetadata(
                Panel.BackgroundProperty.DefaultMetadata.DefaultValue,
                FrameworkPropertyMetadataOptions.None));

    /// <summary>
    /// Gets or sets a brush that describes the border background of a control.
    /// </summary>
    public Brush BorderBrush
    {
        get => (Brush)GetValue(BorderBrushProperty);
        set => SetValueInternal(BorderBrushProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="BorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderBrushProperty =
        Border.BorderBrushProperty.AddOwner(
            typeof(Control),
            new FrameworkPropertyMetadata(
                Border.BorderBrushProperty.DefaultMetadata.DefaultValue,
                FrameworkPropertyMetadataOptions.None));

    /// <summary>
    /// Gets or sets the thickness of the border.
    /// </summary>
    public Thickness BorderThickness
    {
        get => (Thickness)GetValue(BorderThicknessProperty);
        set => SetValueInternal(BorderThicknessProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="BorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BorderThicknessProperty =
        Border.BorderThicknessProperty.AddOwner(
            typeof(Control),
            new FrameworkPropertyMetadata(
                Border.BorderThicknessProperty.DefaultMetadata.DefaultValue,
                FrameworkPropertyMetadataOptions.None));

    /// <summary>
    /// Gets or sets the radius for the corners of the control's border.
    /// </summary>
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValueInternal(CornerRadiusProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CornerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CornerRadiusProperty =
        Border.CornerRadiusProperty.AddOwner(
            typeof(Control),
            new FrameworkPropertyMetadata(
                Border.CornerRadiusProperty.DefaultMetadata.DefaultValue,
                FrameworkPropertyMetadataOptions.None));


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
    public static readonly DependencyProperty TabIndexProperty = KeyboardNavigation.TabIndexProperty.AddOwner(typeof(Control));

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
    public static readonly DependencyProperty IsTabStopProperty = KeyboardNavigation.IsTabStopProperty.AddOwner(typeof(Control));

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
    public static readonly DependencyProperty TabNavigationProperty = KeyboardNavigation.TabNavigationProperty.AddOwner(typeof(Control));

    //-----------------------
    // TEMPLATE
    //-----------------------

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
        StyleHelper.UpdateTemplateCache(control, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);

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
    public new bool Focus() => base.Focus();

    [Obsolete(Helper.ObsoleteMemberMessage)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool UseSystemFocusVisuals
    {
        get => base.UseSystemFocusVisuals;
        set => base.UseSystemFocusVisuals = value;
    }

    /// <summary>
    /// Gets a value that indicates whether a control supports scrolling.
    /// </summary>
    /// <returns>
    /// true if the control has a <see cref="ScrollViewer"/> in its style and has a 
    /// custom keyboard scrolling behavior; otherwise, false.
    /// </returns>
    protected internal virtual bool HandlesScrolling => false;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (EnableBaseControlHandlingOfVisualStates)
        {
            if (VisualStatesUpdaterField.GetValue(this) is VisualStateUpdater visualStateUpdater)
            {
                visualStateUpdater.Dispose();
            }

            VisualStatesUpdaterField.SetValue(this, new VisualStateUpdater(this));
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
        IsFocused = true;

        if (IsInvalid)
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
        IsFocused = false;

        if (IsInvalid)
        {
            UpdateValidationState();
        }
    }

    /// <inheritdoc />
    protected internal sealed override HtmlElementReference CreateDomElement(HtmlElementReference parent)
    {
        return CreateDomElementInternal(parent, true);
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

    private bool IsFocused
    {
        get => ReadControlFlag(ControlFlags.Focused);
        set => WriteControlFlag(ControlFlags.Focused, value);
    }

    internal bool ReadControlFlag(ControlFlags flag) => (_flags & flag) != 0;

    internal void WriteControlFlag(ControlFlags flag, bool set)
    {
        if (set)
        {
            _flags |= flag;
        }
        else
        {
            _flags &= ~flag;
        }
    }

    private ControlFlags _flags;
    private ControlTemplate _templateCache;

    internal enum ControlFlags : byte
    {
        HandleCommonVisualStates = 0x001, // Used in Control
        HasVisualStateUpdater = 0x002, // Used in Control
        Invalid = 0x004, // Used in Control
        Focused = 0x008, // Used in Control
        ContentIsNotLogical = 0x010, // Used in ContentControl
        CommandDisabled = 0x020, // Used in ButtonBase, MenuItem
        IsSpaceKeyDown = 0x040, // Used in ButtonBase
    }
}
