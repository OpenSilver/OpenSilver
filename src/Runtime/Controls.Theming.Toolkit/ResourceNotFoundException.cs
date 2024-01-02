// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace System.Windows.Controls.Theming
{
    /// <summary>
    /// Exception thrown when resource is not found.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// Gets the uri of the resource that could not be found.
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundException class.
        /// </summary>
        public ResourceNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ResourceNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="uri">The uri of the resource.</param>
        public ResourceNotFoundException(string message, Uri uri)
            : base(message)
        {
            Uri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the ResourceNotFoundException class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="uri">The uri of the resource.</param>
        /// <param name="innerException">The inner exception.</param>
        public ResourceNotFoundException(string message, Uri uri, Exception innerException)
            : base(message, innerException)
        {
            Uri = uri;
        }
    }
}