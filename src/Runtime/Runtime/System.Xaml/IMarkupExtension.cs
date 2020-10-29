﻿

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


using System;

namespace System.Xaml
{
    /// <summary>
    /// Provides an interface basis for XAML markup extension implementations that can be supported by Silverlight XAML processors.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object that is returned by the <see cref="M:System.Xaml.IMarkupExtension.ProvideValue(System.IServiceProvider)" /> implementation.
    /// This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived.
    /// </typeparam>
    public interface IMarkupExtension<out T> where T : class
    {
        /// <summary>
        /// Returns an object that is provided as the value of the target property for the markup extension.
        /// </summary>
        /// <returns>
        /// The value to set on the property where the extension is applied.
        /// </returns>
        /// <param name="serviceProvider">
        /// A service provider helper that can provide services for the markup extension.
        /// </param>
        T ProvideValue(IServiceProvider serviceProvider);
    }
}
