#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{    /// <summary>
     /// Enumeration for ValidationSummary control position.
     /// </summary>
    public enum DataFormValidationSummaryPosition
    {
        /// <summary>
        /// Represents the position at the bottom.
        /// </summary>
        Bottom,
        /// <summary>
        /// Represents the position at the top.
        /// </summary>
        Top
    }
}
