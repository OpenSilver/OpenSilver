
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
using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public class ChannelFactory<T> : IDisposable
    {
        public Binding Binding { get; set; }
        public EndpointAddress EndpointAddress { get; set; }

        public ChannelFactory(Binding binding, EndpointAddress endpointAddress)
        {
            this.Binding = binding;
            this.EndpointAddress = endpointAddress;
        }

        public void Dispose()
        {
        }

        public T CreateChannel()
        {
            return default(T);
        }
    }
}
#endif

#endif