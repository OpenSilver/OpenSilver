

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/


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