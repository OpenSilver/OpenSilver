

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace System
{
    /// <summary>
    /// Defines a mechanism for retrieving a service object; that is, an object that
    /// provides custom support to other objects.
    /// </summary>
    /// <exclude/>
    /// 
#if !BRIDGE
    public class ServiceProvider : IServiceProvider, IProvideValueTarget
#else
    public class ServiceProvider : IProvideValueTarget
#endif
    {
        /// <summary>
        /// Constructor for the  ServiceProvider class
        /// </summary>
        /// <param name="element"></param>
        /// <param name="property"></param>
        /// <param name="parents"></param>
        public ServiceProvider(object element, DependencyProperty property, List<object> parents)
        {
            TargetObject = element;
            TargetProperty = property;
            Parents = parents;
        }

        public ServiceProvider(object element, DependencyProperty property) : this(element, property, new List<object>())
        {

        }
        
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
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
