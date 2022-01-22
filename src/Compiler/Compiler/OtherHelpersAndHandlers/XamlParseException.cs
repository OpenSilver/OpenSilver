using System;

namespace DotNetForHtml5.Compiler
{
    public class XamlParseException : SystemException
    {
        ///<summary>
        /// Constructor
        ///</summary>
        public XamlParseException()
            : base()
        {
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="message">
        /// Exception message
        ///</param>
        public XamlParseException(string message)
            : base(message)
        {
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="message">Exception message</param>
        ///<param name="innerException">exception occured</param>
        public XamlParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="message">
        /// Exception message
        ///</param>
        ///<param name="lineNumber">
        /// lineNumber the exception occured at
        ///</param>
        ///<param name="linePosition">
        /// LinePosition the Exception occured at.
        ///</param>
        /// <ExternalAPI/> 
        public XamlParseException(string message, int lineNumber, int linePosition)
            : this(message)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="message">
        /// Exception message
        ///</param>
        ///<param name="lineNumber">
        /// lineNumber the exception occured at
        ///</param>
        ///<param name="linePosition">
        /// LinePosition the Exception occured at.
        ///</param>
        ///<param name="innerException">
        /// original Exception that was thrown.
        ///</param>
        public XamlParseException(string message, int lineNumber, int linePosition, Exception innerException)
            : this(message, innerException)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }

        internal XamlParseException(string message, int lineNumber, int linePosition, Uri baseUri, Exception innerException)
            : this(message, innerException)
        {
            LineNumber = lineNumber;
            LinePosition = linePosition;
            BaseUri = baseUri;
        }

        ///<summary>
        /// LineNumber that the exception occured on.
        ///</summary>
        public int LineNumber { get; }

        ///<summary>
        /// LinePosition that the exception occured on.
        ///</summary>
        public int LinePosition { get; }

        ///<summary>
        /// If this is set, it indicates that the Xaml exception occurred
        /// in the context of a dictionary item, and this was the Xaml Key
        /// value of that item.
        ///</summary>

        public object KeyContext { get; }

        ///<summary>
        /// If this is set, it indicates that the Xaml exception occurred
        /// in the context of an object with a Xaml Uid set, and this was the
        /// value of that Uid.
        ///</summary>
        public string UidContext { get; }

        ///<summary>
        /// If this is set, it indicates that the Xaml exception occurred
        /// in the context of an object with a Xaml Name set, and this was the
        /// value of that name.
        ///</summary>
        public string NameContext { get; }

        ///<summary>
        /// The BaseUri in effect at the point of the exception.
        ///</summary>
        public Uri BaseUri { get; }
    }
}