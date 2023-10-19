
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
    /// Provides device specific settings for printers that have vector printing limitations.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class PrinterFallbackSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrinterFallbackSettings"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public PrinterFallbackSettings()
        {
        }

        /// <summary>
        /// Gets or sets whether all printing is forced to print in vector format.
        /// </summary>
        /// <returns>
        /// true to indicate all printing is forced to print in vector format; otherwise
        /// false. The default is true.
        /// </returns>
        [OpenSilver.NotImplemented]
        public bool ForceVector
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the opacity value of visual elements at which Silverlight will round
        /// the opacity to 1.0 to support vector printing on PostScript printers or drivers.
        /// </summary>
        /// <returns>
        /// The opacity value of visual elements at which Silverlight rounds the opacity
        /// to 1.0 to support vector printing on Postscript printers or drivers. The default
        /// is 0.
        /// </returns>
        [OpenSilver.NotImplemented]
        public double OpacityThreshold
        {
            get;
            set;
        }
    }
}
