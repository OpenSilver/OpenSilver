
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

using System.Xaml;

namespace System.Windows.Markup
{
    /// <summary>
    /// Provides a base class for XAML markup extension implementations that can be supported
    /// by Silverlight XAML processors.
    /// </summary>
    public abstract class MarkupExtension : IMarkupExtension<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkupExtension"/>.
        /// </summary>
        protected MarkupExtension()
        {
        }

        /// <summary>
        /// When implemented in a derived class, returns an object that is provided as the
        /// value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">
        /// A service provider helper that can provide services for the markup extension.
        /// </param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
#if NETSTANDARD
        public abstract object ProvideValue(IServiceProvider serviceProvider);
#else // BRIDGE
        public abstract object ProvideValue(IServiceProvider serviceProvider);
#endif
    }
}

