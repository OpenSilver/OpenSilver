
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Xaml;

namespace OpenSilver.Internal.Xaml;

internal sealed class XamlTemplateContent : TemplateContent
{
    private readonly IXamlObjectWriterFactory _objectWriterFactory;
    private readonly XamlSchemaContext _schemaContext;
    private readonly XamlObjectWriterSettings _objectWriterParentSettings;
    private readonly XamlNodeList _xamlNodeList;
    private Dictionary<string, XamlType> _namedTypes;

    public XamlTemplateContent(XamlReader xamlReader, IXamlObjectWriterFactory factory, IServiceProvider context)
    {
        _objectWriterFactory = factory;
        _schemaContext = xamlReader.SchemaContext;
        _objectWriterParentSettings = factory.GetParentSettings();
        _xamlNodeList = new XamlNodeList(_schemaContext);

        Initialize(xamlReader);
    }

    /// <inheritdoc />
    protected override void OnSealed()
    {
        base.OnSealed();
        _namedTypes = null;
    }

    /// <inheritdoc />
    public override IFrameworkElement LoadContent<T>(T owner)
    {
        XamlObjectWriterSettings settings = CreateObjectWriterSettings(_objectWriterParentSettings);
        settings.ExternalNameScope = new NameScope();
        settings.RegisterNamesOnExternalNamescope = true;
        settings.TemplateOwnerReference = new(owner);

        settings.BeforePropertiesHandler =
            delegate (object sender, XamlObjectEventArgs args)
            {
                if (args.Instance is FrameworkElement fe)
                {
                    fe.SetTemplatedParent(settings.TemplateOwnerReference);
                }
            };

        if (LoadXaml(settings) is IFrameworkElement rootElement)
        {
            if (owner is null)
            {
                if (NameScope.GetNameScope(rootElement) is null)
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
        var shouldSetLineInfo = xamlLineInfo is not null && xamlLineConsumer is not null && xamlLineConsumer.ShouldProvideLineInfo && xamlLineInfo.HasLineInfo;

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
        if (parentSettings is not null)
        {
            owSettings.SkipDuplicatePropertyCheck = parentSettings.SkipDuplicatePropertyCheck;
            owSettings.SkipProvideValueOnRoot = parentSettings.SkipProvideValueOnRoot;
        }
        return owSettings;
    }

    internal XamlType GetTypeForName(string name) => _namedTypes?[name];

    private void Initialize(XamlReader xamlReader)
    {
        Debug.Assert(xamlReader.NodeType == XamlNodeType.None);

        using var xamlWriter = new XamlNodeWriter(_xamlNodeList, this);

        while (xamlReader.Read())
        {
            xamlWriter.WriteNode(xamlReader);
        }
    }

    private static bool IsNameProperty(XamlMember member, XamlType owner)
    {
        return member == owner.GetAliasedProperty(XamlLanguage.Name) || XamlLanguage.Name == member;
    }

    private static bool IsNameScope(XamlType type)
    {
        if (typeof(ResourceDictionary).IsAssignableFrom(type.UnderlyingType))
        {
            return true;
        }
        return type.IsNameScope;
    }

    private sealed class XamlNodeWriter : XamlNodeListWriter
    {
        private readonly StackOfFrames _stack;
        private readonly XamlTemplateContent _owner;

        public XamlNodeWriter(XamlNodeList source, XamlTemplateContent owner)
            : base(source)
        {
            _owner = owner;
            _stack = new();
        }

        public override void WriteStartObject(XamlType type)
        {
            base.WriteStartObject(type);
            _stack.Push(type);
        }

        public override void WriteGetObject()
        {
            base.WriteGetObject();
            _stack.Push(_stack.CurrentFrame.Property.Type);
        }

        public override void WriteEndObject()
        {
            base.WriteEndObject();
            _stack.PopScope();
        }

        public override void WriteStartMember(XamlMember xamlMember)
        {
            base.WriteStartMember(xamlMember);
            _stack.CurrentFrame.Property = xamlMember;
        }

        public override void WriteEndMember()
        {
            base.WriteEndMember();
            _stack.CurrentFrame.Property = null;
        }

        public override void WriteValue(object value)
        {
            base.WriteValue(value);
            if (!_stack.CurrentFrame.IsInStyleOrTemplate)
            {
                if (IsNameProperty(_stack.CurrentFrame.Property, _stack.CurrentFrame.Type))
                {
                    if (!_stack.CurrentFrame.IsInNameScope)
                    {
                        // FEs need to be added to the name to index map
                        if (typeof(FrameworkElement).IsAssignableFrom(_stack.CurrentFrame.Type.UnderlyingType))
                        {
                            string name = value as string;

                            _owner._namedTypes ??= [];
                            _owner._namedTypes.Add(name, _stack.CurrentFrame.Type);
                        }
                    }
                }
            }
        }
    }

    private sealed class StackOfFrames
    {
        public StackOfFrames()
        {
            Grow();
            Depth = 0;
        }

        public Frame CurrentFrame { get; private set; }

        public Frame PreviousFrame => CurrentFrame.Previous;

        public int Depth { get; set; }

        public void Push(XamlType xamlType)
        {
            bool isInNameScope = false;
            bool isInStyleOrTemplate = false;

            if (Depth > 0)
            {
                isInNameScope = CurrentFrame.IsInNameScope || (CurrentFrame.Type is not null && IsNameScope(CurrentFrame.Type));
                isInStyleOrTemplate = CurrentFrame.IsInStyleOrTemplate ||
                    (CurrentFrame.Type is not null &&
                     (typeof(FrameworkTemplate).IsAssignableFrom(CurrentFrame.Type.UnderlyingType) ||
                      typeof(Style).IsAssignableFrom(CurrentFrame.Type.UnderlyingType)));
            }

            if (Depth == 0 || CurrentFrame.Type is not null)
            {
                PushScope();
            }

            CurrentFrame.Type = xamlType;
            CurrentFrame.IsInNameScope = isInNameScope;
            CurrentFrame.IsInStyleOrTemplate = isInStyleOrTemplate;
        }

        public void PushScope()
        {
            Grow();
            Depth++;
            Debug.Assert(CurrentFrame.Depth == Depth);
        }

        public void PopScope()
        {
            Depth--;
            CurrentFrame = CurrentFrame.Previous;
            Debug.Assert(CurrentFrame.Depth == Depth);
        }

        private void Grow()
        {
            Frame lastFrame = CurrentFrame;
            CurrentFrame = new Frame
            {
                Previous = lastFrame
            };
        }
    }

    private sealed class Frame
    {
        private XamlType _xamlType;
        private int _depth;
        private Frame _previous;

        public XamlType Type
        {
            get { return _xamlType; }
            set
            {
                // can't change the type (except from null)
                Debug.Assert(_xamlType is null);

                _xamlType = value;
            }
        }

        public XamlMember Property { get; set; }

        public bool IsInNameScope { get; set; }

        public bool IsInStyleOrTemplate { get; set; }

        public int Depth
        {
            get
            {
                Debug.Assert(_depth != -1, "Context Frame is uninitialized");
                return _depth;
            }
        }

        public Frame Previous
        {
            get { return _previous; }
            set
            {
                _previous = value;
                _depth = _previous is null ? 0 : (_previous._depth + 1);
            }
        }
    }
}
