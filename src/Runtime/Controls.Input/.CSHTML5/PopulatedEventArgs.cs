
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


using System.Collections;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides data for the
    /// <see cref="E:System.Windows.Controls.AutoCompleteBox.Populated" />
    /// event.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class PopulatedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the list of possible matches added to the drop-down portion of
        /// the <see cref="T:System.Windows.Controls.AutoCompleteBox" />
        /// control.
        /// </summary>
        /// <value>The list of possible matches added to the
        /// <see cref="T:System.Windows.Controls.AutoCompleteBox" />.</value>
        public IEnumerable Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.PopulatedEventArgs" />.
        /// </summary>
        /// <param name="data">The list of possible matches added to the
        /// drop-down portion of the
        /// <see cref="T:System.Windows.Controls.AutoCompleteBox" /> control.</param>
        public PopulatedEventArgs(IEnumerable data)
        {
            Data = data;
        }
    }
}