
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

namespace System.ServiceModel
{
    public class OperationContext
    {
        IContextChannel _channel;

        private MessageHeaders _outgoingMessageHeaders;
        public MessageHeaders OutgoingMessageHeaders
        {
            get { return _outgoingMessageHeaders ?? (_outgoingMessageHeaders = new MessageHeaders()); }
        }

        private MessageHeaders _incomingMessageHeaders;
        public MessageHeaders IncomingMessageHeaders
        {
            get { return _incomingMessageHeaders ?? (_incomingMessageHeaders = new MessageHeaders()); }
        }

        public static OperationContext Current { get; set; }

        public OperationContext(IContextChannel channel)
        {
            _channel = channel;
        }
    }
}

#endif

#endif