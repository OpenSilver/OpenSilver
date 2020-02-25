using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines a method for internal navigation in an application.
    /// </summary>
    public partial interface INavigate
    {
        /// <summary>
        /// Displays the content located at the specified URI.
        /// </summary>
        /// <param name="source">The URI of the content to display.</param>
        /// <returns>true if the content was successfully displayed; otherwise, false.</returns>
        bool Navigate(Uri source);
    }
}
