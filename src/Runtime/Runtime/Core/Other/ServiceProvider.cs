

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

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace System
{

#if BRIDGE
    /// <summary>
    /// Defines a mechanism for retrieving a service object; that is, an object that
    /// provides custom support to other objects.
    /// </summary>
    public interface IServiceProvider
    {

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType. -or- null if there is no service object
        /// of type serviceType.
        /// </returns>
        object GetService(Type serviceType);
    } 
#endif



    /// <summary>
    /// Defines a mechanism for retrieving a service object; that is, an object that
    /// provides custom support to other objects.
    /// </summary>
#if NETSTANDARD
    public class ServiceProvider : IServiceProvider, IProvideValueTarget
#else // BRIDGE
    public class ServiceProvider : IServiceProvider, IProvideValueTarget
#endif
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
        public ServiceProvider(object element, DependencyProperty property) : this(element, property, new List<object>(0))
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
        public object GetService(Type serviceType)
        {
            return this;
        }


        List<Object> _parents;
        /// <summary>
        /// Gets or sets the List of UIElement that are parents of the element.
        /// </summary>
        public List<Object> Parents
        {
            get { return _parents; }
            private set { _parents = value; }
        }

        object _targetObject;
        public object TargetObject
        {
            get { return _targetObject; }
            private set { _targetObject = value; }
        }

        object _targetProperty;
        public object TargetProperty
        {
            get { return _targetProperty; }
            private set { _targetProperty = value; }
        }
    }
}
