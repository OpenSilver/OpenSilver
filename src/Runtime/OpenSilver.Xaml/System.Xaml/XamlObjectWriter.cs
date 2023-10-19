//
// Copyright (C) 2010 Novell Inc. http://novell.com
// Copyright (C) 2012 Xamarin Inc. http://xamarin.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xaml.Markup;
using System.Xaml.Schema;
using System.Xml;
using System.Xml.Serialization;

// To use this under .NET, compile sources as:
//
//	dmcs -d:DOTNET -r:System.Xaml -debug System.Xaml/XamlObjectWriter.cs System.Xaml/XamlWriterInternalBase.cs System.Xaml/TypeExtensionMethods.cs System.Xaml/XamlWriterStateManager.cs System.Xaml/XamlNameResolver.cs System.Xaml/PrefixLookup.cs System.Xaml/ValueSerializerContext.cs ../../build/common/MonoTODOAttribute.cs Test/System.Xaml/TestedTypes.cs

/*

State transition:

* StartObject or GetObject
    These start a new object instance, either by creating new or getting
    from parent.
* Value
    This either becomes an entire property value, or an item of current
    collection, or a key or a value item of current dictionary, or an
    entire object if it is either Initialization.
* EndObject
    Almost the same as Value. Though the it is likely already instantiated.
* StartMember
    Indicates a new property as current.
* EndMember
    It accompanies a property value (might be lacking), or ends a
    collection (including those for PositionalParameters), or ends a key
    property of a dictionary element (if it is Key), or ends an entire
    value of current object if it is Initialization.


*/


namespace System.Xaml
{
    internal class XamlObjectWriter : XamlWriter, IXamlLineInfoConsumer
    {
        public XamlObjectWriter(XamlSchemaContext schemaContext)
            : this(schemaContext, null)
        {
        }

        public XamlObjectWriter(XamlSchemaContext schemaContext, XamlObjectWriterSettings settings)
            : this(schemaContext, settings, null)
        {
        }

        [EnhancedXaml]
        public XamlObjectWriter(XamlSchemaContext schemaContext, XamlObjectWriterSettings settings, IAmbientProvider parentAmbientProvider)
        {
            if (schemaContext == null)
                throw new ArgumentNullException("schemaContext");
            this.sctx = schemaContext;
            this.settings = settings ?? new XamlObjectWriterSettings();
            var manager = new XamlWriterStateManager<XamlObjectWriterException, XamlObjectWriterException>(false);
            intl = new XamlObjectWriterInternal(this, sctx, manager, parentAmbientProvider);
        }

        XamlSchemaContext sctx;
        XamlObjectWriterSettings settings;
        XamlObjectWriterInternal intl;
        DeferredWriter deferredWriter;

        class DeferredWriter
        {
            public XamlDeferringLoader Loader { get; }
            public XamlNodeList List { get; }
            public XamlWriter Writer { get { return List.Writer; } }
            public int DeferCount { get; set; }
            public DeferredWriter(XamlSchemaContext context, XamlDeferringLoader loader)
            {
                List = new XamlNodeList(context);
                Loader = loader;
            }
        }

        internal XamlObjectWriterSettings Settings
        {
            get { return settings; }
        }

        internal int Line { get; private set; }
        internal int Column { get; private set; }

        public virtual object Result
        {
            get { return intl.Result; }
        }

        public INameScope RootNameScope
        {
            get { return intl.NameScope; }
        }

        public override XamlSchemaContext SchemaContext
        {
            get { return sctx; }
        }

        public bool ShouldProvideLineInfo
        {
            get { return true; }
        }

        public void SetLineInfo(int lineNumber, int linePosition)
        {
            Line = lineNumber;
            Column = linePosition;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            intl.CloseAll();
        }

        protected internal virtual void OnAfterBeginInit(object value)
        {
            if (settings.AfterBeginInitHandler != null)
                settings.AfterBeginInitHandler(this, new XamlObjectEventArgs(value));
        }

        protected internal virtual void OnAfterEndInit(object value)
        {
            if (settings.AfterEndInitHandler != null)
                settings.AfterEndInitHandler(this, new XamlObjectEventArgs(value));
        }

        protected internal virtual void OnAfterProperties(object value)
        {
            if (settings.AfterPropertiesHandler != null)
                settings.AfterPropertiesHandler(this, new XamlObjectEventArgs(value));
        }

        protected internal virtual void OnBeforeProperties(object value)
        {
            if (settings.BeforePropertiesHandler != null)
                settings.BeforePropertiesHandler(this, new XamlObjectEventArgs(value));
        }

        protected internal virtual bool OnSetValue(object eventSender, XamlMember member, object value)
        {
            if (settings.XamlSetValueHandler != null)
            {
                var args = new XamlSetValueEventArgs(member, value);
                settings.XamlSetValueHandler(eventSender, args);
                return args.Handled;
            }
            return false;
        }

        XamlException WithLineInfo(XamlException ex)
        {
            ex.SetLineInfo(Line, Column);
            return ex;
        }

        public override void WriteGetObject()
        {
            if (deferredWriter != null)
            {
                deferredWriter.Writer.WriteGetObject();
                deferredWriter.DeferCount++;
                return;
            }

            intl.WriteGetObject();
        }

        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            if (deferredWriter != null)
            {
                deferredWriter.Writer.WriteNamespace(namespaceDeclaration);
                return;
            }

            intl.WriteNamespace(namespaceDeclaration);
        }

        public override void WriteStartObject(XamlType xamlType)
        {
            if (xamlType.IsUnknown)
            {
                throw WithLineInfo(new XamlObjectWriterException($"Cannot create unknown type '{xamlType}'."));
            }

            if (deferredWriter != null)
            {
                deferredWriter.Writer.WriteStartObject(xamlType);
                deferredWriter.DeferCount++;
                return;
            }

            intl.WriteStartObject(xamlType);
        }

        public override void WriteValue(object value)
        {
            if (deferredWriter != null)
            {
                deferredWriter.Writer.WriteValue(value);
                return;
            }

            intl.WriteValue(value);
        }

        public override void WriteStartMember(XamlMember property)
        {
            if (deferredWriter != null)
            {
                deferredWriter.Writer.WriteStartMember(property);
                deferredWriter.DeferCount++;
                return;
            }

            if (property.IsUnknown)
                throw WithLineInfo(new XamlObjectWriterException($"Cannot set unknown member '{property}'", null));

            intl.WriteStartMember(property);

            var defer = property.DeferringLoader;
            if (defer != null)
            {
                deferredWriter = new DeferredWriter(sctx, defer.ConverterInstance);
                deferredWriter.DeferCount++;
                return;
            }
        }

        public override void WriteEndObject()
        {
            if (deferredWriter != null)
            {
                deferredWriter.Writer.WriteEndObject();
                if (--deferredWriter.DeferCount > 0)
                    return;
                intl.WriteDeferred(deferredWriter.Loader, deferredWriter.List, false);
                deferredWriter = null;
            }

            intl.WriteEndObject();
        }

        public override void WriteEndMember()
        {
            if (deferredWriter != null)
            {
                if (--deferredWriter.DeferCount > 0)
                {
                    deferredWriter.Writer.WriteEndMember();
                    return;
                }
                intl.WriteDeferred(deferredWriter.Loader, deferredWriter.List, true);
                deferredWriter = null;
            }

            intl.WriteEndMember();
        }
    }

    // specific implementation
    class XamlObjectWriterInternal : XamlWriterInternalBase, IXamlObjectWriterFactory
    {
        const string Xmlns2000Namespace = "http://www.w3.org/2000/xmlns/";

        public XamlObjectWriterInternal(XamlObjectWriter source, XamlSchemaContext schemaContext, XamlWriterStateManager manager)
            : this(source, schemaContext, manager, null)
        {
        }
        public XamlObjectWriterInternal(
            XamlObjectWriter source, XamlSchemaContext schemaContext,
            XamlWriterStateManager manager, IAmbientProvider parentAmbientProvider)
            : base(schemaContext, manager, parentAmbientProvider)
        {
            this.source = source;
            var ext = source.Settings.ExternalNameScope;
            name_scope = ext != null && source.Settings.RegisterNamesOnExternalNamescope ? ext : new NameScope(ext);
        }

        XamlObjectWriter source;
        INameScope name_scope;
        List<NameFixupRequired> pending_name_references = new List<NameFixupRequired>();

        public INameScope NameScope
        {
            get { return name_scope; }
        }

        public object Result { get; set; }

        protected override void OnWriteStartObject()
        {
            var state = object_states.Pop();
            if (object_states.Count > 0)
            {
                var pstate = object_states.Peek();
                if (CurrentMemberState.Value != null)
                    throw WithLineInfo(new XamlDuplicateMemberException(CurrentMember, pstate.Type));
            }
            else
            {
                var obj = source.Settings.RootObjectInstance;
                if (obj != null)
                {
                    if (state.Type.UnderlyingType != null && !state.Type.UnderlyingType.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo()))
                        throw WithLineInfo(new XamlObjectWriterException(
                            String.Format("RootObjectInstance type '{0}' is not assignable to '{1}'", obj.GetType(), state.Type)));
                    state.Value = obj;
                    state.IsInstantiated = true;
                    HandleBeginInit(obj);
                    source.OnBeforeProperties(state.Value);
                }
                root_state = state;
            }
            object_states.Push(state);
            if (!state.Type.IsContentValue(service_provider))
                InitializeObjectIfRequired(true);
            state.IsXamlWriterCreated = true;
        }

        protected override void OnWriteGetObject()
        {
            var state = object_states.Pop();
            InitializeObjectIfRequired(false, true);
            var xm = CurrentMember;
            var instance = xm.Invoker.GetValue(object_states.Peek().Value);
            if (state.Type.IsImmutable)
                instance = state.Type.Invoker.ToMutable(instance);
            if (instance == null)
                throw WithLineInfo(new XamlObjectWriterException(String.Format("The value  for '{0}' property is null", xm.Name)));

            //if the type is immutable then we need set value
            if (!state.Type.IsImmutable)
                object_states.Peek().IsValueProvidedByParent = true;
            state.Value = instance;
            state.IsInstantiated = true;
            object_states.Push(state);
        }

        protected override void OnWriteEndObject()
        {
            InitializeObjectIfRequired(false, true); // this is required for such case that there was no StartMember call.

            var state = object_states.Pop();
            var obj = state.Value;
            if (state.Type.IsImmutable)
                obj = state.Type.Invoker.ToImmutable(obj);

            if (state.Type.IsMarkupExtension)
            {
                // validate that the provided value is a markup extension, throws InvalidCastException if not
                var markupExtension = (MarkupExtension)obj;
                try
                {
                    obj = markupExtension.ProvideValue(service_provider);
                }
                catch (XamlObjectWriterException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw WithLineInfo(new XamlObjectWriterException("An error occured getting provided value", ex));
                }
            }

            // call this (possibly) before the object is added to parent collection. (bug #3003 also expects this)
            if (obj != null && state.IsXamlWriterCreated)
                source.OnAfterProperties(obj);

            var nfr = obj as NameFixupRequired;
            if (nfr != null && object_states.Count > 0)
            {
                // IF the root object to be written is x:Reference, then the Result property will become the NameFixupRequired. That's what .NET also does.
                // actually .NET seems to seek "parent" object in its own IXamlNameResolver implementation.
                nfr.State = object_states.Peek();
                nfr.MemberState = nfr.State.CurrentMemberState;

                if (nfr.Type.IsCollection && !nfr.Type.IsDictionary && nfr.Value is IList)
                {
                    // ensure the collection is in the same order as in xaml, so add a null in place
                    // and we will replace it later
                    var list = (IList)nfr.Value;
                    nfr.ListIndex = list.Count;
                    list.Add(null);
                }

                if (nfr.Type.IsImmutable && object_states.Count > 0)
                {
                    // if it's immutable, we need to set the parent member after getting the real value
                    object_states.Pop();
                    nfr.ParentState = object_states.Peek();
                    nfr.ParentMemberState = nfr.ParentState.CurrentMemberState;
                    object_states.Push(nfr.State);
                }
                pending_name_references.Add((NameFixupRequired)obj);
            }
            else
            {
                // UsableDuringInitialization type may have already been attached to its parent in OnWriteStartMember.
                if (!(state.Type.IsUsableDuringInitialization && state.WrittenProperties.Count > 0 && CurrentMemberState?.IsAlreadySet == true))
                {
                    StoreAppropriatelyTypedValue(obj, state.KeyValue);
                }
            }

            if (!state.IsGetObject)
                HandleEndInit(obj);

            object_states.Push(state);
            if (object_states.Count == 1)
            {
                Result = obj;
                ResolvePendingReferences();
            }
        }

        Stack<object> escaped_objects = new Stack<object>();

        protected override void OnWriteStartMember(XamlMember property)
        {
            if (ReferenceEquals(property, XamlLanguage.PositionalParameters) ||
                ReferenceEquals(property, XamlLanguage.Arguments))
            {
                var state = object_states.Peek();
                escaped_objects.Push(state.Value);
                state.Value = new List<object>();
            }

            // FIXME: this condition needs to be examined. What is known to be prevented are: PositionalParameters, Initialization and Base (the last one sort of indicates there's a lot more).
            else
            {
                if (property == XamlLanguage.UnknownContent)
                    throw WithLineInfo(new XamlObjectWriterException($"Type '{object_states.Peek().Type}' does not have a content property."));
                if (property.IsUnknown)
                    throw WithLineInfo(new XamlObjectWriterException($"Cannot set unknown member '{property}'"));
                if (!property.IsDirective || ReferenceEquals(property, XamlLanguage.Name)) // x:Name requires an object instance
                {
                    InitializeObjectIfRequired(false);

                    ObjectState state;
                    if (object_states.Count > 1)
                    {
                        state = object_states.Pop();
                        var parent_state = object_states.Peek();
                        object_states.Push(state);

                        if (state.Type.IsUsableDuringInitialization)
                        {
                            // Make sure that we invoke this block only once, while setting the very first property.
                            if (state.WrittenProperties.Count == 1)
                            {
                                if (!AddToCollectionIfAppropriate(parent_state.Type, parent_state.CurrentMember, parent_state.Value, state.Value, state.KeyValue) &&
                                    !parent_state.CurrentMemberState.IsAlreadySet)
                                {
                                    SetValue(parent_state.CurrentMember, parent_state.Value, state.Value);
                                }

                                parent_state.CurrentMemberState.IsAlreadySet = true;
                            }
                        }
                    }
                }
            }
        }

        //static readonly BindingFlags static_flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        protected override void OnWriteEndMember()
        {
            var xm = CurrentMember;
            var state = object_states.Peek();

            if (ReferenceEquals(xm, XamlLanguage.PositionalParameters))
            {
                var l = (List<object>)state.Value;
                state.Value = escaped_objects.Pop();
                state.IsInstantiated = true;
                PopulateObject(true, l);
                return;
            }
            else if (ReferenceEquals(xm, XamlLanguage.Arguments))
            {
                if (state.FactoryMethod != null)
                {
                    var contents = (List<object>)state.Value;
                    bool found = false;
                    foreach (var mi in state.Type.UnderlyingType.GetRuntimeMethods())
                    {
                        if (mi.Name == state.FactoryMethod && mi.IsStatic && mi.GetParameters().Length == contents.Count)
                        {
                            state.Value = mi.Invoke(null, contents.ToArray());
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                        throw WithLineInfo(new XamlObjectWriterException(
                            String.Format("Specified static factory method '{0}' for type '{1}' was not found", state.FactoryMethod, state.Type)));
                }
                else
                    PopulateObject(true, (List<object>)state.Value);
                state.IsInstantiated = true;
                escaped_objects.Pop();
            }
            else if (ReferenceEquals(xm, XamlLanguage.Initialization))
            {
                // ... and no need to do anything. The object value to pop *is* the return value.
            }
            else
            {
                XamlMember aliasedName = state.Type.GetAliasedProperty(XamlLanguage.Name);
                if (ReferenceEquals(xm, XamlLanguage.Name) || xm == aliasedName)
                {
                    string name = (string)CurrentMemberState.Value;
                    name_scope.RegisterName(name, state.Value);

                    // if x:Name is used, then we set the backing property defined by RuntimeNamePropertyAttribute
                    xm = aliasedName ?? xm;
                }

                if (!xm.IsReadOnly) // exclude read-only object such as collection item.
                    SetValue(xm, CurrentMemberState.Value);
            }
        }

        void SetValue(XamlMember member, object value)
        {
            if (ReferenceEquals(member, XamlLanguage.FactoryMethod))
                object_states.Peek().FactoryMethod = (string)value;
            else if (member.IsDirective)
                return;
            else
            {
                var state = object_states.Peek();
                // won't be instantiated yet if dealing with a type that has no default constructor
                if (state.IsInstantiated && !state.CurrentMemberState.IsAlreadySet
                                         && !(state.IsValueProvidedByParent && state.CurrentMember.Type.IsCollection))
                    SetValue(member, state.Value, value);
            }
        }

        void SetValue(XamlMember member, object target, object value)
        {
            try
            {
                if (!source.OnSetValue(target, member, value))
                    member.Invoker.SetValue(target, value);
            }
            catch (TargetInvocationException ex)
            {
                throw WithLineInfo(new XamlObjectWriterException($"Set value of member '{member}' threw an exception", ex.InnerException));
            }
            catch (Exception ex)
            {
                throw WithLineInfo(new XamlObjectWriterException($"Set value of member '{member}' threw an exception", ex));
            }
        }

        void PopulateObject(bool considerPositionalParameters, IList<object> contents)
        {
            var state = object_states.Peek();

            var positionalParameters = considerPositionalParameters ? state.Type.GetPositionalParameters(contents.Count) : null;

            var args = state.Type.GetSortedConstructorArguments(contents)?.ToArray();
            var argt = args != null ? (from arg in args select arg.Type).ToArray() : positionalParameters;
            if (argt == null)
                throw WithLineInfo(new XamlObjectWriterException($"Could not find matching constructor for type {state.Type}"));

            var argv = new object[argt.Count];
            for (int i = 0; i < argv.Length; i++)
                argv[i] = GetCorrectlyTypedValue(args[i], argt[i], i < contents.Count ? contents[i] : null);
            state.Value = state.Type.Invoker.CreateInstance(argv);
            state.IsInstantiated = true;
            HandleBeginInit(state.Value);
            source.OnBeforeProperties(state.Value);
        }

        protected override void OnWriteValue(object value)
        {
            if (CurrentMemberState.Value != null)
                throw WithLineInfo(new XamlDuplicateMemberException(CurrentMember, object_states.Peek().Type));
            StoreAppropriatelyTypedValue(value, null);
        }

        protected override void OnWriteNamespace(NamespaceDeclaration nd)
        {
            // nothing to do here.
        }

        protected override XamlException WithLineInfo(XamlException ex)
        {
            ex.SetLineInfo(source.Line, source.Column);
            return ex;
        }

        void StoreAppropriatelyTypedValue(object obj, object keyObj)
        {
            var ms = CurrentMemberState;
            if (ms != null)
                StoreAppropriatelyTypedValue(object_states.Peek(), ms, obj, keyObj);
        }

        void StoreAppropriatelyTypedValue(ObjectState state, MemberAndValue ms, object obj, object keyObj)
        {
            var parent = state.Value;
            var xt = state.Type;
            var xm = ms.Member;
            if (ReferenceEquals(xm, XamlLanguage.Initialization))
            {
                state.Value = GetCorrectlyTypedValue(null, xt, obj);
                state.IsInstantiated = true;
            }
            else if (xm.Type.IsXData)
            {
                var xdata = (XData)obj;

                var ixser = xm.Invoker.GetValue(state.Value) as IXmlSerializable;
                if (ixser != null)
                    ixser.ReadXml((XmlReader)xdata.XmlReader);
            }
            else if (ReferenceEquals(xm, XamlLanguage.Base))
                ms.Value = GetCorrectlyTypedValue(null, xm.Type, obj);
            else if (ReferenceEquals(xm, XamlLanguage.Name) || xm == xt.GetAliasedProperty(XamlLanguage.Name))
                ms.Value = GetCorrectlyTypedValue(xm, XamlLanguage.String, obj);
            else if (ReferenceEquals(xm, XamlLanguage.Key))
            {
                var keyValue = GetCorrectlyTypedValue(null, xt.KeyType, obj);
                state.KeyValue = keyValue;
                ms.Value = keyValue;
            }
            else
            {
                if (!AddToCollectionIfAppropriate(xt, xm, parent, obj, keyObj))
                {
                    if (!xm.IsReadOnly || xm.IsConstructorArgument)
                        ms.Value = GetCorrectlyTypedValue(xm, xm.Type, obj);
                }
            }
        }

        bool AddToCollectionIfAppropriate(XamlType xt, XamlMember xm, object parent, object obj, object keyObj)
        {
            var mt = xm.Type;
            if (ReferenceEquals(xm, XamlLanguage.Items) ||
                ReferenceEquals(xm, XamlLanguage.PositionalParameters) ||
                ReferenceEquals(xm, XamlLanguage.Arguments))
            {
                if (xt.IsDictionary)
                {
                    var key = keyObj;
                    if (key == null && obj != null)
                        key = GetKeyFromInstance(obj, sctx.GetXamlType(obj.GetType()));
                    mt.Invoker.AddToDictionary(parent, GetCorrectlyTypedValue(null, xt.KeyType, key), GetCorrectlyTypedValue(null, xt.ItemType, obj));
                }
                else // collection. Note that state.Type isn't usable for PositionalParameters to identify collection kind.
                    mt.Invoker.AddToCollection(parent, GetCorrectlyTypedValue(null, xt.ItemType, obj, true));
                return true;
            }
            else
                return false;
        }

        object GetKeyFromInstance(object instance, XamlType instanceType)
        {
            XamlMember keyProperty = instanceType.GetAliasedProperty(XamlLanguage.Key);
            if (keyProperty == null || instance == null)
            {
                throw WithLineInfo(new XamlObjectWriterException(string.Format("Missing key value on '{0}' object.", instanceType.Name)));
            }
            object key = keyProperty.Invoker.GetValue(instance);
            return key;
        }

        // It expects that it is not invoked when there is no value to 
        // assign.
        // When it is passed null, then it returns a default instance.
        // For example, passing null as Int32 results in 0.
        // But do not immediately try to instantiate with the type, since the type might be abstract.
        object GetCorrectlyTypedValue(XamlMember xm, XamlType xt, object value, bool fallbackToString = false)
        {
            try
            {
                if (value == null)
                {
                    if (xt.IsContentValue(service_provider)) // it is for collection/dictionary key and item
                        return null;
                    else
                        return xt.IsNullable ? null : xt.Invoker.CreateInstance(new object[0]);
                }
                if (ReferenceEquals(xt, null))
                    return value;

                // FIXME: this could be generalized by some means, but I cannot find any.
                if (xt.UnderlyingType == typeof(XamlType) && value is string)
                    value = ResolveTypeFromName((string)value);

                // FIXME: this could be generalized by some means, but I cannot find any.
                if (xt.UnderlyingType == typeof(Type))
                    value = new TypeExtension((string)value).ProvideValue(service_provider);
                if (ReferenceEquals(xt, XamlLanguage.Type) && value is string)
                    value = new TypeExtension((string)value);

                var xtc = xm?.TypeConverter ?? xt.TypeConverter;
                if (xtc != null && value != null)
                {
                    var tc = xtc.ConverterInstance;
                    if (tc != null && tc.CanConvertFrom(service_provider, value.GetType()))
                        value = tc.ConvertFrom(service_provider, CultureInfo.InvariantCulture, value);
                    return value;
                }

                if (IsAllowedType(xt, value))
                    return value;
            }
            catch (Exception ex)
            {
                // For + ex.Message, the runtime should print InnerException message like .NET does.
                throw WithLineInfo(new XamlObjectWriterException(
                    String.Format("Could not convert object \'{0}' (of type {1}) to {2}: ", value, value != null ? (object)value.GetType() : "(null)", xt) + ex.Message,
                    ex));
            }

            return fallbackToString ?
                value :
                throw WithLineInfo(new XamlObjectWriterException(
                    String.Format("Value '{0}' (of type {1}) is not of or convertible to type {2} (member {3})", value, value != null ? (object)value.GetType() : "(null)", xt, xm),
                    null));
        }

        XamlType ResolveTypeFromName(string name)
        {
            var nsr = (IXamlNamespaceResolver)service_provider.GetService(typeof(IXamlNamespaceResolver));
            return sctx.GetXamlType(XamlTypeName.Parse(name, nsr));
        }

        static bool IsAllowedType(XamlType xt, object value)
        {
            return ReferenceEquals(xt, null) ||
                xt.UnderlyingType == null ||
                xt.UnderlyingType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()) ||
                value == null && ReferenceEquals(xt, XamlLanguage.Null) ||
                xt.IsMarkupExtension && IsAllowedType(xt.MarkupExtensionReturnType, value);
        }

        void InitializeObjectIfRequired(bool waitForParameters, bool required = false)
        {
            var state = object_states.Peek();
            if (state.IsInstantiated)
                return;

            object obj = null;
            if ((state.Type.ConstructionRequiresArguments && !required)
                || (waitForParameters && state.Type.HasPositionalParameters(service_provider)))
            {
                if (!state.Type.IsImmutable)
                    return;

                obj = state.Type.Invoker.ToMutable(null);
                if (obj == null)
                    return;
            }

            if (obj == null)
            {
                // FIXME: "The default techniques in absence of a factory method are to attempt to find a default constructor, then attempt to find an identified type converter on type, member, or destination type."
                // http://msdn.microsoft.com/en-us/library/System.Xaml.xamllanguage.factorymethod%28VS.100%29.aspx
                if (state.FactoryMethod != null) // FIXME: it must be implemented and verified with tests.
                    throw new NotImplementedException();
                else
                {
                    if (state.Type.ConstructionRequiresArguments)
                    {
                        var constructorProps = state.WrittenProperties.Where(r => r.Member.IsConstructorArgument).ToList();

                        // immutable type (no default constructor), so we create based on supplied constructor arguments 
                        var args = state.Type.GetSortedConstructorArguments(constructorProps)?.ToList();
                        if (args == null)
                            throw WithLineInfo(new XamlObjectWriterException($"Could not find constructor for {state.Type} based on supplied members"));

                        var argValues = args.Select(r => r.Value).ToArray();

                        obj = state.Type.Invoker.CreateInstance(argValues);
                        state.Value = obj;
                        state.IsInstantiated = true;
                        HandleBeginInit(obj);
                        source.OnBeforeProperties(state.Value);

                        // set other writable properties now that the object is instantiated
                        foreach (var prop in state.WrittenProperties.Where(p => args.All(r => r.Member != p.Member)))
                        {
                            if (prop.Member.IsReadOnly && prop.Member.IsConstructorArgument)
                                throw WithLineInfo(new XamlObjectWriterException($"Member {prop.Member} is read only and cannot be used in any constructor"));
                            if (!prop.Member.IsReadOnly)
                                SetValue(prop.Member, prop.Value);
                        }
                        return;
                    }
                    else
                        obj = state.Type.Invoker.CreateInstance(null);

                    if (state.Type.IsImmutable)
                        obj = state.Type.Invoker.ToMutable(obj);
                }
            }

            state.Value = obj;
            state.IsInstantiated = true;
            HandleBeginInit(obj);
            source.OnBeforeProperties(state.Value);
        }

        internal IXamlNameResolver name_resolver
        {
            get { return (IXamlNameResolver)service_provider.GetService(typeof(IXamlNameResolver)); }
        }

        void ResolvePendingReferences()
        {
            foreach (var fixup in pending_name_references)
            {
                foreach (var name in fixup.Names)
                {
                    bool isFullyInitialized;
                    // FIXME: sort out relationship between name_scope and name_resolver. (unify to name_resolver, probably)
                    var obj = name_scope.FindName(name) ?? name_resolver.Resolve(name, out isFullyInitialized);
                    if (obj == null)
                        throw WithLineInfo(new XamlObjectWriterException(String.Format("Unresolved object reference '{0}' was found", name)));

                    if (fixup.ListIndex != null)
                        ((IList)fixup.Value)[fixup.ListIndex.Value] = obj;
                    else if (!AddToCollectionIfAppropriate(fixup.Type, fixup.Member, fixup.Value, obj, fixup.KeyValue))
                        SetValue(fixup.Member, fixup.Value, obj);
                }
            }

            var handled_values = new HashSet<object>();
            foreach (var fixup in pending_name_references)
            {
                if (fixup.Type.IsImmutable && !handled_values.Contains(fixup.Value))
                {
                    var value = fixup.Type.Invoker.ToImmutable(fixup.Value);
                    SetValue(fixup.ParentMemberState.Member, fixup.ParentState.Value, value);

                    handled_values.Add(fixup.Value);
                }
            }
        }

        void HandleBeginInit(object value)
        {
			var si = value as ISupportInitialize;
			if (si == null)
				return;
			si.BeginInit ();
			source.OnAfterBeginInit (value);
        }

        void HandleEndInit(object value)
        {
			var si = value as ISupportInitialize;
			if (si == null)
				return;
			si.EndInit ();
			source.OnAfterEndInit (value);
        }

        public void WriteDeferred(XamlDeferringLoader loader, XamlNodeList nodeList, bool setValue)
        {
            nodeList.Writer.Close();
            var obj = loader.Load(nodeList.GetReader(), service_provider);
            var cms = CurrentMemberState;
            if (cms != null)
                cms.Value = obj;
            else
            {
                var state = object_states.Peek();
                state.Value = obj;
                state.IsInstantiated = true;
            }
            if (setValue)
                manager.Value();
        }

        public XamlObjectWriterSettings GetParentSettings()
        {
            return source.Settings;
        }

        public XamlObjectWriter GetXamlObjectWriter(XamlObjectWriterSettings settings)
        {
            return new XamlObjectWriter(sctx, settings);
        }
    }
}

