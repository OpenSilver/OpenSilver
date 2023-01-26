
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
    /// A control that enables drag and drop operations on ListBox.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public class ListBoxDragDropTarget : ItemsControlDragDropTarget<ListBox, ListBoxItem>
    {
        /// <summary>
        /// Gets the ListBox that is the drag drop target.
        /// </summary>
        protected ListBox ListBox
        {
            get { return Content as ListBox; }
        }

        /// <summary>
        /// Initializes a new instance of the ListBoxDragDropTarget class.
        /// </summary>
        public ListBoxDragDropTarget()
        {
        }

        /// <summary>
        /// Ensures the content of control is a ListBox.
        /// </summary>
        /// <param name="oldContent">The old content.</param>
        /// <param name="newContent">The new content.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            if (newContent != null && !(newContent is ListBox))
            {
                throw new ArgumentException("The content property must be a ListBox.");
            }
            base.OnContentChanged(oldContent, newContent);
        }
    }
}
