

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Markup
{
    public partial class XamlParseException : SystemException
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Markup.XamlParseException
        /// class.
        /// </summary>
        public XamlParseException()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the System.Windows.Markup.XamlParseException
        /// class, using the specified exception message string.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public XamlParseException(string message)
            : base(message)
        {
        }
       
        /// <summary>
        /// Initializes a new instance of the System.Windows.Markup.XamlParseException
        /// class, using the specified exception message string and inner exception.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The initial exception that occurred.</param>
        public XamlParseException(string message, Exception innerException)
            : base (message, innerException)
        {
        }

        internal void SetLineInfo(int lineNumber, int linePosition)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        internal int LineNumber { get; private set; }
        internal int LinePosition { get; private set; }
    }
}
