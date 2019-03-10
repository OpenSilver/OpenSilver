
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

/// <summary>
/// Represents the SOAP style that determines how the WSDL metadata for the service
/// is formatted.
/// </summary>
public enum OperationFormatStyle
{

    /// <summary>
    /// Causes the WSDL representation to contain a single element that represents the document that is exchanged for the operation.
    /// </summary>
    Document = 0,

    /// <summary>
    /// Causes the WSDL representation of messages exchanged for an operation and
    /// contains parameters as if it were a remote procedure call.
    /// </summary>
    Rpc = 1,
}

#endif