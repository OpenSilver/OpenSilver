
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

using System.Collections.Generic;
using System.Windows.Markup;
using System.Windows;

namespace System
{
    /// <summary>
    /// Defines a mechanism for retrieving a service object; that is, an object that
    /// provides custom support to other objects.
    /// </summary>
    public class ServiceProvider : IServiceProvider, IProvideValueTarget
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="ServiceProvider"/> class.
        /// </summary>
        public ServiceProvider(object element, DependencyProperty property, List<object> parents)
        {
            TargetObject = element;
            TargetProperty = property;
            Parents = parents;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="ServiceProvider"/> class.
        /// </summary>
        public ServiceProvider(object element, DependencyProperty property)
            : this(element, property, new List<object>(0))
        {
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">
        /// An object that specifies the type of service object to get.
        /// </param>
        /// <returns>
        /// A service object of type serviceType.-or- null if there is no service object
        /// of type serviceType.
        /// </returns>
        public object GetService(Type serviceType) => this;

        /// <summary>
        /// Gets or sets the List of UIElement that are parents of the element.
        /// </summary>
        public List<object> Parents { get; private set; }

        public object TargetObject { get; private set; }

        public object TargetProperty { get; private set; }
    }
}
