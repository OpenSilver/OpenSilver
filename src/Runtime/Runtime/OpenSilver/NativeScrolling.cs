
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
using System.Windows;
using System.Windows.Controls;
using OpenSilver.Internal;

namespace OpenSilver;

/// <summary>
/// Provides attached properties to control whether <see cref="ScrollViewer"/> elements let the browser
/// scroll their content natively, and whether they show the platform scrollbars.
/// </summary>
/// <remarks>
/// Native scrolling is enabled globally through <see cref="Features.Scrolling.UseNativeScrolling"/>.
/// The IsEnabled property is inherited, so setting it to false on a container opts out every ScrollViewer
/// below it.
/// </remarks>
public static class NativeScrolling
{
    /// <summary>
    /// Identifies the IsEnabled attached property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(NativeScrolling),
            new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, FrameworkPropertyMetadataOptions.Inherits, OnIsEnabledChanged));

    /// <summary>
    /// Gets a value that indicates whether native scrolling is allowed for the specified element
    /// and its descendants.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the property value.
    /// </param>
    /// <returns>
    /// true if native scrolling is allowed; otherwise, false. The default is true.
    /// </returns>
    public static bool GetIsEnabled(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(IsEnabledProperty);
    }

    /// <summary>
    /// Sets a value that indicates whether native scrolling is allowed for the specified element
    /// and its descendants.
    /// </summary>
    /// <param name="element">
    /// The element on which to set the property value.
    /// </param>
    /// <param name="value">
    /// true to allow native scrolling; otherwise, false.
    /// </param>
    public static void SetIsEnabled(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValueInternal(IsEnabledProperty, BooleanBoxes.Box(value));
    }

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        switch (d)
        {
            case ScrollContentPresenter scp:
                scp.UpdateNativeScrolling();
                break;

            case VirtualizingStackPanel vsp:
                vsp.UpdateNativeScrolling();
                break;
        }
    }

    /// <summary>
    /// Identifies the UseNativeScrollBars attached property.
    /// </summary>
    public static readonly DependencyProperty UseNativeScrollBarsProperty =
        DependencyProperty.RegisterAttached(
            "UseNativeScrollBars",
            typeof(bool),
            typeof(NativeScrolling),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits, OnUseNativeScrollBarsChanged));

    /// <summary>
    /// Gets a value that indicates whether natively scrolled <see cref="ScrollViewer"/> elements show the
    /// scrollbars of the platform instead of the ScrollBars defined in their template.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the property value.
    /// </param>
    /// <returns>
    /// true if the platform scrollbars are used; otherwise, false. The default is false.
    /// </returns>
    public static bool GetUseNativeScrollBars(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (bool)element.GetValue(UseNativeScrollBarsProperty);
    }

    /// <summary>
    /// Sets a value that indicates whether natively scrolled <see cref="ScrollViewer"/> elements show the
    /// scrollbars of the platform instead of the ScrollBars defined in their template.
    /// </summary>
    /// <param name="element">
    /// The element on which to set the property value.
    /// </param>
    /// <param name="value">
    /// true to use the platform scrollbars; otherwise, false.
    /// </param>
    /// <remarks>
    /// <para>
    /// This property is inherited: it can be set on a single ScrollViewer, on a container, or in a
    /// ScrollViewer style.
    /// </para>
    /// <para>
    /// The platform scrollbars are only used where they overlay the content (typically on mobile devices
    /// and on macOS), where the platform also decides when they are shown. Where they would take space
    /// (for example in desktop browsers on Windows), the ScrollBars of the template are kept, as well as
    /// for ScrollViewers that are not natively scrolled (for example in a TextBox), whose template has no
    /// ScrollBar for a scrollable axis, or that have a <see cref="ScrollBarVisibility.Hidden"/> axis.
    /// </para>
    /// <para>
    /// While the platform scrollbars are used, the ScrollBars of the template are collapsed:
    /// <see cref="ScrollViewer.ComputedHorizontalScrollBarVisibility"/> and
    /// <see cref="ScrollViewer.ComputedVerticalScrollBarVisibility"/> are <see cref="Visibility.Collapsed"/>.
    /// </para>
    /// </remarks>
    public static void SetUseNativeScrollBars(DependencyObject element, bool value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValueInternal(UseNativeScrollBarsProperty, BooleanBoxes.Box(value));
    }

    private static void OnUseNativeScrollBarsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        switch (d)
        {
            // Also updates its ScrollContentPresenter.
            case ScrollViewer sv:
                sv.InvalidateMeasure();
                break;

            case VirtualizingStackPanel vsp:
                vsp.UpdateNativeScrolling();
                break;
        }
    }
}
