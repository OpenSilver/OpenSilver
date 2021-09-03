

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
using System.Windows.Navigation;
#else
using Windows.UI.Xaml.Navigation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class DataGridRow    
    {
        /// <summary>
        ///     Returns the index of this row within the DataGrid's list of item containers.
        /// </summary>
        /// <remarks>
        ///     This method performs a linear search.
        /// </remarks>
        /// <returns>The index, if found, -1 otherwise.</returns>
        [OpenSilver.NotImplemented]
        public int GetIndex()
        {
            //DataGrid dataGridOwner = DataGridOwner;
            //if (dataGridOwner != null)
            //{
            //    return dataGridOwner.ItemContainerGenerator.IndexFromContainer(this);
            //}

            return -1;
        }

        /// <summary>
        ///     Searchs up the visual parent chain from the given element until
        ///     a DataGridRow element is found.
        /// </summary>
        /// <param name="element">The descendent of a DataGridRow.</param>
        /// <returns>
        ///     The first ancestor DataGridRow of the element parameter.
        ///     Returns null of none is found.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static DataGridRow GetRowContainingElement(FrameworkElement element)
        {
            //return DataGridHelper.FindVisualParent<DataGridRow>(element);
            return null;
        }
    }
}
