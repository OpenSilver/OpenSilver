
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Enables the user to search a list of items in an <see cref="ItemsControl"/>
    /// using keyboard input.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class TextSearch : DependencyObject
    {
        /// <summary>
        /// Identifies the TextSearch.TextPath attached property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TextPathProperty =
            DependencyProperty.RegisterAttached(
                "TextPath",
                typeof(string),
                typeof(TextSearch),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Returns the name of the property that identifies an item in the specified element's
        /// collection.
        /// </summary>
        /// <param name="element">
        /// The element from which the property value is read.
        /// </param>
        /// <returns>
        /// The name of the property that identifies the item to the user.
        /// </returns>
        [OpenSilver.NotImplemented]
        public static string GetTextPath(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (string)element.GetValue(TextPathProperty);
        }

        /// <summary>
        /// Writes the TextSearch.TextPath attached property to the
        /// specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the property value is written.
        /// </param>
        /// <param name="path">
        /// The name of the property that identifies an item.
        /// </param>
        [OpenSilver.NotImplemented]
        public static void SetTextPath(DependencyObject element, string path)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(TextPathProperty, path);
        }
    }
}
