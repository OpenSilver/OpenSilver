
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