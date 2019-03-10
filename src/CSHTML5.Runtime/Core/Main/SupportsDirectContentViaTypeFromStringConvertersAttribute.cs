
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


namespace System.Windows.Markup
{
    /// <summary>
    /// Indicates that the type supports direct content when used in XAML. A XAML processor
    /// uses this information when processing XAML child elements of XAML representations
    /// of the attributed type. The direct content is converted using the
    /// TypeFromStringConverter class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class SupportsDirectContentViaTypeFromStringConvertersAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the SupportsDirectContentViaTypeFromStringConvertersAttribute class.
        /// </summary>
        public SupportsDirectContentViaTypeFromStringConvertersAttribute() { }
    }
}