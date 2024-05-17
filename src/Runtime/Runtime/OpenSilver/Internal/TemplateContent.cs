
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
using OpenSilver.Internal.Xaml.Context;

namespace OpenSilver.Internal;

internal sealed class TemplateContent : ITemplateContent
{
    private readonly XamlContext _xamlContext;
    private readonly Func<IFrameworkElement, XamlContext, IFrameworkElement> _factory;

    internal TemplateContent(XamlContext xamlContext, Func<IFrameworkElement, XamlContext, IFrameworkElement> factory)
    {
        if (xamlContext is null)
        {
            throw new ArgumentNullException(nameof(xamlContext));
        }

        _xamlContext = new XamlContext(xamlContext);
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public IFrameworkElement LoadContent(IFrameworkElement owner)
    {
        var context = new XamlContext(_xamlContext)
        {
            ExternalNameScope = new NameScope(),
        };

        IFrameworkElement rootElement = _factory(owner, context);

        if (owner is null)
        {
            if (NameScope.GetNameScope(rootElement) is null)
            {
                NameScope.SetNameScope(rootElement, context.ExternalNameScope);
            }
        }
        else
        {
            FrameworkTemplate.SetTemplateNameScope(owner, context.ExternalNameScope);
        }

        return rootElement;
    }
}
