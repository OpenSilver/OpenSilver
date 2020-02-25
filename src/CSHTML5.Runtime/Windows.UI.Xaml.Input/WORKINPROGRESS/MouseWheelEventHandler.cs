#if WORKINPROGRESS
using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    ///     The delegate to use for handlers that receive MouseWheelEventArgs.
    /// </summary>
    public delegate void MouseWheelEventHandler(object sender, MouseWheelEventArgs e);
}
#endif