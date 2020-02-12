
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



#if !BRIDGE

using System;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// The exception that is thrown when the DataContractSerializer encounters an invalid data contract during serialization and deserialization.
    /// </summary>
    public partial class InvalidDataContractException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidDataContractException class.
        /// </summary>
        public InvalidDataContractException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidDataContractException class with the specified error message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public InvalidDataContractException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidDataContractException
        /// </summary>
        /// <param name="message">A description of the error.</param>
        /// <param name="innerException">The original Exception.</param>
        public InvalidDataContractException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

#endif