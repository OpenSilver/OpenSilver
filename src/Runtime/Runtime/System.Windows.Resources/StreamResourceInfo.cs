

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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Resources
{
    /// <summary>
    /// Provides resource stream information for application resources or other packages
    /// obtained through the System.Windows.Application.GetResourceStream(System.Windows.Resources.StreamResourceInfo,System.Uri)
    /// method.
    /// </summary>
    public partial class StreamResourceInfo
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Resources.StreamResourceInfo
        /// class based on a provided stream.
        /// </summary>
        /// <param name="stream">The reference stream.</param>
        /// <param name="contentType">The Multipurpose Internet Mail Extensions (MIME) content type of the stream.</param>
        public StreamResourceInfo(Stream stream, string contentType)
        {
            Stream = stream;
            ContentType = contentType;
        }

        string _contentType;
        // Returns:
        //     The Multipurpose Internet Mail Extensions (MIME) content type.
        /// <summary>
        /// Gets the content type of a stream.
        /// </summary>
        public string ContentType
        {
            get{return _contentType;}
            private set { _contentType = value; }
        }

        Stream _stream;
        /// <summary>
        /// Gets the actual stream of the resource.
        /// </summary>
        public Stream Stream
        {
            get { return _stream; }
            private set { _stream = value; }
        }
    }
}
