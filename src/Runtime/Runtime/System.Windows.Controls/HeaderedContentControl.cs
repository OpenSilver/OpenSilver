// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using OpenSilver.Internal;

namespace System.Windows.Controls;

/// <summary>
/// Provides the base implementation for all controls that contain single content and have a header.
/// </summary>
public class HeaderedContentControl : ContentControl
{
    static HeaderedContentControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedContentControl), new PropertyMetadata(typeof(HeaderedContentControl)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderedContentControl" /> class.
    /// </summary>
    public HeaderedContentControl() { }

    /// <summary>
    /// Identifies the <see cref="Header"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(HeaderedContentControl),
            new PropertyMetadata(null, OnHeaderChanged));

    /// <summary>
    /// Gets or sets the data used for the header of each control.
    /// </summary>
    /// <value>
    /// A header object. The default is null.
    /// </value>
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValueInternal(HeaderProperty, value);
    }

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (HeaderedContentControl)d;
        ctrl.SetValueInternal(HasHeaderPropertyKey, e.NewValue is not null);
        ctrl.OnHeaderChanged(e.OldValue, e.NewValue);
    }

    /// <summary>
    /// Called when the <see cref="Header"/> property of a <see cref="HeaderedContentControl"/> changes.
    /// </summary>
    /// <param name="oldHeader">
    /// Old value of the <see cref="Header"/> property.
    /// </param>
    /// <param name="newHeader">
    /// New value of the <see cref="Header"/> property.
    /// </param>
    protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
    {
    }

    private static readonly DependencyPropertyKey HasHeaderPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(HasHeader),
            typeof(bool),
            typeof(HeaderedContentControl),
            new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>
    /// Identifies the <see cref="HasHeader"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HasHeaderProperty = HasHeaderPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value that indicates whether the header is null.
    /// </summary>
    /// <returns>
    /// true if the <see cref="Header"/> property is not null; otherwise, false. The default is false.
    /// </returns>
    public bool HasHeader => (bool)GetValue(HasHeaderProperty);

    /// <summary>
    /// Identifies the <see cref="HeaderTemplate"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(
            nameof(HeaderTemplate),
            typeof(DataTemplate),
            typeof(HeaderedContentControl),
            new PropertyMetadata(null, OnHeaderTemplateChanged));

    /// <summary>
    /// Gets or sets the template used to display the content of the control's header.
    /// </summary>
    /// <returns>
    /// A data template. The default is null.
    /// </returns>
    public DataTemplate HeaderTemplate
    {
        get => (DataTemplate)GetValue(HeaderTemplateProperty);
        set => SetValueInternal(HeaderTemplateProperty, value);
    }

    private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (HeaderedContentControl)d;
        ctrl.OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
    }

    /// <summary>
    /// Called when the <see cref="HeaderTemplate"/> property changes.
    /// </summary>
    /// <param name="oldHeaderTemplate">
    /// Old value of the <see cref="HeaderTemplate"/> property.
    /// </param>
    /// <param name="newHeaderTemplate">
    /// New value of the <see cref="HeaderTemplate"/> property.
    /// </param>
    protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
    {
    }

    /// <summary>
    /// Identifies the <see cref="HeaderTemplateSelector"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty =
        DependencyProperty.Register(
            nameof(HeaderTemplateSelectorProperty),
            typeof(DataTemplateSelector),
            typeof(HeaderedContentControl),
            new PropertyMetadata(null, OnHeaderTemplateSelectorChanged));

    /// <summary>
    /// Gets or sets a data template selector that provides custom logic for choosing the template used to display the header.
    /// </summary>
    /// <returns>
    /// A data template selector object. The default is null.
    /// </returns>
    public DataTemplateSelector HeaderTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty);
        set => SetValueInternal(HeaderTemplateSelectorProperty, value);
    }

    private static void OnHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (HeaderedContentControl)d;
        ctrl.OnHeaderTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
    }

    /// <summary>
    /// Called when the <see cref="HeaderTemplateSelector"/> property changes.
    /// </summary>
    /// <param name="oldHeaderTemplateSelector">
    /// Old value of the <see cref="HeaderTemplateSelector"/> property.
    /// </param>
    /// <param name="newHeaderTemplateSelector">
    /// New value of the <see cref="HeaderTemplateSelector"/> property.
    /// </param>
    protected virtual void OnHeaderTemplateSelectorChanged(DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
    {
    }

    internal override string GetPlainText() => ContentObjectToString(Header);
}