
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


#if WCF_STACK

#if UNIMPLEMENTED_MEMBERS

using System;
using System.ServiceModel;

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Represents the unit of communication between endpoints in a distributed environment.
    /// </summary>
    public abstract class Message : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        /*
        // Summary:
        //     Initializes a new instance of the System.ServiceModel.Channels.Message class.
        protected Message();

        // Summary:
        //     When overridden in a derived class, gets the headers of the message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.MessageHeaders object that represents the
        //     headers of the message.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The message has been disposed of.
        public abstract MessageHeaders Headers { get; }
        //
        // Summary:
        //     Returns a value that indicates whether the System.ServiceModel.Channels.Message
        //     is disposed.
        //
        // Returns:
        //     true if the message is disposed; otherwise, false.
        protected bool IsDisposed { get; }
        //
        // Summary:
        //     Returns a value that indicates whether the System.ServiceModel.Channels.Message
        //     is empty.
        //
        // Returns:
        //     true if the message is empty; otherwise, false.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The message has been disposed of.
        public virtual bool IsEmpty { get; }
        //
        // Summary:
        //     Gets a value that indicates whether this message generates any SOAP faults.
        //
        // Returns:
        //     true if this message generates any SOAP faults; otherwise, false.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The message has been disposed of.
        public virtual bool IsFault { get; }
        //
        // Summary:
        //     When overridden in a derived class, gets a set of processing-level annotations
        //     to the message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.MessageProperties that contains a set of processing-level
        //     annotations to the message.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The message has been disposed of.
        public abstract MessageProperties Properties { get; }
        //
        // Summary:
        //     Gets the current state of this System.ServiceModel.Channels.Message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.MessageState that contains the current state
        //     of this System.ServiceModel.Channels.Message.
        public MessageState State { get; }
        //
        // Summary:
        //     When overridden in a derived class, gets the SOAP version of the message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.MessageVersion object that represents the
        //     SOAP version.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The message has been disposed of.
        public abstract MessageVersion Version { get; }

        // Summary:
        //     Starts the asynchronous writing of the contents of the message body.
        //
        // Parameters:
        //   writer:
        //     The writer used to serialize the message body.
        //
        //   callback:
        //     The delegate method that receives the notification when the operation completed.
        //
        //   state:
        //     The user-defined object that represents the state of the operation.
        //
        // Returns:
        //     The System.IAsyncResult object that represents the result of the asynchronous
        //     operation.
        public IAsyncResult BeginWriteBodyContents(System.Xml.XmlDictionaryWriter writer, AsyncCallback callback, object state);
        //
        // Summary:
        //     Starts the asynchronous writing of the entire message.
        //
        // Parameters:
        //   writer:
        //     The writer used to serialize the entire message.
        //
        //   callback:
        //     The delegate method that receives the notification when the operation completed.
        //
        //   state:
        //     The user-defined object that represents the state of the operation.
        //
        // Returns:
        //     The System.IAsyncResult object that represents the result of the asynchronous
        //     operation.
        public IAsyncResult BeginWriteMessage(System.Xml.XmlDictionaryWriter writer, AsyncCallback callback, object state);
        //
        // Summary:
        //     Closes the System.ServiceModel.Channels.Message and releases any resources.
        public void Close();
        //
        // Summary:
        //     Stores an entire System.ServiceModel.Channels.Message into a memory buffer
        //     for future access.
        //
        // Parameters:
        //   maxBufferSize:
        //     The maximum size of the buffer to be created.
        //
        // Returns:
        //     A newly created System.ServiceModel.Channels.MessageBuffer object.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     maxBufferSize  is smaller than zero.
        //
        //   System.ObjectDisposedException:
        //     The message is closed.
        //
        //   System.InvalidOperationException:
        //     The message has been copied, read or written.
        public MessageBuffer CreateBufferedCopy(int maxBufferSize);
        //
        // Summary:
        //     Creates a message that contains a version and an action.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     version or action is null.
        public static Message CreateMessage(MessageVersion version, string action);
        //
        // Summary:
        //     Creates a message that contains a SOAP fault, a version and an action.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   fault:
        //     A System.ServiceModel.Channels.MessageFault object that represents a SOAP
        //     fault.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     Version, fault or action is null.
        public static Message CreateMessage(MessageVersion version, MessageFault fault, string action);
        //
        // Summary:
        //     Creates a message with a body that consists of an array of bytes.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        //   body:
        //     A System.ServiceModel.Channels.BodyWriter of type byte.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     Version, action or body is null.
        public static Message CreateMessage(MessageVersion version, string action, BodyWriter body);
        //
        // Summary:
        //     Creates a message with the specified version, action and body.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        //   body:
        //     The body of the message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     version, action or body is null.
        public static Message CreateMessage(MessageVersion version, string action, object body);
        //
        // Summary:
        //     Creates a message with the specified version, action and body.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        //   body:
        //     The body of the message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     version, action or body is null.
        public static Message CreateMessage(MessageVersion version, string action, System.Xml.XmlDictionaryReader body);
        //
        // Summary:
        //     Creates a message using the specified reader, action and version.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        //   body:
        //     The System.Xml.XmlReader object to be used for reading the SOAP message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     version, action or body is null.
        public static Message CreateMessage(MessageVersion version, string action, XmlReader body);
        //
        // Summary:
        //     Creates a message using the specified reader, action and version.
        //
        // Parameters:
        //   envelopeReader:
        //     The System.Xml.XmlDictionaryReader object to be used for reading the SOAP
        //     message.
        //
        //   maxSizeOfHeaders:
        //     The maximum size in bytes of a header.
        //
        //   version:
        //     A valid System.ServiceModel.Channels.MessageVersion value that specifies
        //     the SOAP version to use for the message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     envelopeReader or version is null.
        public static Message CreateMessage(System.Xml.XmlDictionaryReader envelopeReader, int maxSizeOfHeaders, MessageVersion version);
        //
        // Summary:
        //     Creates a message using the specified reader, action and version.
        //
        // Parameters:
        //   envelopeReader:
        //     The System.Xml.XmlReader object to be used for reading the SOAP message.
        //
        //   maxSizeOfHeaders:
        //     The maximum size in bytes of a header.
        //
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     envelopeReader or version is null.
        public static Message CreateMessage(XmlReader envelopeReader, int maxSizeOfHeaders, MessageVersion version);
        //
        // Summary:
        //     Creates a message that contains a SOAP fault, the reason for the fault, a
        //     version and an action.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   faultCode:
        //     A System.ServiceModel.Channels.MessageFault object that represents a SOAP
        //     fault.
        //
        //   reason:
        //     The reason of the SOAP fault.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     version, fault, action or faultCode is null.
        public static Message CreateMessage(MessageVersion version, FaultCode faultCode, string reason, string action);
        //
        // Summary:
        //     Creates a message using the specified version, action, message body and serializer.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        //   body:
        //     The body of the message.
        //
        //   serializer:
        //     A System.Runtime.Serialization.XmlObjectSerializer object used to serialize
        //     the message.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     version, fault or action is null.
        public static Message CreateMessage(MessageVersion version, string action, object body, System.Runtime.Serialization.XmlObjectSerializer serializer);
        //
        // Summary:
        //     Creates a message that contains a SOAP fault, a reason and the detail for
        //     the fault, a version and an action.
        //
        // Parameters:
        //   version:
        //     A System.ServiceModel.Channels.MessageVersion object that specifies the SOAP
        //     version to use for the message.
        //
        //   faultCode:
        //     A System.ServiceModel.Channels.MessageFault object that represents a SOAP
        //     fault.
        //
        //   reason:
        //     The reason of the SOAP fault.
        //
        //   detail:
        //     The details of the SOAP fault.
        //
        //   action:
        //     A description of how the message should be processed.
        //
        // Returns:
        //     A System.ServiceModel.Channels.Message object for the message created.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     version, fault, action, detail or faultCode is null.
        public static Message CreateMessage(MessageVersion version, FaultCode faultCode, string reason, object detail, string action);
        //
        // Summary:
        //     Ends the asynchronous writing of the contents of the message body.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult object that represents the result of the asynchronous
        //     operation.
        public void EndWriteBodyContents(IAsyncResult result);
        //
        // Summary:
        //     Ends the asynchronous writing of the entire message.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult object that represents the result of the asynchronous
        //     operation.
        public void EndWriteMessage(IAsyncResult result);
        //
        // Summary:
        //     Retrieves the body of this System.ServiceModel.Channels.Message instance.
        //
        // Type parameters:
        //   T:
        //     The body of the message.
        //
        // Returns:
        //     An object of type T that contains the body of this message.
        public T GetBody<T>();
        //
        // Summary:
        //     Retrieves the body of this System.ServiceModel.Channels.Message using the
        //     specified serializer.
        //
        // Parameters:
        //   serializer:
        //     A System.Runtime.Serialization.XmlObjectSerializer object used to read the
        //     body of the message.
        //
        // Type parameters:
        //   T:
        //     The body of the message.
        //
        // Returns:
        //     An object of type T that contains the body of this message.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     serializer is null.
        public T GetBody<T>(System.Runtime.Serialization.XmlObjectSerializer serializer);
        //
        // Summary:
        //     Retrieves the attributes of the message body.
        //
        // Parameters:
        //   localName:
        //     The local name of the XML node.The name of the element that corresponds to
        //     this member. This string must be a valid XML element name.
        //
        //   ns:
        //     The namespace to which this XML element belongs.The namespace URI of the
        //     element that corresponds to this member. The system does not validate any
        //     URIs other than transport addresses.
        //
        // Returns:
        //     The attributes of the message body.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     localName or ns is null.
        //
        //   System.ObjectDisposedException:
        //     The message is closed.
        //
        //   System.InvalidOperationException:
        //     The message has been copied, read or written.
        public string GetBodyAttribute(string localName, string ns);
        //
        // Summary:
        //     Gets the XML dictionary reader that accesses the body content of this message.
        //
        // Returns:
        //     A System.Xml.XmlDictionaryReader object that accesses the body content of
        //     this message.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The message is closed.
        //
        //   System.InvalidOperationException:
        //     The message is empty, or has been copied, read or written.
        public System.Xml.XmlDictionaryReader GetReaderAtBodyContents();
        //
        // Summary:
        //     Raises an event when the message starts writing the contents of the message
        //     body.
        //
        // Parameters:
        //   writer:
        //     The writer used to serialize the contents of the message body.
        //
        //   callback:
        //     The delegate method that receives the notification when the operation completed.
        //
        //   state:
        //     The user-defined object that represents the state of the operation.
        //
        // Returns:
        //     The System.IAsyncResult object that represents the result of the asynchronous
        //     operation.
        protected virtual IAsyncResult OnBeginWriteBodyContents(System.Xml.XmlDictionaryWriter writer, AsyncCallback callback, object state);
        //
        // Summary:
        //     Raises an event the writing of entire messages starts.
        //
        // Parameters:
        //   writer:
        //     The writer used to serialize the entire message.
        //
        //   callback:
        //     The delegate method that receives the notification when the operation completed.
        //
        //   state:
        //     The user-defined object that represents the state of the operation.
        //
        // Returns:
        //     The System.IAsyncResult object that represents the result of the asynchronous
        //     operation.
        protected virtual IAsyncResult OnBeginWriteMessage(System.Xml.XmlDictionaryWriter writer, AsyncCallback callback, object state);
        //
        // Summary:
        //     Called when the message body is converted to a string.
        //
        // Parameters:
        //   writer:
        //     A System.Xml.XmlDictionaryWriter that is used to convert the message body
        //     to a string.
        protected virtual void OnBodyToString(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Called when the message is closing.
        protected virtual void OnClose();
        //
        // Summary:
        //     Called when a message buffer is created to store this message.
        //
        // Parameters:
        //   maxBufferSize:
        //     The maximum size of the buffer to be created.
        //
        // Returns:
        //     A System.ServiceModel.Channels.MessageBuffer object for the newly created
        //     message copy.
        protected virtual MessageBuffer OnCreateBufferedCopy(int maxBufferSize);
        //
        // Summary:
        //     Raises an event when writing of the contents of the message body ends.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult object that represents the result of the asynchronous
        //     operation.
        protected virtual void OnEndWriteBodyContents(IAsyncResult result);
        //
        // Summary:
        //     Raises an event when the writing of the entire message ends.
        //
        // Parameters:
        //   result:
        //     The System.IAsyncResult object that represents the result of the asynchronous
        //     operation.
        protected virtual void OnEndWriteMessage(IAsyncResult result);
        //
        // Summary:
        //     Called when the body of the message is retrieved.
        //
        // Parameters:
        //   reader:
        //     A System.Xml.XmlDictionaryReader) object used to read the body of the message.
        //
        // Type parameters:
        //   T:
        //     The type of the message body.
        //
        // Returns:
        //     A System.ServiceModel.Channels.MessageBuffer that represents the body of
        //     the message.
        protected virtual T OnGetBody<T>(System.Xml.XmlDictionaryReader reader);
        //
        // Summary:
        //     Called when the attributes of the message body is retrieved.
        //
        // Parameters:
        //   localName:
        //     The local name of the XML node.The name of the element that corresponds to
        //     this member. This string must be a valid XML element name.
        //
        //   ns:
        //     The namespace to which this XML element belongs.The namespace URI of the
        //     element that corresponds to this member. The system does not validate any
        //     URIs other than transport addresses.
        //
        // Returns:
        //     The attributes of the message body.
        protected virtual string OnGetBodyAttribute(string localName, string ns);
        //
        // Summary:
        //     Called when an XML dictionary reader that accesses the body content of this
        //     message is retrieved.
        //
        // Returns:
        //     A System.Xml.XmlDictionaryReader object that accesses the body content of
        //     this message.
        protected virtual System.Xml.XmlDictionaryReader OnGetReaderAtBodyContents();
        //
        // Summary:
        //     Called when the message body is written to an XML file.
        //
        // Parameters:
        //   writer:
        //     A System.Xml.XmlDictionaryWriter that is used to write this message body
        //     to an XML file.
        protected abstract void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Called when the entire message is written to an XML file.
        //
        // Parameters:
        //   writer:
        //     A System.Xml.XmlDictionaryWriter that is used to write this message to an
        //     XML file.
        protected virtual void OnWriteMessage(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Called when the start body is written to an XML file.
        //
        // Parameters:
        //   writer:
        //     A System.Xml.XmlDictionaryWriter that is used to write the start body to
        //     an XML file.
        protected virtual void OnWriteStartBody(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Called when the start envelope is written to an XML file.
        //
        // Parameters:
        //   writer:
        //     A System.Xml.XmlDictionaryWriter that is used to write the start envelope
        //     to an XML file.
        protected virtual void OnWriteStartEnvelope(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Called when the start header is written to an XML file.
        //
        // Parameters:
        //   writer:
        //     A System.Xml.XmlDictionaryWriter that is used to write the start header to
        //     an XML file.
        protected virtual void OnWriteStartHeaders(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Returns a string that represents the current System.ServiceModel.Channels.Message
        //     instance.
        //
        // Returns:
        //     The string representation of the current System.ServiceModel.Channels.Message
        //     instance.
        public override string ToString();
        //
        // Summary:
        //     Writes the body element using the specified System.Xml.XmlDictionaryWriter.
        //
        // Parameters:
        //   writer:
        //     An System.Xml.XmlDictionaryWriter object to be used to write the body element.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     writer is null.
        public void WriteBody(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Serializes the message body using the specified System.Xml.XmlWriter.
        //
        // Parameters:
        //   writer:
        //     The System.Xml.XmlWriter object to be used to write the body of the message.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     writer is null.
        //
        //   System.ObjectDisposedException:
        //     The message is disposed.
        public void WriteBody(XmlWriter writer);
        //
        // Summary:
        //     Serializes the body content using the specified System.Xml.XmlDictionaryWriter.
        //
        // Parameters:
        //   writer:
        //     An System.Xml.XmlDictionaryWriter object to be used to write the body element.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     writer is null.
        //
        //   System.ObjectDisposedException:
        //     The message is closed.
        //
        //   System.InvalidOperationException:
        //     The message has been copied, read or written.
        public void WriteBodyContents(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Serializes the entire message using the specified System.Xml.XmlDictionaryWriter.
        //
        // Parameters:
        //   writer:
        //     An System.Xml.XmlDictionaryWriter object to be used to write the message.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     writer is null.
        //
        //   System.ObjectDisposedException:
        //     The message is closed.
        //
        //   System.InvalidOperationException:
        //     The message has been copied, read or written.
        public void WriteMessage(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Serializes the entire message using the specified System.Xml.XmlWriter.
        //
        // Parameters:
        //   writer:
        //     The System.Xml.XmlWriter object to be used to write the entire message.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     writer is null.
        //
        //   System.ObjectDisposedException:
        //     The message is closed.
        //
        //   System.InvalidOperationException:
        //     The message has been copied, read or written.
        public void WriteMessage(XmlWriter writer);
        //
        // Summary:
        //     Serializes the start body of the message using the specified System.Xml.XmlDictionaryWriter.
        //
        // Parameters:
        //   writer:
        //     An System.Xml.XmlDictionaryWriter object to be used to write the start body.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     writer is null.
        public void WriteStartBody(System.Xml.XmlDictionaryWriter writer);
        //
        // Summary:
        //     Serializes the start body of the message using the specified System.Xml.XmlDictionaryWriter.
        //
        // Parameters:
        //   writer:
        //     The System.Xml.XmlDictionaryWriter object to be used to write the start body
        //     of the message.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     writer is null.
        public void WriteStartBody(XmlWriter writer);
        //
        // Summary:
        //     Serializes the start envelope using the specified System.Xml.XmlDictionaryWriter.
        //
        // Parameters:
        //   writer:
        //     An System.Xml.XmlDictionaryWriter object to be used to write the start envelope.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     writer is null.
        public void WriteStartEnvelope(System.Xml.XmlDictionaryWriter writer);
         */
    }
}

#endif

#endif