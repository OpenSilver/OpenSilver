﻿
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

#if !OPENSILVER

#if WCF_STACK || BRIDGE || CSHTML5BLAZOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    /// <summary>
    /// Indicates that an interface or a class defines a service contract in a Windows
    /// Communication Foundation (WCF) application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed partial class ServiceContractAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the System.ServiceModel.ServiceContractAttribute
        /// class.
        /// </summary>
        public ServiceContractAttribute()
        {
            Namespace = "http://tempuri.org"; //default value
        }

        /// <summary>
        /// Gets or sets the type of callback contract when the contract is a duplex
        /// contract.
        /// </summary>
        public Type CallbackContract { get; set; }

        // Returns:
        //     The name used to locate the service element in an application configuration
        //     file. The default is the name of the service implementation class.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The value is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     The value is an empty string.
        /// <summary>
        /// The name used to locate the service element in an application configuration
        /// file. The default is the name of the service implementation class.
        /// </summary>
        public string ConfigurationName { get; set; } //todo: set default value
        
        // Returns:
        //     The default value is the name of the class or interface to which the System.ServiceModel.ServiceContractAttribute
        //     is applied.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The value is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     The value is an empty string.
        /// <summary>
        /// Gets or sets the name for the portType element in Web Services Description
        /// Language (WSDL).
        /// </summary>
        public string Name { get; set; } //todo: set default value

        /// <summary>
        /// Gets or sets the namespace of the portType element in Web Services Description
        /// Language (WSDL).
        /// </summary>
        public string Namespace { get; set; }
    }
}

#endif
#endif