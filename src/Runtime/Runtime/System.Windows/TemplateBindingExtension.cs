
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
using System.Windows.Controls;

namespace System.Windows
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
                if (provider.TargetObject is IInternalControl source)
                {
                    string propertyName = DependencyPropertyName ?? Path;
                    Type type = DependencyPropertyOwnerType ?? source.GetType();
                    DependencyProperty dp = DependencyProperty.FromName(propertyName, type);

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
