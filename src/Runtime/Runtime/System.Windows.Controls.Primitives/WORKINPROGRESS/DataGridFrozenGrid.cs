

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

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    /// <summary>
    /// Represents a non-scrollable grid that contains <see cref="T:System.Windows.Controls.DataGrid" /> row headers.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class DataGridFrozenGrid : Grid
    {
        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.Primitives.DataGridFrozenGrid.IsFrozen" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsFrozenProperty = DependencyProperty.RegisterAttached("IsFrozen", typeof(bool), typeof(DataGridFrozenGrid), null);

        /// <summary>
        /// Gets a value that indicates whether the grid is frozen.
        /// </summary>
        /// <returns>
        /// true if the grid is frozen; otherwise, false. The default is true.
        /// </returns>
        /// <param name="element">
        /// The object to get the <see cref="P:System.Windows.Controls.Primitives.DataGridFrozenGrid.IsFrozen" /> value from.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static bool GetIsFrozen(DependencyObject element) => (bool?) element?.GetValue(IsFrozenProperty) ?? throw new ArgumentNullException(nameof(element));

        /// <summary>
        /// Sets a value that indicates whether the grid is frozen.
        /// </summary>
        /// <param name="element">
        /// The object to set the <see cref="P:System.Windows.Controls.Primitives.DataGridFrozenGrid.IsFrozen" /> value on.
        /// </param>
        /// <param name="value">
        /// true if <paramref name="element" /> is frozen; otherwise, false.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        [OpenSilver.NotImplemented]
        public static void SetIsFrozen(DependencyObject element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(IsFrozenProperty, value);
        }
    }
}
