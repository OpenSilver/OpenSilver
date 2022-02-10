

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides data for the <see cref="E:System.Windows.Controls.AutoCompleteBox.Populating" /> event.
    /// </summary>
    public class PopulatingEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Windows.Controls.PopulatingEventArgs" /> class.
        /// </summary>
        /// <param name="parameter">
        /// The value of the <see cref="P:System.Windows.Controls.AutoCompleteBox.SearchText" /> property,
        /// which is used to filter items for the <see cref="T:System.Windows.Controls.AutoCompleteBox" /> control.
        /// </param>
        public PopulatingEventArgs(string parameter)
        {
            this.Parameter = parameter;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="E:System.Windows.Controls.AutoCompleteBox.Populating" /> event should be canceled.
        /// </summary>
        /// <returns>
        /// true to cancel the event, otherwise false. The default is false.
        /// </returns>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets the text that is used to determine which items to display in the <see cref="T:System.Windows.Controls.AutoCompleteBox" /> control.
        /// </summary>
        /// <returns>
        /// The text that is used to determine which items to display in the <see cref="T:System.Windows.Controls.AutoCompleteBox" />.
        /// </returns>
        public string Parameter { get; }
    }
}
