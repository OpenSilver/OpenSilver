
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


#if WCF_STACK || BRIDGE

namespace System.ServiceModel
{
    /// <summary>
    /// Determines the format of the message.
    /// </summary>
    public enum OperationFormatUse
    {
        /// <summary>
        /// Implies that the message is a literal instance of the schema in the WSDL.
        /// </summary>
        Literal = 0,

        /// <summary>
        /// Implies that the schemas in the WSDL are abstract specifications that are
        /// encoded according to the rules found in SOAP 1.1 section 5.
        /// </summary>
        Encoded = 1,
    }
}

#endif