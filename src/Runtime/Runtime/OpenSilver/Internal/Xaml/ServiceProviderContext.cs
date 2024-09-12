
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
using System.Windows;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Xaml;
using OpenSilver.Internal.Xaml.Context;

namespace OpenSilver.Internal.Xaml
{
    internal sealed class ServiceProviderContext :
        IProvideValueTarget,
        IServiceProvider,
        IRootObjectProvider,
        IAmbientResourcesProvider,
        ITemplateOwnerProvider
    {
        private readonly XamlContext _xamlContext;

        public ServiceProviderContext(XamlContext context)
        {
            _xamlContext = context;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IAmbientResourcesProvider))
            {
                return this;
            }

            if (serviceType == typeof(ITemplateOwnerProvider))
            {
                return this;
            }

            if (serviceType == typeof(IProvideValueTarget))
            {
                return this;
            }

            if (serviceType == typeof(IRootObjectProvider))
            {
                return this;
            }

            return null;
        } 

        object IProvideValueTarget.TargetObject => _xamlContext.ParentInstance;

        object IProvideValueTarget.TargetProperty => throw new NotSupportedException();

        object IRootObjectProvider.RootObject => _xamlContext.RootInstance;

        IEnumerable<object> IAmbientResourcesProvider.GetAllAmbientValues()
            => _xamlContext.ServiceProvider_GetAllAmbientValues();

        DependencyObject ITemplateOwnerProvider.GetTemplateOwner() => _xamlContext.GetTemplateOwner();
    }
}
