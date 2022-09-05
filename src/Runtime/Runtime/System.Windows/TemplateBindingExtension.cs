
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
using System.Windows.Markup;
using System.ComponentModel;
using CSHTML5.Internal;

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
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Path { get; set; }

        public string DependencyPropertyName { get; set; }

        public Type DependencyPropertyOwnerType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provider)
            {
                if (provider.TargetObject is Control source)
                {
                    string propertyName = DependencyPropertyName ?? Path;
                    Type type = DependencyPropertyOwnerType ?? source.GetType();
                    DependencyProperty dp = INTERNAL_TypeToStringsToDependencyProperties.GetPropertyInTypeOrItsBaseTypes(
                        type, propertyName);

                    if (dp != null)
                    {
                        return new TemplateBindingExpression(source, dp);
                    }
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
