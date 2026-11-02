
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
/// Provides an attached property to control whether <see cref="ScrollViewer"/> elements let the browser
/// scroll their content natively.
/// </summary>
/// <remarks>
/// Native scrolling is enabled globally through <see cref="Features.Scrolling.UseNativeScrolling"/>.
/// This property is inherited, so setting it to false on a container opts out every ScrollViewer
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
        if (d is ScrollContentPresenter scp)
        {
            scp.UpdateNativeScrolling();
        }
    }
}
