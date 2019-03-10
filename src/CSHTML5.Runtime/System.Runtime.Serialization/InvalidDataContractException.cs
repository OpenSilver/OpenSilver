
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


#if !BRIDGE

using System;

namespace System.Runtime.Serialization
{
    /// <summary>
    /// The exception that is thrown when the DataContractSerializer encounters an invalid data contract during serialization and deserialization.
    /// </summary>
    public class InvalidDataContractException : Exception
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