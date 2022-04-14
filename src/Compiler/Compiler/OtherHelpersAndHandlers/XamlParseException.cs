using System;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Xml;

namespace DotNetForHtml5.Compiler
{
    [Serializable]
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

        public XamlParseException(string message, IXmlLineInfo lineInfo)
            : this(message)
        {
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                LineNumber = lineInfo.LineNumber;
                LinePosition = lineInfo.LinePosition;
            }
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

        public XamlParseException(string message, IXmlLineInfo lineInfo, Exception innerException)
            : this(message, innerException)
        {
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                LineNumber = lineInfo.LineNumber;
                LinePosition = lineInfo.LinePosition;
            }
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

        /// <summary>
        /// Internal constructor used for serialization when marshalling an
        /// exception of this type across and AppDomain or machine boundary.
        /// </summary>
        /// <param name="info">
        /// Contains all the information needed to serialize or deserialize
        /// the object.
        /// </param>
        /// <param name="context">
        /// Describes the source and destination of a given serialized stream,
        /// as well as a means for serialization to retain that context and an
        /// additional caller-defined context.
        /// </param>
        protected XamlParseException(
            SerializationInfo info,
            StreamingContext context
            )
            : base(info, context)
        {
            LineNumber = info.GetInt32("Line");
            LinePosition = info.GetInt32("Position");
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">
        /// The SerializationInfo to populate with data.
        /// </param>
        /// <param name="context">
        /// The destination for this serialization.
        /// </param>
        ///
        /// <SecurityNote>
        ///     Critical: calls Exception.GetObjectData which LinkDemands
        /// </SecurityNote>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Line", LineNumber);
            info.AddValue("Position", LinePosition);
        }
    }
}