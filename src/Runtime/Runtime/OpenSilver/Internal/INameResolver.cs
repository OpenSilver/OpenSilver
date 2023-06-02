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

#if MIGRATION
using System.Windows;
using System.Windows.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

namespace OpenSilver.Internal
{
    internal interface INameResolver
    {
        object Resolve(string name);
    }

    internal sealed class TemplateNameResolver : INameResolver
    {
        private readonly WeakReference<IInternalFrameworkElement> _templateRootRef;

        public TemplateNameResolver(IInternalFrameworkElement templateRoot)
        {
            if (templateRoot is null)
            {
                throw new ArgumentNullException(nameof(templateRoot));
            }

            _templateRootRef = new WeakReference<IInternalFrameworkElement>(templateRoot);
        }

        object INameResolver.Resolve(string name) => GetNameScope()?.FindName(name);

        private INameScope GetNameScope()
        {
            if (_templateRootRef.TryGetTarget(out IInternalFrameworkElement templateRoot))
            {
                if (templateRoot.TemplatedParent != null)
                {
                    return FrameworkTemplate.GetTemplateNameScope(templateRoot.TemplatedParent as IFrameworkElement);
                }
                else
                {
                    return NameScope.GetNameScope(templateRoot);
                }
            }

            return null;
        }
    }

    internal sealed class XamlNameResolver : INameResolver
    {
        private readonly WeakReference<IFrameworkElement> _namescopeOwnerRef;

        public XamlNameResolver(IFrameworkElement namescopeOwner)
        {
            if (namescopeOwner is null)
            {
                throw new ArgumentNullException(nameof(namescopeOwner));
            }

            _namescopeOwnerRef = new WeakReference<IFrameworkElement>(namescopeOwner);
        }

        object INameResolver.Resolve(string name) => GetNameScope()?.FindName(name);

        private INameScope GetNameScope()
        {
            if (_namescopeOwnerRef.TryGetTarget(out IFrameworkElement namescopeOwner))
            {
                return NameScope.GetNameScope(namescopeOwner);
            }

            return null;
        }
    }
}
