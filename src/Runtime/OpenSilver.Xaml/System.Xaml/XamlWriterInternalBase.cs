//
// Copyright (C) 2010 Novell Inc. http://novell.com
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml.Markup;


namespace System.Xaml
{
	abstract class XamlWriterInternalBase : IProvideValueTarget, IRootObjectProvider, IDestinationTypeProvider
	{
		protected XamlWriterInternalBase(XamlSchemaContext schemaContext, XamlWriterStateManager manager)
			: this(schemaContext, manager, null)
		{
		}

		protected XamlWriterInternalBase(XamlSchemaContext schemaContext, XamlWriterStateManager manager, IAmbientProvider parentAmbientProvider = null)
		{
			this.object_states = new ObjectStateStack(parentAmbientProvider);
			this.sctx = schemaContext;
			this.manager = manager;
			var p = new PrefixLookup(sctx) { IsCollectingNamespaces = true }; // it does not raise unknown namespace error.
			service_provider = ValueSerializerContext.Create(p, schemaContext, object_states.GetAmbientProvider, this, this, this, this as IXamlObjectWriterFactory);
		}

		internal XamlSchemaContext sctx;
		internal XamlWriterStateManager manager;

		internal ValueSerializerContext service_provider;

		internal ObjectState root_state;
		internal readonly ObjectStateStack object_states;
		internal PrefixLookup prefix_lookup => (PrefixLookup)service_provider.GetService(typeof(INamespacePrefixLookup));

		public Type GetDestinationType() => CurrentMember?.Type.UnderlyingType;

		List<NamespaceDeclaration> Namespaces => prefix_lookup.Namespaces;

		internal class ObjectState
		{
			public XamlType Type;
			FlagValue _flags;
			static class ObjectStateFlags
			{
				public const int IsGetObject = 1 << 0;
				public const int IsInstantiated = 1 << 1;
				public const int IsXamlWriterCreated = 1 << 2;
				public const int IsValueProvidedByParent = 1 << 3;
				public const int HasSeparator = 1 << 4;
			}

			public bool IsGetObject
			{
				get { return _flags.Get(ObjectStateFlags.IsGetObject) ?? false; }
				set { _flags.Set(ObjectStateFlags.IsGetObject, value); }
			}
			public bool IsInstantiated
			{
				get { return _flags.Get(ObjectStateFlags.IsInstantiated) ?? false; }
				set { _flags.Set(ObjectStateFlags.IsInstantiated, value); }
			}
			public bool IsXamlWriterCreated // affects AfterProperties() calls.
			{
				get { return _flags.Get(ObjectStateFlags.IsXamlWriterCreated) ?? false; }
				set { _flags.Set(ObjectStateFlags.IsXamlWriterCreated, value); }
			}
			public bool HasSeparator
			{
				get { return _flags.Get(ObjectStateFlags.HasSeparator) ?? false; }
				set { _flags.Set(ObjectStateFlags.HasSeparator, value); }
			}

			public int PositionalParameterIndex = -1;

			public bool IsValueProvidedByParent // affects AfterProperties() calls.
			{
				get { return _flags.Get(ObjectStateFlags.IsValueProvidedByParent) ?? false; }
				set { _flags.Set(ObjectStateFlags.IsValueProvidedByParent, value); }
			}
			public string FactoryMethod;
			public object Value;
			public object KeyValue;
			public List<MemberAndValue> WrittenProperties = new List<MemberAndValue>();

			public XamlMember CurrentMember => CurrentMemberState?.Member;

			public MemberAndValue CurrentMemberState
			{
				get { return WrittenProperties.Count > 0 ? WrittenProperties[WrittenProperties.Count - 1] : null; }
			}
		}

		object IProvideValueTarget.TargetObject => object_states.Peek().Value;

		object IProvideValueTarget.TargetProperty => GetTargetProperty(this);

		private static object GetTargetProperty(XamlWriterInternalBase intl)
		{
			if (intl.CurrentMember is IProvideValueTarget ipvt)
			{
				return ipvt.TargetProperty;
			}

			//
			// We should never have a null ParentProperty here but
			// protect against null refs since we are going to dereference it
			if (intl.CurrentMember is not XamlMember parentProperty)
			{
				return null;
			}

			if (parentProperty.IsAttachable)
			{
				//
				// IPVT returns the static Set method for attached properties in 3.0
				return parentProperty.UnderlyingSetter;
			}
			else
			{
				//
				// This branch cover regular property (will return non null)
				// and items in a collection/diction (will return null - IPVT returns null in 3.0 for collections/dictionaries).
				return parentProperty.UnderlyingMember;
			}
		}

		object IRootObjectProvider.RootObject => root_state.Value;

		internal class MemberAndValue
		{
			public MemberAndValue(XamlMember xm)
			{
				Member = xm;
			}

			public XamlMember Member;
			public object Value;
			public AllowedMemberLocations OccuredAs = AllowedMemberLocations.None;
			public bool IsAlreadySet;
		}

		public virtual void CloseAll()
		{
			while (object_states.Count > 0)
			{
				switch (manager.State)
				{
					case XamlWriteState.MemberDone:
					case XamlWriteState.ObjectStarted: // StartObject without member
						WriteEndObject();
						break;
					case XamlWriteState.ValueWritten:
					case XamlWriteState.ObjectWritten:
					case XamlWriteState.MemberStarted: // StartMember without content
						manager.OnClosingItem();
						WriteEndMember();
						break;
					default:
						throw new NotImplementedException(manager.State.ToString()); // there shouldn't be anything though
				}
			}
		}

		internal string GetPrefix(string ns)
		{
			foreach (var nd in Namespaces)
				if (nd.Namespace == ns)
					return nd.Prefix;
			return null;
		}

		protected ObjectState CurrentState => object_states.Count > 0 ? object_states.Peek() : null;
		protected MemberAndValue CurrentMemberState => CurrentState?.CurrentMemberState;
		protected XamlMember CurrentMember => CurrentMemberState?.Member;

		protected XamlMember LastMember => LastState?.CurrentMemberState?.Member;
		protected ObjectState LastState
		{
			get
			{
				if (object_states.Count > 1)
				{
					var state = object_states.Pop();
					var last = object_states.Peek();
					object_states.Push(state);
					return last;
				}
				return null;
			}

		}

		public void WriteGetObject()
		{
			manager.GetObject();

			var xm = CurrentMember;

			var state = new ObjectState() { Type = xm.Type, IsGetObject = true };

			object_states.Push(state);

			OnWriteGetObject();
			manager.GotObject(object_states.Peek().Type);
		}

		public void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
		{
			if (namespaceDeclaration == null)
				throw new ArgumentNullException("namespaceDeclaration");

			manager.Namespace();

			Namespaces.Add(namespaceDeclaration);
			OnWriteNamespace(namespaceDeclaration);
		}

		public void WriteStartObject(XamlType xamlType)
		{
			if (ReferenceEquals(xamlType, null))
				throw new ArgumentNullException("xamlType");

			manager.StartObject(xamlType);
			var cstate = new ObjectState() { Type = xamlType };
			object_states.Push(cstate);

			OnWriteStartObject();
		}

		public void WriteValue(object value)
		{
			manager.Value();

			OnWriteValue(value);
		}

		public void WriteStartMember(XamlMember property)
		{
			if (ReferenceEquals(property, null))
				throw new ArgumentNullException("property");

			manager.StartMember();
			if (ReferenceEquals(property, XamlLanguage.PositionalParameters))
				// this is an exception that indicates the state manager to accept more than values within this member.
				manager.AcceptMultipleValues = true;

			var state = object_states.Peek();
			var wpl = state.WrittenProperties;
			for (int i = 0; i < wpl.Count; i++)
			{
				var wp = wpl[i];
				if (ReferenceEquals(wp.Member, property))
					throw WithLineInfo(new XamlDuplicateMemberException(property, object_states.Peek().Type));
			}

			wpl.Add(new MemberAndValue(property));
			if (ReferenceEquals(property, XamlLanguage.PositionalParameters))
				state.PositionalParameterIndex = 0;

			OnWriteStartMember(property);

		}

		public void WriteEndObject()
		{
			manager.EndObject(object_states.Count > 1);

			OnWriteEndObject();

			object_states.Pop();
		}

		public void WriteEndMember()
		{

			manager.EndMember();

			OnWriteEndMember();

			var state = object_states.Peek();
			if (ReferenceEquals(CurrentMember, XamlLanguage.PositionalParameters))
			{
				manager.AcceptMultipleValues = false;
				state.PositionalParameterIndex = -1;
			}
		}

		protected abstract void OnWriteEndObject();

		protected abstract void OnWriteEndMember();

		protected abstract void OnWriteStartObject();

		protected abstract void OnWriteGetObject();

		protected abstract void OnWriteStartMember(XamlMember xm);

		protected abstract void OnWriteValue(object value);

		protected abstract void OnWriteNamespace(NamespaceDeclaration nd);

		protected string GetValueString(XamlMember xm, object value)
		{
			// change XamlXmlReader too if we change here.
			if (value is string stringValue)
				return stringValue;

			var xt = value == null ? XamlLanguage.Null : sctx.GetXamlType(value.GetType());
			var vs = xm.ValueSerializer ?? xt.ValueSerializer;
			if (vs != null)
				return vs.ConverterInstance.ConvertToString(value, service_provider);
			else
				throw new XamlXmlWriterException(String.Format("Value type is '{0}' but it must be either string or any type that is convertible to string indicated by TypeConverterAttribute.", value != null ? value.GetType() : null));
		}

		protected abstract XamlException WithLineInfo(XamlException ex);

		internal class ObjectStateStack
		{
			private readonly Stack<StackItem> stack = new Stack<StackItem>();
			private readonly IAmbientProvider parentAmbientProvider;

			public ObjectStateStack(IAmbientProvider parentAmbientProvider = null)
			{
				this.parentAmbientProvider = parentAmbientProvider;
			}

			public ObjectState Peek() => stack.Peek().ObjectState;

			public ObjectState Pop() => stack.Pop().ObjectState;

			public void Push(ObjectState item) => stack.Push(new StackItem(item));

			public int Count => stack.Count;

			public IAmbientProvider GetAmbientProvider()
			{
				// if we already have one for this stack state, provide
				var existingProvider = stack.Peek().AmbientProvider;
				if (existingProvider != null)
				{
					return existingProvider;
				}
				// else, create one, add it to stack state and return.
				var provider = CreateAmbientProvider();
				var state = Pop();
				stack.Push(new StackItem(state, provider));
				return provider;
			}
			private IAmbientProvider CreateAmbientProvider()
			{
				IAmbientProvider basicAmbientProvider = new ObjectStatesAmbientProvider(stack.Select(x => x.ObjectState));
				var ambientProvider =
					parentAmbientProvider == null ? basicAmbientProvider
						: new StackAmbientProvider(basicAmbientProvider, parentAmbientProvider);
				return ambientProvider;
			}

			private struct StackItem
			{
				public StackItem(ObjectState objectState, IAmbientProvider provider = null)
				{
					ObjectState = objectState;
					AmbientProvider = provider;
				}
				public readonly ObjectState ObjectState;
				public readonly IAmbientProvider AmbientProvider;
			}
		}

		private class ObjectStatesAmbientProvider : IAmbientProvider
		{
			public ObjectStatesAmbientProvider(IEnumerable<ObjectState> objectStates)
			{
				object_states = objectStates.ToArray();
			}

			private IReadOnlyCollection<ObjectState> object_states { get; }

			public IEnumerable<object> GetAllAmbientValues(params XamlType[] types)
			{
				return GetAllAmbientValues(null, false, types).Select(r => r.Value);
			}

			public IEnumerable<AmbientPropertyValue> GetAllAmbientValues(IEnumerable<XamlType> ceilingTypes, params XamlMember[] properties)
			{
				return GetAllAmbientValues(ceilingTypes, false, null, properties);
			}

			public IEnumerable<AmbientPropertyValue> GetAllAmbientValues(IEnumerable<XamlType> ceilingTypes, bool searchLiveStackOnly, IEnumerable<XamlType> types, params XamlMember[] properties)
			{
				// check arguments
				if (properties == null)
					throw new ArgumentNullException("properties");

				var nonAmbientProperty = properties.FirstOrDefault(r => !r.IsAmbient);
				if (nonAmbientProperty != null)
					throw new ArgumentException(nonAmbientProperty.ToString() + "is not an ambient property", "properties");

				return DoGetAllAmbientValues(ceilingTypes, searchLiveStackOnly, types, properties);
			}

			private IEnumerable<AmbientPropertyValue> DoGetAllAmbientValues(IEnumerable<XamlType> ceilingTypes, bool searchLiveStackOnly, IEnumerable<XamlType> types, params XamlMember[] properties)
			{
				foreach (var state in object_states)
				{
					if (types != null)
					{
						if (types.Any(xt => xt.UnderlyingType != null && state.Type.CanAssignTo(xt)))
							yield return new AmbientPropertyValue(null, state.Value);
					}
					if (properties != null)
					{
						// get ambient properties in the stack
						foreach (var prop in properties)
						{
							if (!state.Type.CanAssignTo(prop.DeclaringType))
								continue;
							var value = prop.Invoker.GetValue(state.Value);
							yield return new AmbientPropertyValue(prop, value);
						}
					}

					if (ceilingTypes != null && ceilingTypes.Contains(state.Type))
						yield break;
				}
			}

			public object GetFirstAmbientValue(params XamlType[] types)
			{
				return GetAllAmbientValues(types).FirstOrDefault();
			}

			public AmbientPropertyValue GetFirstAmbientValue(IEnumerable<XamlType> ceilingTypes, params XamlMember[] properties)
			{
				return GetAllAmbientValues(ceilingTypes, properties).FirstOrDefault();
			}
		}

		internal class StackAmbientProvider : IAmbientProvider
		{
			public StackAmbientProvider(params IAmbientProvider[] providers)
			{
				Providers = providers;
			}

			private IAmbientProvider[] Providers { get; }

			public IEnumerable<object> GetAllAmbientValues(params XamlType[] types)
			{
				return Providers.SelectMany(x => x.GetAllAmbientValues(types));
			}

			public IEnumerable<AmbientPropertyValue> GetAllAmbientValues(IEnumerable<XamlType> ceilingTypes, params XamlMember[] properties)
			{
				return Providers.SelectMany(x => x.GetAllAmbientValues(ceilingTypes, properties));
			}

			public IEnumerable<AmbientPropertyValue> GetAllAmbientValues(IEnumerable<XamlType> ceilingTypes, bool searchLiveStackOnly, IEnumerable<XamlType> types,
				params XamlMember[] properties)
			{
				return Providers.SelectMany(x => x.GetAllAmbientValues(ceilingTypes, searchLiveStackOnly, types, properties));
			}

			public object GetFirstAmbientValue(params XamlType[] types)
			{
				return Providers.SelectMany(x => x.GetAllAmbientValues(types)).FirstOrDefault();
			}

			public AmbientPropertyValue GetFirstAmbientValue(IEnumerable<XamlType> ceilingTypes, params XamlMember[] properties)
			{
				return Providers.SelectMany(x => x.GetAllAmbientValues(ceilingTypes, properties)).FirstOrDefault();
			}
		}
	}
}
