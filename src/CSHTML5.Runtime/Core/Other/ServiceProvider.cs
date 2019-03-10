
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
        public ServiceProvider(object element, DependencyProperty property, List<Object> parents = null)
        {
            TargetObject = element;
            TargetProperty = property;
            Parents = parents;
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
