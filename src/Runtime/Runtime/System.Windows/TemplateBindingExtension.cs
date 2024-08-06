
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

using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Controls;
using OpenSilver.Internal.Xaml;

namespace System.Windows
{
    [ContentProperty(nameof(Path))]
    public class TemplateBindingExtension : MarkupExtension
    {
        public TemplateBindingExtension() { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public TemplateBindingExtension(string path)
        {
            Path = path;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Path { get; set; }

        public string DependencyPropertyName { get; set; }

        public Type DependencyPropertyOwnerType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(ITemplateOwnerProvider)) is ITemplateOwnerProvider templateOwnerProvider)
            {
                return ProvideValueImpl(templateOwnerProvider, serviceProvider);
            }

            return LegacyProvideValue(serviceProvider);
        }

        private object ProvideValueImpl(ITemplateOwnerProvider templateOwnerProvider, IServiceProvider serviceProvider)
        {
            if (templateOwnerProvider.GetTemplateOwner() is IInternalControl source)
            {
                DependencyProperty dp = null;

                if (DependencyPropertyName is not null)
                {
                    Type type = DependencyPropertyOwnerType ?? source.GetType();
                    dp = DependencyProperty.FromName(DependencyPropertyName, type);
                }
                else if (Path is not null)
                {
                    int index = Path.IndexOf('.');
                    if (index > -1)
                    {
                        if (serviceProvider.GetService(typeof(IXamlTypeResolver)) is IXamlTypeResolver typeResolver)
                        {
                            string typeName = Path.Substring(0, index);
                            if (typeResolver.Resolve(typeName) is Type type)
                            {
                                string propertyName = Path.Substring(index + 1);
                                dp = DependencyProperty.FromName(propertyName, type);
                            }
                        }
                    }
                    else
                    {
                        Type type = source.GetType();
                        dp = DependencyProperty.FromName(Path, type);
                    }
                }

                if (dp is not null)
                {
                    return new TemplateBindingExpression(source, dp);
                }
            }

            return DependencyProperty.UnsetValue;
        }

        private object LegacyProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)
            {
                if (provideValueTarget.TargetObject is IInternalControl source)
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
