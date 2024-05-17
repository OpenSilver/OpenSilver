
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
using System.Diagnostics;
using System.Windows;
using System.Xaml;

namespace OpenSilver.Internal.Xaml;

internal sealed class XamlTemplateContent : ITemplateContent
{
    private readonly IXamlObjectWriterFactory _objectWriterFactory;
    private readonly XamlSchemaContext _schemaContext;
    private readonly XamlObjectWriterSettings _objectWriterParentSettings;
    private readonly XamlNodeList _xamlNodeList;

    public XamlTemplateContent(XamlReader xamlReader, IXamlObjectWriterFactory factory, IServiceProvider context)
    {
        _objectWriterFactory = factory;
        _schemaContext = xamlReader.SchemaContext;
        _objectWriterParentSettings = factory.GetParentSettings();
        _xamlNodeList = new XamlNodeList(_schemaContext);

        Initialize(xamlReader);
    }

    private void Initialize(XamlReader xamlReader)
    {
        Debug.Assert(xamlReader.NodeType == XamlNodeType.None);

        using (var xamlWriter = new XamlNodeListWriter(_xamlNodeList))
        {
            while (xamlReader.Read())
            {
                xamlWriter.WriteNode(xamlReader);
            }
        }
    }

    IFrameworkElement ITemplateContent.LoadContent(IFrameworkElement owner)
    {
        XamlObjectWriterSettings settings = CreateObjectWriterSettings(_objectWriterParentSettings);
        settings.ExternalNameScope = new NameScope();
        settings.RegisterNamesOnExternalNamescope = true;

        settings.BeforePropertiesHandler =
            delegate (object sender, XamlObjectEventArgs args)
            {
                if (args.Instance is FrameworkElement fe)
                {
                    fe.TemplatedParent = (DependencyObject)owner;
                }
            };

        if (LoadXaml(settings) is IFrameworkElement rootElement)
        {
            if (owner == null)
            {
                if (NameScope.GetNameScope(rootElement) == null)
                {
                    NameScope.SetNameScope(rootElement, settings.ExternalNameScope);
                }
            }
            else
            {
                FrameworkTemplate.SetTemplateNameScope(owner, settings.ExternalNameScope);
            }

            return rootElement;
        }

        return null;
    }

    private object LoadXaml(XamlObjectWriterSettings settings)
    {
        var xamlReader = new XamlNodeListReader(_xamlNodeList);
        var xamlWriter = _objectWriterFactory.GetXamlObjectWriter(settings);

        if (xamlReader.NodeType == XamlNodeType.None)
        {
            xamlReader.Read();
        }

        var xamlLineInfo = xamlReader as IXamlLineInfo;
        var xamlLineConsumer = xamlWriter as IXamlLineInfoConsumer;
        var shouldSetLineInfo = xamlLineInfo != null && xamlLineConsumer != null && xamlLineConsumer.ShouldProvideLineInfo && xamlLineInfo.HasLineInfo;

        while (!xamlReader.IsEof)
        {
            if (shouldSetLineInfo)
            {
                xamlLineConsumer.SetLineInfo(xamlLineInfo.LineNumber, xamlLineInfo.LinePosition);
            }
            xamlWriter.WriteNode(xamlReader);
            xamlReader.Read();
        }

        return xamlWriter.Result;
    }

    private static XamlObjectWriterSettings CreateObjectWriterSettings()
    {
        return new XamlObjectWriterSettings
        {
            IgnoreCanConvert = true,
            PreferUnconvertedDictionaryKeys = true
        };
    }

    private static XamlObjectWriterSettings CreateObjectWriterSettings(XamlObjectWriterSettings parentSettings)
    {
        XamlObjectWriterSettings owSettings = CreateObjectWriterSettings();
        if (parentSettings != null)
        {
            owSettings.SkipDuplicatePropertyCheck = parentSettings.SkipDuplicatePropertyCheck;
            owSettings.SkipProvideValueOnRoot = parentSettings.SkipProvideValueOnRoot;
        }
        return owSettings;
    }
}
