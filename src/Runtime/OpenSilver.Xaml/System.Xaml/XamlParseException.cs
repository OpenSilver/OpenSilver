using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xaml
{
	public class XamlParseException : XamlException
	{
		public XamlParseException()
			: this("XAML parse error")
		{
		}

		public XamlParseException(string message)
			: this(message, null)
		{
		}

		public XamlParseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal XamlParseException(string message, Exception innerException, int lineNumber, int linePosition)
			: base(message, innerException, lineNumber, linePosition)
		{
		}

		protected XamlParseException (SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
		}
	}
}
