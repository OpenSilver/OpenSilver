
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

namespace System.Windows.Printing
{
    /// <summary>
    /// Provides data for the <see cref="PrintDocument.PrintPage"/> event.
    /// </summary>
    public sealed class PrintPageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrintPageEventArgs"/> class.
        /// </summary>
        public PrintPageEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets whether there are more pages to print.
        /// </summary>
        /// <returns>
        /// true if there are additional pages to print; otherwise, false. The default is false.
        /// </returns>
        public bool HasMorePages { get; set; }

        /// <summary>
        /// Gets or sets the visual element to print.
        /// </summary>
        /// <returns>
        /// The visual element to print. The default is null.
        /// </returns>
        public UIElement PageVisual { get; set; }

        /// <summary>
        /// Gets the margins of the page that is currently printing.
        /// </summary>
        /// <returns>
        /// The margins of the page that is currently printing.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Thickness PageMargins
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the size of the printable area.
        /// </summary>
        /// <returns>
        /// The size of the printable area in screen pixels. The default is 0.0F.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Size PrintableArea
        {
            get;
            private set;
        }
    }
}
