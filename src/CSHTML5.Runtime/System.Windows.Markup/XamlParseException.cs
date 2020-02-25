
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
    }
}
