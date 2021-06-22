
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

using CSHTML5.Internal;
using System;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    [ContentProperty(nameof(Path))]
    public class TemplateBindingExtension : MarkupExtension
    {
        public string Path { get; set; }

#if NETSTANDARD
        public override object ProvideValue(IServiceProvider serviceProvider)
#elif BRIDGE
        public override object ProvideValue(ServiceProvider serviceProvider)
#endif
        {
            var provider = (ServiceProvider)serviceProvider.GetService(typeof(ServiceProvider));
            if (provider != null)
            {
                var dp = INTERNAL_TypeToStringsToDependencyProperties.GetPropertyInTypeOrItsBaseTypes(
                    provider.TargetObject?.GetType(), Path);
                var source = provider.TargetObject as Control;

                if (dp != null && source != null)
                {
                    return new TemplateBindingExpression(source, dp);
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
