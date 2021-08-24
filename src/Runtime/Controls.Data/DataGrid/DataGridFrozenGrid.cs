// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls,Primitives
#endif
{
    /// <summary>
    /// Represents a non-scrollable grid that contains <see cref="T:System.Windows.Controls.DataGrid" /> row headers.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public class DataGridFrozenGrid : Grid
    {
        /// <summary>
        /// A dependency property that indicates whether the grid is frozen.
        /// </summary>
        public static readonly DependencyProperty IsFrozenProperty = DependencyProperty.RegisterAttached(
            "IsFrozen",
            typeof(Boolean),
            typeof(DataGridFrozenGrid),
            null);

        /// <summary>
        /// Gets a value that indicates whether the grid is frozen.
        /// </summary>
        /// <param name="element">
        /// The object to get the <see cref="P:System.Windows.Controls.Primitives.DataGridFrozenGrid.IsFrozen" /> value from.
        /// </param>
        /// <returns>true if the grid is frozen; otherwise, false. The default is true.</returns>
        public static bool GetIsFrozen(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(IsFrozenProperty);
        }

        /// <summary>
        /// Sets a value that indicates whether the grid is frozen.
        /// </summary>
        /// <param name="element">The object to set the <see cref="P:System.Windows.Controls.Primitives.DataGridFrozenGrid.IsFrozen" /> value on.</param>
        /// <param name="value">true if <paramref name="element" /> is frozen; otherwise, false.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="element" /> is null.</exception>
        public static void SetIsFrozen(DependencyObject element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(IsFrozenProperty, value);
        }
   }
}
