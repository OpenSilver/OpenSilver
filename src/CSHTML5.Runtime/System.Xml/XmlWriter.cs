
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml
{

    // imcomplete
    //todo move to its own file?
    public enum WriteState
    {
        Nothing,
    };


    /// <summary>
    /// Represents a writer that provides a fast, non-cached, forward-only means
    /// of generating streams or files containing XML data.
    /// </summary>
#if !BRIDGE
    [JSIL.Meta.JSStubOnly]
#endif
    public abstract partial class XmlWriter : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the System.Xml.XmlWriter class.
        /// </summary>
        protected XmlWriter() { }

        // Exceptions:
        //   System.InvalidOperationException:
        //     A call is made to write more output after Close has been called or the result
        //     of this call is an invalid XML document.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlWriter method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, closes this stream and the underlying
        /// stream.
        /// </summary>
        public virtual void Close() { throw new NotImplementedException(); }

        // Exceptions:
        //   System.ArgumentNullException:
        //     The stream value is null.
        /// <summary>
        /// Creates a new System.Xml.XmlWriter instance using the specified stream.
        /// </summary>
        /// <param name="output">
        /// The stream to which you want to write. The System.Xml.XmlWriter writes XML
        /// 1.0 text syntax and appends it to the specified stream.
        /// </param>
        /// <returns>An System.Xml.XmlWriter object.</returns>
        public static XmlWriter Create(Stream output)
        {
#if BRIDGE
            return Cshtml5_XmlWriter.Create(output);
#else
            throw new NotImplementedException();
#endif
            
        }


        // Exceptions:
        //   System.InvalidOperationException:
        //     The state of writer is not WriteState.Element or writer is closed.
        //
        //   System.ArgumentException:
        //     The xml:space or xml:lang attribute value is invalid.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlWriter method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, writes out the attribute with the specified
        /// local name and value.
        /// </summary>
        /// <param name="localName">The local name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void WriteAttributeString(string localName, string value)
        {
#if BRIDGE
            if (this is Cshtml5_XmlWriter)
                ((Cshtml5_XmlWriter)this).WriteAttributeString(localName, value);
#else
            throw new NotImplementedException();
#endif
        }

        // Exceptions:
        //   System.ArgumentException:
        //     The localName value is null or an empty string.-or-The parameter values are
        //     not valid.
        //
        //   System.Text.EncoderFallbackException:
        //     There is a character in the buffer that is a valid XML character but is not
        //     valid for the output encoding. For example, if the output encoding is ASCII,
        //     you should only use characters from the range of 0 to 127 for element and
        //     attribute names. The invalid character might be in the argument of this method
        //     or in an argument of previous methods that were writing to the buffer. Such
        //     characters are escaped by character entity references when possible (for
        //     example, in text nodes or attribute values). However, the character entity
        //     reference is not allowed in element and attribute names, comments, processing
        //     instructions, or CDATA sections.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlWriter method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// Writes an element with the specified local name and value.
        /// </summary>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="value">The value of the element.</param>
        public void WriteElementString(string localName, string value)
        {
#if BRIDGE
            if (this is Cshtml5_XmlWriter)
                ((Cshtml5_XmlWriter)this).WriteElementString(localName, value);
#else
            throw new NotImplementedException();
#endif
        }

 
        public void WriteStartElement(string localName)
        {
#if BRIDGE
            if (this is Cshtml5_XmlWriter)
                ((Cshtml5_XmlWriter)this).WriteStartElement(localName);
#else
            throw new NotImplementedException();
#endif
        }

        // Exceptions:
        //   System.InvalidOperationException:
        //     The writer is closed.
        //
        //   System.Text.EncoderFallbackException:
        //     There is a character in the buffer that is a valid XML character but is not
        //     valid for the output encoding. For example, if the output encoding is ASCII,
        //     you should only use characters from the range of 0 to 127 for element and
        //     attribute names. The invalid character might be in the argument of this method
        //     or in an argument of previous methods that were writing to the buffer. Such
        //     characters are escaped by character entity references when possible (for
        //     example, in text nodes or attribute values). However, the character entity
        //     reference is not allowed in element and attribute names, comments, processing
        //     instructions, or CDATA sections.
        //
        //   System.InvalidOperationException:
        //     An System.Xml.XmlWriter method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// When overridden in a derived class, writes the specified start tag and associates
        /// it with the given namespace.
        /// </summary>
        /// <param name="localName">The local name of the element.</param>
        /// <param name="ns">
        /// The namespace URI to associate with the element. If this namespace is already
        /// in scope and has an associated prefix, the writer automatically writes that
        /// prefix also.
        /// </param>
        public void WriteStartElement(string localName, string ns)
        {
#if BRIDGE
            if (this is Cshtml5_XmlWriter)
                ((Cshtml5_XmlWriter)this).WriteStartElement(string.Empty, localName, ns);
#else
            throw new NotImplementedException();
#endif
        }



        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlWriter method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// Releases all resources used by the current instance of the System.Xml.XmlWriter
        /// class.
        /// </summary>
        public void Dispose()
        {
#if BRIDGE
            // do nothing, because there is nothing to dispose
#else
            throw new NotImplementedException();
#endif
        }

        // Exceptions:
        //   System.InvalidOperationException:
        //     An System.Xml.XmlWriter method was called before a previous asynchronous
        //     operation finished. In this case, System.InvalidOperationException is thrown
        //     with the message “An asynchronous operation is already in progress.”
        /// <summary>
        /// Releases the unmanaged resources used by the System.Xml.XmlWriter and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) { throw new NotImplementedException(); }










#region Abstract

        public abstract void WriteStartDocument();

        public abstract void WriteStartDocument(bool standalone);

        public abstract void WriteEndDocument();

        public abstract void WriteDocType(string name, string pubid, string sysid, string subset);

        public abstract void WriteStartElement(string prefix, string localName, string ns);

        public abstract void WriteEndElement();

        public abstract void WriteFullEndElement();

        public abstract void WriteStartAttribute(string prefix, string localName, string ns);

        public abstract void WriteEndAttribute();

        public abstract void WriteCData(string text);

        public abstract void WriteComment(string text);

        public abstract void WriteProcessingInstruction(string name, string text);

        public abstract void WriteEntityRef(string name);

        public abstract void WriteCharEntity(char ch);

        public abstract void WriteWhitespace(string ws);

        public abstract void WriteString(string text);

        public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);

        public abstract void WriteChars(char[] buffer, int index, int count);

        public abstract void WriteRaw(char[] buffer, int index, int count);

        public abstract void WriteRaw(string data);

        public abstract void WriteBase64(byte[] buffer, int index, int count);

        public abstract WriteState WriteState { get; }

        public abstract void Flush();

        public abstract string LookupPrefix(string ns);

#endregion

        //// Summary:
        ////     Gets the System.Xml.XmlWriterSettings object used to create this System.Xml.XmlWriter
        ////     instance.
        ////
        //// Returns:
        ////     The System.Xml.XmlWriterSettings object used to create this writer instance.
        ////     If this writer was not created using the Overload:System.Xml.XmlWriter.Create
        ////     method, this property returns null.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual XmlWriterSettings Settings { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the state of the writer.
        ////
        //// Returns:
        ////     One of the System.Xml.WriteState values.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract WriteState WriteState { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets the current xml:lang scope.
        ////
        //// Returns:
        ////     The current xml:lang scope.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual string XmlLang { get; }
        ////
        //// Summary:
        ////     When overridden in a derived class, gets an System.Xml.XmlSpace representing
        ////     the current xml:space scope.
        ////
        //// Returns:
        ////     An XmlSpace representing the current xml:space scope.Value Meaning NoneThis
        ////     is the default if no xml:space scope exists.DefaultThe current scope is xml:space="default".PreserveThe
        ////     current scope is xml:space="preserve".
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual XmlSpace XmlSpace { get; }

       
        
        ////
        //// Summary:
        ////     Creates a new System.Xml.XmlWriter instance using the specified filename.
        ////
        //// Parameters:
        ////   outputFileName:
        ////     The file to which you want to write. The System.Xml.XmlWriter creates a file
        ////     at the specified path and writes to it in XML 1.0 text syntax. The outputFileName
        ////     must be a file system path.
        ////
        //// Returns:
        ////     An System.Xml.XmlWriter object.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The url value is null.
        //public static XmlWriter Create(string outputFileName);
        ////
        //// Summary:
        ////     Creates a new System.Xml.XmlWriter instance using the specified System.Text.StringBuilder.
        ////
        //// Parameters:
        ////   output:
        ////     The System.Text.StringBuilder to which to write to. Content written by the
        ////     System.Xml.XmlWriter is appended to the System.Text.StringBuilder.
        ////
        //// Returns:
        ////     An System.Xml.XmlWriter object.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The builder value is null.
        //public static XmlWriter Create(StringBuilder output);
        ////
        //// Summary:
        ////     Creates a new System.Xml.XmlWriter instance using the specified System.IO.TextWriter.
        ////
        //// Parameters:
        ////   output:
        ////     The System.IO.TextWriter to which you want to write. The System.Xml.XmlWriter
        ////     writes XML 1.0 text syntax and appends it to the specified System.IO.TextWriter.
        ////
        //// Returns:
        ////     An System.Xml.XmlWriter object.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The text value is null.
        //public static XmlWriter Create(TextWriter output);
        ////
        //// Summary:
        ////     Creates a new System.Xml.XmlWriter instance using the specified System.Xml.XmlWriter
        ////     object.
        ////
        //// Parameters:
        ////   output:
        ////     The System.Xml.XmlWriter object that you want to use as the underlying writer.
        ////
        //// Returns:
        ////     An System.Xml.XmlWriter object that is wrapped around the specified System.Xml.XmlWriter
        ////     object.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The writer value is null.
        //public static XmlWriter Create(XmlWriter output);
        
        ////
        //// Summary:
        ////     Creates a new System.Xml.XmlWriter instance using the filename and System.Xml.XmlWriterSettings
        ////     object.
        ////
        //// Parameters:
        ////   outputFileName:
        ////     The file to which you want to write. The System.Xml.XmlWriter creates a file
        ////     at the specified path and writes to it in XML 1.0 text syntax. The outputFileName
        ////     must be a file system path.
        ////
        ////   settings:
        ////     The System.Xml.XmlWriterSettings object used to configure the new System.Xml.XmlWriter
        ////     instance. If this is null, a System.Xml.XmlWriterSettings with default settings
        ////     is used.If the System.Xml.XmlWriter is being used with the System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)
        ////     method, you should use the System.Xml.Xsl.XslCompiledTransform.OutputSettings
        ////     property to obtain an System.Xml.XmlWriterSettings object with the correct
        ////     settings. This ensures that the created System.Xml.XmlWriter object has the
        ////     correct output settings.
        ////
        //// Returns:
        ////     An System.Xml.XmlWriter object.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The url value is null.
        //public static XmlWriter Create(string outputFileName, XmlWriterSettings settings);
        ////
        //// Summary:
        ////     Creates a new System.Xml.XmlWriter instance using the System.Text.StringBuilder
        ////     and System.Xml.XmlWriterSettings objects.
        ////
        //// Parameters:
        ////   output:
        ////     The System.Text.StringBuilder to which to write to. Content written by the
        ////     System.Xml.XmlWriter is appended to the System.Text.StringBuilder.
        ////
        ////   settings:
        ////     The System.Xml.XmlWriterSettings object used to configure the new System.Xml.XmlWriter
        ////     instance. If this is null, a System.Xml.XmlWriterSettings with default settings
        ////     is used.If the System.Xml.XmlWriter is being used with the System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)
        ////     method, you should use the System.Xml.Xsl.XslCompiledTransform.OutputSettings
        ////     property to obtain an System.Xml.XmlWriterSettings object with the correct
        ////     settings. This ensures that the created System.Xml.XmlWriter object has the
        ////     correct output settings.
        ////
        //// Returns:
        ////     An System.Xml.XmlWriter object.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The builder value is null.
        //public static XmlWriter Create(StringBuilder output, XmlWriterSettings settings);
        ////
        //// Summary:
        ////     Creates a new System.Xml.XmlWriter instance using the System.IO.TextWriter
        ////     and System.Xml.XmlWriterSettings objects.
        ////
        //// Parameters:
        ////   output:
        ////     The System.IO.TextWriter to which you want to write. The System.Xml.XmlWriter
        ////     writes XML 1.0 text syntax and appends it to the specified System.IO.TextWriter.
        ////
        ////   settings:
        ////     The System.Xml.XmlWriterSettings object used to configure the new System.Xml.XmlWriter
        ////     instance. If this is null, a System.Xml.XmlWriterSettings with default settings
        ////     is used.If the System.Xml.XmlWriter is being used with the System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)
        ////     method, you should use the System.Xml.Xsl.XslCompiledTransform.OutputSettings
        ////     property to obtain an System.Xml.XmlWriterSettings object with the correct
        ////     settings. This ensures that the created System.Xml.XmlWriter object has the
        ////     correct output settings.
        ////
        //// Returns:
        ////     An System.Xml.XmlWriter object.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The text value is null.
        //public static XmlWriter Create(TextWriter output, XmlWriterSettings settings);
        ////
        //// Summary:
        ////     Creates a new System.Xml.XmlWriter instance using the specified System.Xml.XmlWriter
        ////     and System.Xml.XmlWriterSettings objects.
        ////
        //// Parameters:
        ////   output:
        ////     The System.Xml.XmlWriter object that you want to use as the underlying writer.
        ////
        ////   settings:
        ////     The System.Xml.XmlWriterSettings object used to configure the new System.Xml.XmlWriter
        ////     instance. If this is null, a System.Xml.XmlWriterSettings with default settings
        ////     is used.If the System.Xml.XmlWriter is being used with the System.Xml.Xsl.XslCompiledTransform.Transform(System.String,System.Xml.XmlWriter)
        ////     method, you should use the System.Xml.Xsl.XslCompiledTransform.OutputSettings
        ////     property to obtain an System.Xml.XmlWriterSettings object with the correct
        ////     settings. This ensures that the created System.Xml.XmlWriter object has the
        ////     correct output settings.
        ////
        //// Returns:
        ////     An System.Xml.XmlWriter object that is wrapped around the specified System.Xml.XmlWriter
        ////     object.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     The writer value is null.
        //public static XmlWriter Create(XmlWriter output, XmlWriterSettings settings);
        
        
        ////
        //// Summary:
        ////     Asynchronously flushes whatever is in the buffer to the underlying streams
        ////     and also flushes the underlying stream.
        ////
        //// Returns:
        ////     The task that represents the asynchronous Flush operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task FlushAsync();
        ////
        //// Summary:
        ////     When overridden in a derived class, returns the closest prefix defined in
        ////     the current namespace scope for the namespace URI.
        ////
        //// Parameters:
        ////   ns:
        ////     The namespace URI whose prefix you want to find.
        ////
        //// Returns:
        ////     The matching prefix or null if no matching namespace URI is found in the
        ////     current scope.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     ns is either null or String.Empty.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract string LookupPrefix(string ns);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out all the attributes found at
        ////     the current position in the System.Xml.XmlReader.
        ////
        //// Parameters:
        ////   reader:
        ////     The XmlReader from which to copy the attributes.
        ////
        ////   defattr:
        ////     true to copy the default attributes from the XmlReader; otherwise, false.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     reader is null.
        ////
        ////   System.Xml.XmlException:
        ////     The reader is not positioned on an element, attribute or XmlDeclaration node.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteAttributes(XmlReader reader, bool defattr);
        ////
        //// Summary:
        ////     Asynchronously writes out all the attributes found at the current position
        ////     in the System.Xml.XmlReader.
        ////
        //// Parameters:
        ////   reader:
        ////     The XmlReader from which to copy the attributes.
        ////
        ////   defattr:
        ////     true to copy the default attributes from the XmlReader; otherwise, false.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteAttributes operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task WriteAttributesAsync(XmlReader reader, bool defattr);
        
        ////
        //// Summary:
        ////     When overridden in a derived class, writes an attribute with the specified
        ////     local name, namespace URI, and value.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the attribute.
        ////
        ////   ns:
        ////     The namespace URI to associate with the attribute.
        ////
        ////   value:
        ////     The value of the attribute.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The state of writer is not WriteState.Element or writer is closed.
        ////
        ////   System.ArgumentException:
        ////     The xml:space or xml:lang attribute value is invalid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        //public void WriteAttributeString(string localName, string ns, string value);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out the attribute with the specified
        ////     prefix, local name, namespace URI, and value.
        ////
        //// Parameters:
        ////   prefix:
        ////     The namespace prefix of the attribute.
        ////
        ////   localName:
        ////     The local name of the attribute.
        ////
        ////   ns:
        ////     The namespace URI of the attribute.
        ////
        ////   value:
        ////     The value of the attribute.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The state of writer is not WriteState.Element or writer is closed.
        ////
        ////   System.ArgumentException:
        ////     The xml:space or xml:lang attribute value is invalid.
        ////
        ////   System.Xml.XmlException:
        ////     The localName or ns is null.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public void WriteAttributeString(string prefix, string localName, string ns, string value);
        ////
        //// Summary:
        ////     Asynchronously writes out the attribute with the specified prefix, local
        ////     name, namespace URI, and value.
        ////
        //// Parameters:
        ////   prefix:
        ////     The namespace prefix of the attribute.
        ////
        ////   localName:
        ////     The local name of the attribute.
        ////
        ////   ns:
        ////     The namespace URI of the attribute.
        ////
        ////   value:
        ////     The value of the attribute.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteAttributeString operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        //public Task WriteAttributeStringAsync(string prefix, string localName, string ns, string value);
        ////
        //// Summary:
        ////     When overridden in a derived class, encodes the specified binary bytes as
        ////     Base64 and writes out the resulting text.
        ////
        //// Parameters:
        ////   buffer:
        ////     Byte array to encode.
        ////
        ////   index:
        ////     The position in the buffer indicating the start of the bytes to write.
        ////
        ////   count:
        ////     The number of bytes to write.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     buffer is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     index or count is less than zero. -or-The buffer length minus index is less
        ////     than count.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteBase64(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously encodes the specified binary bytes as Base64 and writes out
        ////     the resulting text.
        ////
        //// Parameters:
        ////   buffer:
        ////     Byte array to encode.
        ////
        ////   index:
        ////     The position in the buffer indicating the start of the bytes to write.
        ////
        ////   count:
        ////     The number of bytes to write.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteBase64 operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteBase64Async(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     When overridden in a derived class, encodes the specified binary bytes as
        ////     BinHex and writes out the resulting text.
        ////
        //// Parameters:
        ////   buffer:
        ////     Byte array to encode.
        ////
        ////   index:
        ////     The position in the buffer indicating the start of the bytes to write.
        ////
        ////   count:
        ////     The number of bytes to write.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     buffer is null.
        ////
        ////   System.InvalidOperationException:
        ////     The writer is closed or in error state.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     index or count is less than zero. -or-The buffer length minus index is less
        ////     than count.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteBinHex(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously encodes the specified binary bytes as BinHex and writes out
        ////     the resulting text.
        ////
        //// Parameters:
        ////   buffer:
        ////     Byte array to encode.
        ////
        ////   index:
        ////     The position in the buffer indicating the start of the bytes to write.
        ////
        ////   count:
        ////     The number of bytes to write.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteBinHex operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteBinHexAsync(byte[] buffer, int index, int count);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out a <![CDATA[...]]> block containing
        ////     the specified text.
        ////
        //// Parameters:
        ////   text:
        ////     The text to place inside the CDATA block.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The text would result in a non-well formed XML document.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteCData(string text);
        ////
        //// Summary:
        ////     Asynchronously writes out a <![CDATA[...]]> block containing the specified
        ////     text.
        ////
        //// Parameters:
        ////   text:
        ////     The text to place inside the CDATA block.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteCData operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteCDataAsync(string text);
        ////
        //// Summary:
        ////     When overridden in a derived class, forces the generation of a character
        ////     entity for the specified Unicode character value.
        ////
        //// Parameters:
        ////   ch:
        ////     The Unicode character for which to generate a character entity.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The character is in the surrogate pair character range, 0xd800 - 0xdfff.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteCharEntity(char ch);
        ////
        //// Summary:
        ////     Asynchronously forces the generation of a character entity for the specified
        ////     Unicode character value.
        ////
        //// Parameters:
        ////   ch:
        ////     The Unicode character for which to generate a character entity.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteCharEntity operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteCharEntityAsync(char ch);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes text one buffer at a time.
        ////
        //// Parameters:
        ////   buffer:
        ////     Character array containing the text to write.
        ////
        ////   index:
        ////     The position in the buffer indicating the start of the text to write.
        ////
        ////   count:
        ////     The number of characters to write.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     buffer is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     index or count is less than zero.-or-The buffer length minus index is less
        ////     than count; the call results in surrogate pair characters being split or
        ////     an invalid surrogate pair being written.
        ////
        ////   System.ArgumentException:
        ////     The buffer parameter value is not valid.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteChars(char[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously writes text one buffer at a time.
        ////
        //// Parameters:
        ////   buffer:
        ////     Character array containing the text to write.
        ////
        ////   index:
        ////     The position in the buffer indicating the start of the text to write.
        ////
        ////   count:
        ////     The number of characters to write.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteChars operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteCharsAsync(char[] buffer, int index, int count);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out a comment <!--...--> containing
        ////     the specified text.
        ////
        //// Parameters:
        ////   text:
        ////     Text to place inside the comment.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The text would result in a non-well-formed XML document.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteComment(string text);
        ////
        //// Summary:
        ////     Asynchronously writes out a comment <!--...--> containing the specified text.
        ////
        //// Parameters:
        ////   text:
        ////     Text to place inside the comment.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteComment operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteCommentAsync(string text);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes the DOCTYPE declaration with the
        ////     specified name and optional attributes.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the DOCTYPE. This must be non-empty.
        ////
        ////   pubid:
        ////     If non-null it also writes PUBLIC "pubid" "sysid" where pubid and sysid are
        ////     replaced with the value of the given arguments.
        ////
        ////   sysid:
        ////     If pubid is null and sysid is non-null it writes SYSTEM "sysid" where sysid
        ////     is replaced with the value of this argument.
        ////
        ////   subset:
        ////     If non-null it writes [subset] where subset is replaced with the value of
        ////     this argument.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     This method was called outside the prolog (after the root element).
        ////
        ////   System.ArgumentException:
        ////     The value for name would result in invalid XML.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteDocType(string name, string pubid, string sysid, string subset);
        ////
        //// Summary:
        ////     Asynchronously writes the DOCTYPE declaration with the specified name and
        ////     optional attributes.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the DOCTYPE. This must be non-empty.
        ////
        ////   pubid:
        ////     If non-null it also writes PUBLIC "pubid" "sysid" where pubid and sysid are
        ////     replaced with the value of the given arguments.
        ////
        ////   sysid:
        ////     If pubid is null and sysid is non-null it writes SYSTEM "sysid" where sysid
        ////     is replaced with the value of this argument.
        ////
        ////   subset:
        ////     If non-null it writes [subset] where subset is replaced with the value of
        ////     this argument.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteDocType operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteDocTypeAsync(string name, string pubid, string sysid, string subset);
        
        ////
        //// Summary:
        ////     Writes an element with the specified local name, namespace URI, and value.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the element.
        ////
        ////   ns:
        ////     The namespace URI to associate with the element.
        ////
        ////   value:
        ////     The value of the element.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The localName value is null or an empty string.-or-The parameter values are
        ////     not valid.
        ////
        ////   System.Text.EncoderFallbackException:
        ////     There is a character in the buffer that is a valid XML character but is not
        ////     valid for the output encoding. For example, if the output encoding is ASCII,
        ////     you should only use characters from the range of 0 to 127 for element and
        ////     attribute names. The invalid character might be in the argument of this method
        ////     or in an argument of previous methods that were writing to the buffer. Such
        ////     characters are escaped by character entity references when possible (for
        ////     example, in text nodes or attribute values). However, the character entity
        ////     reference is not allowed in element and attribute names, comments, processing
        ////     instructions, or CDATA sections.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public void WriteElementString(string localName, string ns, string value);
        ////
        //// Summary:
        ////     Writes an element with the specified prefix, local name, namespace URI, and
        ////     value.
        ////
        //// Parameters:
        ////   prefix:
        ////     The prefix of the element.
        ////
        ////   localName:
        ////     The local name of the element.
        ////
        ////   ns:
        ////     The namespace URI of the element.
        ////
        ////   value:
        ////     The value of the element.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The localName value is null or an empty string.-or-The parameter values are
        ////     not valid.
        ////
        ////   System.Text.EncoderFallbackException:
        ////     There is a character in the buffer that is a valid XML character but is not
        ////     valid for the output encoding. For example, if the output encoding is ASCII,
        ////     you should only use characters from the range of 0 to 127 for element and
        ////     attribute names. The invalid character might be in the argument of this method
        ////     or in an argument of previous methods that were writing to the buffer. Such
        ////     characters are escaped by character entity references when possible (for
        ////     example, in text nodes or attribute values). However, the character entity
        ////     reference is not allowed in element and attribute names, comments, processing
        ////     instructions, or CDATA sections.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public void WriteElementString(string prefix, string localName, string ns, string value);
        ////
        //// Summary:
        ////     Asynchronously writes an element with the specified prefix, local name, namespace
        ////     URI, and value.
        ////
        //// Parameters:
        ////   prefix:
        ////     The prefix of the element.
        ////
        ////   localName:
        ////     The local name of the element.
        ////
        ////   ns:
        ////     The namespace URI of the element.
        ////
        ////   value:
        ////     The value of the element.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteElementString operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public Task WriteElementStringAsync(string prefix, string localName, string ns, string value);
        ////
        //// Summary:
        ////     When overridden in a derived class, closes the previous System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)
        ////     call.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteEndAttribute();
        ////
        //// Summary:
        ////     Asynchronously closes the previous System.Xml.XmlWriter.WriteStartAttribute(System.String,System.String)
        ////     call.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteEndAttribute operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //protected internal virtual Task WriteEndAttributeAsync();
        
        ////
        //// Summary:
        ////     Asynchronously closes any open elements or attributes and puts the writer
        ////     back in the Start state.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteEndDocument operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteEndDocumentAsync();
        
        ////
        //// Summary:
        ////     Asynchronously closes one element and pops the corresponding namespace scope.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteEndElement operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteEndElementAsync();
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out an entity reference as &name;.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the entity reference.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     name is either null or String.Empty.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteEntityRef(string name);
        ////
        //// Summary:
        ////     Asynchronously writes out an entity reference as &name;.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the entity reference.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteEntityRef operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteEntityRefAsync(string name);
        ////
        //// Summary:
        ////     When overridden in a derived class, closes one element and pops the corresponding
        ////     namespace scope.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteFullEndElement();
        ////
        //// Summary:
        ////     Asynchronously closes one element and pops the corresponding namespace scope.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteFullEndElement operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteFullEndElementAsync();
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out the specified name, ensuring
        ////     it is a valid name according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        ////
        //// Parameters:
        ////   name:
        ////     The name to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     name is not a valid XML name; or name is either null or String.Empty.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteName(string name);
        ////
        //// Summary:
        ////     Asynchronously writes out the specified name, ensuring it is a valid name
        ////     according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        ////
        //// Parameters:
        ////   name:
        ////     The name to write.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteName operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteNameAsync(string name);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out the specified name, ensuring
        ////     it is a valid NmToken according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        ////
        //// Parameters:
        ////   name:
        ////     The name to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     name is not a valid NmToken; or name is either null or String.Empty.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteNmToken(string name);
        ////
        //// Summary:
        ////     Asynchronously writes out the specified name, ensuring it is a valid NmToken
        ////     according to the W3C XML 1.0 recommendation (http://www.w3.org/TR/1998/REC-xml-19980210#NT-Name).
        ////
        //// Parameters:
        ////   name:
        ////     The name to write.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteNmToken operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteNmTokenAsync(string name);
        ////
        //// Summary:
        ////     When overridden in a derived class, copies everything from the reader to
        ////     the writer and moves the reader to the start of the next sibling.
        ////
        //// Parameters:
        ////   reader:
        ////     The System.Xml.XmlReader to read from.
        ////
        ////   defattr:
        ////     true to copy the default attributes from the XmlReader; otherwise, false.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     reader is null.
        ////
        ////   System.ArgumentException:
        ////     reader contains invalid characters.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteNode(XmlReader reader, bool defattr);
        ////
        //// Summary:
        ////     Copies everything from the System.Xml.XPath.XPathNavigator object to the
        ////     writer. The position of the System.Xml.XPath.XPathNavigator remains unchanged.
        ////
        //// Parameters:
        ////   navigator:
        ////     The System.Xml.XPath.XPathNavigator to copy from.
        ////
        ////   defattr:
        ////     true to copy the default attributes; otherwise, false.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     navigator is null.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteNode(XPathNavigator navigator, bool defattr);
        ////
        //// Summary:
        ////     Asynchronously copies everything from the reader to the writer and moves
        ////     the reader to the start of the next sibling.
        ////
        //// Parameters:
        ////   reader:
        ////     The System.Xml.XmlReader to read from.
        ////
        ////   defattr:
        ////     true to copy the default attributes from the XmlReader; otherwise, false.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteNode operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteNodeAsync(XmlReader reader, bool defattr);
        ////
        //// Summary:
        ////     Asynchronously copies everything from the System.Xml.XPath.XPathNavigator
        ////     object to the writer. The position of the System.Xml.XPath.XPathNavigator
        ////     remains unchanged.
        ////
        //// Parameters:
        ////   navigator:
        ////     The System.Xml.XPath.XPathNavigator to copy from.
        ////
        ////   defattr:
        ////     true to copy the default attributes; otherwise, false.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteNode operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task WriteNodeAsync(XPathNavigator navigator, bool defattr);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out a processing instruction with
        ////     a space between the name and text as follows: <?name text?>.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the processing instruction.
        ////
        ////   text:
        ////     The text to include in the processing instruction.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The text would result in a non-well formed XML document.name is either null
        ////     or String.Empty.This method is being used to create an XML declaration after
        ////     System.Xml.XmlWriter.WriteStartDocument() has already been called.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteProcessingInstruction(string name, string text);
        ////
        //// Summary:
        ////     Asynchronously writes out a processing instruction with a space between the
        ////     name and text as follows: <?name text?>.
        ////
        //// Parameters:
        ////   name:
        ////     The name of the processing instruction.
        ////
        ////   text:
        ////     The text to include in the processing instruction.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteProcessingInstruction operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteProcessingInstructionAsync(string name, string text);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out the namespace-qualified name.
        ////     This method looks up the prefix that is in scope for the given namespace.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name to write.
        ////
        ////   ns:
        ////     The namespace URI for the name.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     localName is either null or String.Empty.localName is not a valid name.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteQualifiedName(string localName, string ns);
        ////
        //// Summary:
        ////     Asynchronously writes out the namespace-qualified name. This method looks
        ////     up the prefix that is in scope for the given namespace.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name to write.
        ////
        ////   ns:
        ////     The namespace URI for the name.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteQualifiedName operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //[DebuggerStepThrough]
        //public virtual Task WriteQualifiedNameAsync(string localName, string ns);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes raw markup manually from a string.
        ////
        //// Parameters:
        ////   data:
        ////     String containing the text to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     data is either null or String.Empty.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteRaw(string data);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes raw markup manually from a character
        ////     buffer.
        ////
        //// Parameters:
        ////   buffer:
        ////     Character array containing the text to write.
        ////
        ////   index:
        ////     The position within the buffer indicating the start of the text to write.
        ////
        ////   count:
        ////     The number of characters to write.
        ////
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     buffer is null.
        ////
        ////   System.ArgumentOutOfRangeException:
        ////     index or count is less than zero. -or-The buffer length minus index is less
        ////     than count.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteRaw(char[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Asynchronously writes raw markup manually from a string.
        ////
        //// Parameters:
        ////   data:
        ////     String containing the text to write.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteRaw operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteRawAsync(string data);
        ////
        //// Summary:
        ////     Asynchronously writes raw markup manually from a character buffer.
        ////
        //// Parameters:
        ////   buffer:
        ////     Character array containing the text to write.
        ////
        ////   index:
        ////     The position within the buffer indicating the start of the text to write.
        ////
        ////   count:
        ////     The number of characters to write.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteRaw operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteRawAsync(char[] buffer, int index, int count);
        ////
        //// Summary:
        ////     Writes the start of an attribute with the specified local name.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the attribute.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The writer is closed.
        ////
        ////   System.Text.EncoderFallbackException:
        ////     There is a character in the buffer that is a valid XML character but is not
        ////     valid for the output encoding. For example, if the output encoding is ASCII,
        ////     you should only use characters from the range of 0 to 127 for element and
        ////     attribute names. The invalid character might be in the argument of this method
        ////     or in an argument of previous methods that were writing to the buffer. Such
        ////     characters are escaped by character entity references when possible (for
        ////     example, in text nodes or attribute values). However, the character entity
        ////     reference is not allowed in element and attribute names, comments, processing
        ////     instructions, or CDATA sections.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public void WriteStartAttribute(string localName);
        ////
        //// Summary:
        ////     Writes the start of an attribute with the specified local name and namespace
        ////     URI.
        ////
        //// Parameters:
        ////   localName:
        ////     The local name of the attribute.
        ////
        ////   ns:
        ////     The namespace URI of the attribute.
        ////
        //// Exceptions:
        ////   System.Text.EncoderFallbackException:
        ////     There is a character in the buffer that is a valid XML character but is not
        ////     valid for the output encoding. For example, if the output encoding is ASCII,
        ////     you should only use characters from the range of 0 to 127 for element and
        ////     attribute names. The invalid character might be in the argument of this method
        ////     or in an argument of previous methods that were writing to the buffer. Such
        ////     characters are escaped by character entity references when possible (for
        ////     example, in text nodes or attribute values). However, the character entity
        ////     reference is not allowed in element and attribute names, comments, processing
        ////     instructions, or CDATA sections.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public void WriteStartAttribute(string localName, string ns);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes the start of an attribute with
        ////     the specified prefix, local name, and namespace URI.
        ////
        //// Parameters:
        ////   prefix:
        ////     The namespace prefix of the attribute.
        ////
        ////   localName:
        ////     The local name of the attribute.
        ////
        ////   ns:
        ////     The namespace URI for the attribute.
        ////
        //// Exceptions:
        ////   System.Text.EncoderFallbackException:
        ////     There is a character in the buffer that is a valid XML character but is not
        ////     valid for the output encoding. For example, if the output encoding is ASCII,
        ////     you should only use characters from the range of 0 to 127 for element and
        ////     attribute names. The invalid character might be in the argument of this method
        ////     or in an argument of previous methods that were writing to the buffer. Such
        ////     characters are escaped by character entity references when possible (for
        ////     example, in text nodes or attribute values). However, the character entity
        ////     reference is not allowed in element and attribute names, comments, processing
        ////     instructions, or CDATA sections.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteStartAttribute(string prefix, string localName, string ns);
        ////
        //// Summary:
        ////     Asynchronously writes the start of an attribute with the specified prefix,
        ////     local name, and namespace URI.
        ////
        //// Parameters:
        ////   prefix:
        ////     The namespace prefix of the attribute.
        ////
        ////   localName:
        ////     The local name of the attribute.
        ////
        ////   ns:
        ////     The namespace URI for the attribute.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteStartAttribute operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //protected internal virtual Task WriteStartAttributeAsync(string prefix, string localName, string ns);
        
        ////
        //// Summary:
        ////     When overridden in a derived class, writes the XML declaration with the version
        ////     "1.0" and the standalone attribute.
        ////
        //// Parameters:
        ////   standalone:
        ////     If true, it writes "standalone=yes"; if false, it writes "standalone=no".
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     This is not the first write method called after the constructor.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteStartDocument(bool standalone);
        ////
        //// Summary:
        ////     Asynchronously writes the XML declaration with the version "1.0".
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteStartDocument operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteStartDocumentAsync();
        ////
        //// Summary:
        ////     Asynchronously writes the XML declaration with the version "1.0" and the
        ////     standalone attribute.
        ////
        //// Parameters:
        ////   standalone:
        ////     If true, it writes "standalone=yes"; if false, it writes "standalone=no".
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteStartDocument operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteStartDocumentAsync(bool standalone);
        
        ////
        //// Summary:
        ////     When overridden in a derived class, writes the specified start tag and associates
        ////     it with the given namespace and prefix.
        ////
        //// Parameters:
        ////   prefix:
        ////     The namespace prefix of the element.
        ////
        ////   localName:
        ////     The local name of the element.
        ////
        ////   ns:
        ////     The namespace URI to associate with the element.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     The writer is closed.
        ////
        ////   System.Text.EncoderFallbackException:
        ////     There is a character in the buffer that is a valid XML character but is not
        ////     valid for the output encoding. For example, if the output encoding is ASCII,
        ////     you should only use characters from the range of 0 to 127 for element and
        ////     attribute names. The invalid character might be in the argument of this method
        ////     or in an argument of previous methods that were writing to the buffer. Such
        ////     characters are escaped by character entity references when possible (for
        ////     example, in text nodes or attribute values). However, the character entity
        ////     reference is not allowed in element and attribute names, comments, processing
        ////     instructions, or CDATA sections.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteStartElement(string prefix, string localName, string ns);
        ////
        //// Summary:
        ////     Asynchronously writes the specified start tag and associates it with the
        ////     given namespace and prefix.
        ////
        //// Parameters:
        ////   prefix:
        ////     The namespace prefix of the element.
        ////
        ////   localName:
        ////     The local name of the element.
        ////
        ////   ns:
        ////     The namespace URI to associate with the element.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteStartElement operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteStartElementAsync(string prefix, string localName, string ns);
        
        ////
        //// Summary:
        ////     Asynchronously writes the given text content.
        ////
        //// Parameters:
        ////   text:
        ////     The text to write.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteString operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteStringAsync(string text);
        ////
        //// Summary:
        ////     When overridden in a derived class, generates and writes the surrogate character
        ////     entity for the surrogate character pair.
        ////
        //// Parameters:
        ////   lowChar:
        ////     The low surrogate. This must be a value between 0xDC00 and 0xDFFF.
        ////
        ////   highChar:
        ////     The high surrogate. This must be a value between 0xD800 and 0xDBFF.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid surrogate character pair was passed.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);
        ////
        //// Summary:
        ////     Asynchronously generates and writes the surrogate character entity for the
        ////     surrogate character pair.
        ////
        //// Parameters:
        ////   lowChar:
        ////     The low surrogate. This must be a value between 0xDC00 and 0xDFFF.
        ////
        ////   highChar:
        ////     The high surrogate. This must be a value between 0xD800 and 0xDBFF.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteSurrogateCharEntity operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteSurrogateCharEntityAsync(char lowChar, char highChar);
        ////
        //// Summary:
        ////     Writes a System.Boolean value.
        ////
        //// Parameters:
        ////   value:
        ////     The System.Boolean value to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(bool value);
        ////
        //// Summary:
        ////     Writes a System.DateTime value.
        ////
        //// Parameters:
        ////   value:
        ////     The System.DateTime value to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(DateTime value);
        ////
        //// Summary:
        ////     Writes a System.DateTimeOffset value.
        ////
        //// Parameters:
        ////   value:
        ////     The System.DateTimeOffset value to write.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(DateTimeOffset value);
        ////
        //// Summary:
        ////     Writes a System.Decimal value.
        ////
        //// Parameters:
        ////   value:
        ////     The System.Decimal value to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(decimal value);
        ////
        //// Summary:
        ////     Writes a System.Double value.
        ////
        //// Parameters:
        ////   value:
        ////     The System.Double value to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(double value);
        ////
        //// Summary:
        ////     Writes a single-precision floating-point number.
        ////
        //// Parameters:
        ////   value:
        ////     The single-precision floating-point number to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(float value);
        ////
        //// Summary:
        ////     Writes a System.Int32 value.
        ////
        //// Parameters:
        ////   value:
        ////     The System.Int32 value to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(int value);
        ////
        //// Summary:
        ////     Writes a System.Int64 value.
        ////
        //// Parameters:
        ////   value:
        ////     The System.Int64 value to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(long value);
        ////
        //// Summary:
        ////     Writes the object value.
        ////
        //// Parameters:
        ////   value:
        ////     The object value to write.Note   With the release of the .NET Framework 3.5,
        ////     this method accepts System.DateTimeOffset as a parameter.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.ArgumentNullException:
        ////     The value is null.
        ////
        ////   System.InvalidOperationException:
        ////     The writer is closed or in error state.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(object value);
        ////
        //// Summary:
        ////     Writes a System.String value.
        ////
        //// Parameters:
        ////   value:
        ////     The System.String value to write.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     An invalid value was specified.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public virtual void WriteValue(string value);
        ////
        //// Summary:
        ////     When overridden in a derived class, writes out the given white space.
        ////
        //// Parameters:
        ////   ws:
        ////     The string of white space characters.
        ////
        //// Exceptions:
        ////   System.ArgumentException:
        ////     The string contains non-white space characters.
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        //public abstract void WriteWhitespace(string ws);
        ////
        //// Summary:
        ////     Asynchronously writes out the given white space.
        ////
        //// Parameters:
        ////   ws:
        ////     The string of white space characters.
        ////
        //// Returns:
        ////     The task that represents the asynchronous WriteWhitespace operation.
        ////
        //// Exceptions:
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter method was called before a previous asynchronous
        ////     operation finished. In this case, System.InvalidOperationException is thrown
        ////     with the message “An asynchronous operation is already in progress.”
        ////
        ////   System.InvalidOperationException:
        ////     An System.Xml.XmlWriter asynchronous method was called without setting the
        ////     System.Xml.XmlWriterSettings.Async flag to true. In this case, System.InvalidOperationException
        ////     is thrown with the message “Set XmlWriterSettings.Async to true if you want
        ////     to use Async Methods.”
        //public virtual Task WriteWhitespaceAsync(string ws);

    }

}