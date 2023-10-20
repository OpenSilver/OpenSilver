
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
    /// In XAML markup, specifies a null value for a property.
    /// </summary>
    public class NullExtension : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NullExtension"/>.
        /// </summary>
        public NullExtension() { }

        public override object ProvideValue(IServiceProvider serviceProvider) => null;
    }
}
