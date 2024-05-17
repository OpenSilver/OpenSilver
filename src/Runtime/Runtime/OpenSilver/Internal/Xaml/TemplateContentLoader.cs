
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
using System.Xaml;

namespace OpenSilver.Internal.Xaml;

internal sealed class TemplateContentLoader : XamlDeferringLoader
{
    public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
    {
        if (xamlReader is null)
        {
            throw new ArgumentNullException(nameof(xamlReader));
        }

        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        IXamlObjectWriterFactory factory = RequireService<IXamlObjectWriterFactory>(serviceProvider);

        return new XamlTemplateContent(xamlReader, factory, serviceProvider);
    }

    public override XamlReader Save(object value, IServiceProvider serviceProvider)
    {
        throw new NotSupportedException(
            $"Deferring loader '{typeof(TemplateContentLoader).Name}' does not support the Save operation.");
    }

    private static T RequireService<T>(IServiceProvider provider) where T : class
    {
        if (provider.GetService(typeof(T)) is not T result)
        {
            throw new InvalidOperationException(
                $"Deferring loader '{typeof(TemplateContentLoader).Name}' requires that '{typeof(T).Name}' be implemented in the IServiceProvider.");
        }
        return result;
    }
}
