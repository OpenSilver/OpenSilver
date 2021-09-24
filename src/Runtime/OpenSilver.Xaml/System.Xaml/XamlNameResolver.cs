//
// Copyright (C) 2011 Novell Inc. http://novell.com
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
using System.Threading.Tasks;

namespace System.Xaml
{
	internal class XamlNameResolver : IXamlNameResolver, IXamlNameProvider
	{
		public XamlNameResolver()
		{
		}

		public bool IsCollectingReferences { get; set; }

		internal class NamedObject
		{
			public NamedObject(string name, object value, bool fullyInitialized)
			{
				Name = name;
				Value = value;
				FullyInitialized = fullyInitialized;
			}
			public string Name { get; set; }
			public object Value { get; private set; }
			public bool FullyInitialized { get; private set; }
		}

		Dictionary<string, NamedObject> objects = new Dictionary<string, NamedObject>();
		List<NamedObject> unnamed = new List<NamedObject>();
		List<object> referenced = new List<object>();

		public bool IsFixupTokenAvailable
		{
			get { throw new NotImplementedException(); }
		}

		public event EventHandler OnNameScopeInitializationComplete;

		internal void NameScopeInitializationCompleted(object sender)
		{
			if (OnNameScopeInitializationComplete != null)
				OnNameScopeInitializationComplete(sender, EventArgs.Empty);
			objects.Clear();
			unnamed.Clear();
			used_reference_ids = 0;
		}

		int saved_count, saved_referenced_count, saved_unnamed, saved_used_reference_ids;
		public void Save()
		{
			if (saved_count != 0)
				throw new Exception();
			saved_count = objects.Count;
			saved_referenced_count = referenced.Count;
			saved_unnamed = unnamed.Count;
			saved_used_reference_ids = used_reference_ids;
		}
		public void Restore()
		{
			while (saved_count < objects.Count)
			{
				objects.Remove(objects.Last().Key);
			}
			saved_count = 0;

			if (saved_referenced_count < referenced.Count)
				referenced.RemoveRange(saved_referenced_count, referenced.Count - saved_referenced_count);
			saved_referenced_count = 0;

			if (saved_unnamed < unnamed.Count)
				unnamed.RemoveRange(saved_unnamed, unnamed.Count - saved_unnamed);
			saved_unnamed = 0;

			used_reference_ids = saved_used_reference_ids;
		}

		internal void SetNamedObject(object value, bool fullyInitialized)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			unnamed.Add(new NamedObject(null, value, fullyInitialized));
		}

		public string GetName(object value)
		{
			foreach (var no in objects)
			{
				if (ReferenceEquals(no.Value.Value, value))
					return no.Value.Name;
			}
			return null;
		}

		internal void SaveAsReferenced(object val)
		{
			referenced.Add(val);
		}

		int used_reference_ids;

		internal string GetReferenceName(XamlObject xobj, object value)
		{
			var name = GetName(value);
			if (name != null)
				return name;

			if (unnamed.Count == 0)
				return null;

			NamedObject un = null;
			for (int i = 0; i < unnamed.Count; i++)
			{
				var r = unnamed[i];
				if (ReferenceEquals(r.Value, value))
				{
					un = r;
					break;
				}
			}
			if (un == null)
				return null;

			// generate a name for it, only when needed.
			var xm = xobj.Type.GetAliasedProperty(XamlLanguage.Name);
			if (xm != null)
				name = (string)xm.Invoker.GetValue(xobj.Value);
			else
				name = "__ReferenceID" + used_reference_ids++;
			un.Name = name;
			objects[name] = un;

			return name;
		}

		internal bool Contains(string name)
		{
			return objects.ContainsKey(name);
		}

		internal string GetReferencedName(XamlObject xobj, object value)
		{
			if (!referenced.Contains(value))
				return null;
			return GetReferenceName(xobj, value);
		}

		public object GetFixupToken(IEnumerable<string> names)
		{
			return new NameFixupRequired(names, false);
		}

		public object GetFixupToken(IEnumerable<string> names, bool canAssignDirectly)
		{
			return new NameFixupRequired(names, canAssignDirectly);
		}

		public IEnumerable<KeyValuePair<string, object>> GetAllNamesAndValuesInScope()
		{
			foreach (var pair in objects)
				yield return new KeyValuePair<string, object>(pair.Key, pair.Value.Value);
		}

		public object Resolve(string name)
		{
			NamedObject ret;
			return objects.TryGetValue(name, out ret) ? ret.Value : null;
		}

		public object Resolve(string name, out bool isFullyInitialized)
		{
			NamedObject ret;
			if (objects.TryGetValue(name, out ret))
			{
				isFullyInitialized = ret.FullyInitialized;
				return ret.Value;
			}
			else
			{
				isFullyInitialized = false;
				return null;
			}
		}
	}

	internal class NameFixupRequired
	{
		public NameFixupRequired(IEnumerable<string> names, bool canAssignDirectly)
		{
			CanAssignDirectly = canAssignDirectly;
			Names = names.ToArray();
		}

		public XamlWriterInternalBase.ObjectState ParentState { get; set; }
		public XamlWriterInternalBase.MemberAndValue ParentMemberState { get; set; }
		public XamlWriterInternalBase.ObjectState State { get; set; }
		public XamlWriterInternalBase.MemberAndValue MemberState { get; set; }
		public XamlType Type => State.Type;
		public XamlMember Member => MemberState.Member;
		public object Value => State.Value;
		public object KeyValue => State.KeyValue;

		public int? ListIndex { get; set; }

		public bool CanAssignDirectly { get; set; }
		public IList<string> Names { get; set; }
	}
}
