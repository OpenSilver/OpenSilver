

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