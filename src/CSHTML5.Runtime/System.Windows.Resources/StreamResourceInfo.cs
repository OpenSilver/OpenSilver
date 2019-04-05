
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    public class StreamResourceInfo
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Resources.StreamResourceInfo
        /// class.
        /// </summary>
        public StreamResourceInfo(){ } //don't know what this is good for since Stream and ContentType are both read-only.
       
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
