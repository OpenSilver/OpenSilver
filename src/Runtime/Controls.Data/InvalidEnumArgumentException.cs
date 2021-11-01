// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace System.ComponentModel
{
    // 


    /// <summary>
    /// The exception thrown when using invalid arguments that are enumerators. 
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Exception(SerializationInfo, StreamingContext) does not exist in Silverlight, so neither can the derived InvalidEnumArgumentException constructor")]
    [Obsolete("InvalidEnumArgumentException is obsolete. Use ArgumentException instead.")]
    public class InvalidEnumArgumentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the InvalidEnumArgumentException class without a message.
        /// </summary>
        public InvalidEnumArgumentException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidEnumArgumentException class with the specified message. 
        /// </summary>
        /// <param name="message">The message to display with this exception.</param>
        public InvalidEnumArgumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidEnumArgumentException class with the specified detailed description and the specified exception.
        /// </summary>
        /// <param name="message">A detailed description of the error.</param>
        /// <param name="innerException">A reference to the inner exception that is the cause of this exception.</param>
        public InvalidEnumArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidEnumArgumentException class with a message generated from the argument, the invalid value, and an enumeration class.
        /// </summary>
        /// <param name="argumentName">The name of the argument that caused the exception.</param>
        /// <param name="invalidValue">The value of the argument that failed.</param>
        /// <param name="enumClass">A Type that represents the enumeration class with the valid values.</param>
        public InvalidEnumArgumentException(string argumentName, int invalidValue, System.Type enumClass)
            : base(String.Format(CultureInfo.CurrentCulture, Resource.InvalidEnumArgumentException_InvalidEnumArgument, argumentName, invalidValue.ToString(CultureInfo.CurrentCulture), enumClass.Name))
        {
        }
    }
}
