using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace OpenSilver.IO.IsolatedStorage
{
    /// <summary>The exception that is thrown when an operation in isolated storage fails.</summary>
    [Serializable]
    public class IsolatedStorageException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageException" /> class with default properties.</summary>
        public IsolatedStorageException()
            : base("An Isolated storage operation failed.")
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageException" /> class with a specified error message.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public IsolatedStorageException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception. If the <paramref name="inner" /> parameter is not <see langword="null" />, the current exception is raised in a <see langword="catch" /> block that handles the inner exception.</param>
        public IsolatedStorageException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.IO.IsolatedStorage.IsolatedStorageException" /> class with serialized data.</summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected IsolatedStorageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}