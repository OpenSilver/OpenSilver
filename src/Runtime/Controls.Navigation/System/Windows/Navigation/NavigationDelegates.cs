

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


#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// Delegate for the NavigationFailed event
    /// </summary>
    /// <param name="sender">The object sending this event</param>
    /// <param name="e">The event arguments</param>
    /// <QualityBand>Stable</QualityBand>
    public delegate void NavigationFailedEventHandler(object sender, NavigationFailedEventArgs e);

    /// <summary>
    /// Delegate for the NavigationStopped event
    /// </summary>
    /// <param name="sender">The object sending this event</param>
    /// <param name="e">The event arguments</param>
    /// <QualityBand>Stable</QualityBand>
    public delegate void NavigationStoppedEventHandler(object sender, NavigationEventArgs e);

    /// <summary>
    /// Delegate for FragmentNavigation event
    /// </summary>
    /// <param name="sender">The object sending this event</param>
    /// <param name="e">The event arguments</param>
    /// <QualityBand>Stable</QualityBand>
    public delegate void FragmentNavigationEventHandler(object sender, FragmentNavigationEventArgs e);
}
