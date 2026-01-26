
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
using System.Windows.Markup;

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
            ArgumentNullException.ThrowIfNull(templateRoot);

            _templateRootRef = new WeakReference<IInternalFrameworkElement>(templateRoot);
        }

        object INameResolver.Resolve(string name) => GetNameScope()?.FindName(name);

        private INameScope GetNameScope()
        {
            if (_templateRootRef.TryGetTarget(out IInternalFrameworkElement templateRoot))
            {
                DependencyObject templatedParent = templateRoot.TemplatedParent;
                if (templatedParent is IFrameworkElement)
                {
                    return FrameworkTemplate.GetTemplateNameScope(templatedParent);
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
        private readonly WeakReference<IInternalFrameworkElement> _namescopeOwnerRef;

        public XamlNameResolver(IInternalFrameworkElement namescopeOwner)
        {
            ArgumentNullException.ThrowIfNull(namescopeOwner);

            _namescopeOwnerRef = new WeakReference<IInternalFrameworkElement>(namescopeOwner);
        }

        object INameResolver.Resolve(string name) => GetNameScope()?.FindName(name);

        private INameScope GetNameScope()
        {
            if (_namescopeOwnerRef.TryGetTarget(out IInternalFrameworkElement namescopeOwner))
            {
                return NameScope.GetNameScope(namescopeOwner);
            }

            return null;
        }
    }
}
