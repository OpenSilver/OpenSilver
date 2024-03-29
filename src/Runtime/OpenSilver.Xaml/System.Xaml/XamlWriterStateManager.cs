﻿//
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
using System.Threading.Tasks;

namespace System.Xaml
{
	internal class XamlWriterStateManager<TError, TNSError> : XamlWriterStateManager
			where TError : Exception
			where TNSError : Exception
	{
		public XamlWriterStateManager(bool isXmlWriter)
			: base(isXmlWriter)
		{
		}

		public override Exception CreateError(string msg)
		{
			return (Exception)Activator.CreateInstance(typeof(TError), new object[] { msg });
		}

		public override Exception CreateNamespaceError(string msg)
		{
			return (Exception)Activator.CreateInstance(typeof(TNSError), new object[] { msg });
		}
	}

	internal enum XamlWriteState
	{
		Initial,
		ObjectStarted,
		MemberStarted,
		ObjectWritten,
		ValueWritten,
		MemberDone,
		End
	}

	internal abstract class XamlWriterStateManager
	{
		public XamlWriterStateManager(bool isXmlWriter)
		{
			allow_ns_at_value = isXmlWriter;
			allow_object_after_value = isXmlWriter;
			allow_parallel_values = !isXmlWriter;
			allow_empty_member = !isXmlWriter;
			allow_multiple_results = !isXmlWriter;
			allow_ns_before_endmember = !isXmlWriter;
		}

		// configuration
		bool allow_ns_at_value;
		bool allow_object_after_value;
		bool allow_parallel_values;
		bool allow_empty_member;
		bool allow_multiple_results;
		bool allow_ns_before_endmember;

		// state
		XamlWriteState state = XamlWriteState.Initial;
		bool ns_pushed;
		Stack<XamlType> frames = new Stack<XamlType>();
		bool accept_multiple_values; // It is PositionalParameters-specific state.

		public XamlWriteState State
		{
			get { return state; }
		}

		// FIXME: actually this property is a hack. It should preserve stacked flag values for each nested member in current tree state.
		public bool AcceptMultipleValues
		{
			get { return accept_multiple_values; }
			set { accept_multiple_values = value; }
		}

		public void OnClosingItem()
		{
			// somewhat hacky state change to not reject StartMember->EndMember.
			if (state == XamlWriteState.MemberStarted)
				state = XamlWriteState.ValueWritten;
		}

		public void EndMember()
		{
			if (!allow_ns_before_endmember)
				RejectNamespaces(XamlNodeType.EndMember);
			else
				ns_pushed = false;
			CheckState(XamlNodeType.EndMember);
			state = XamlWriteState.MemberDone;
		}

		public void EndObject(bool hasMoreNodes)
		{
			RejectNamespaces(XamlNodeType.EndObject);
			CheckState(XamlNodeType.EndObject);
			state = hasMoreNodes ? XamlWriteState.ObjectWritten : allow_multiple_results ? XamlWriteState.Initial : XamlWriteState.End;
			frames.Pop();
		}

		public void GetObject()
		{
			CheckState(XamlNodeType.GetObject);
			RejectNamespaces(XamlNodeType.GetObject);
			state = XamlWriteState.MemberDone;
		}

		public void GotObject(XamlType xt)
		{
			frames.Push(xt);
		}

		public void StartMember()
		{
			CheckState(XamlNodeType.StartMember);
			state = XamlWriteState.MemberStarted;
			ns_pushed = false;
		}

		public void StartObject(XamlType xt)
		{
			CheckState(XamlNodeType.StartObject);
			state = XamlWriteState.ObjectStarted;
			ns_pushed = false;
			frames.Push(xt);
		}

		public void Value()
		{
			CheckState(XamlNodeType.Value);
			RejectNamespaces(XamlNodeType.Value);
			state = XamlWriteState.ValueWritten;
		}

		public void Namespace()
		{
			if (!allow_ns_at_value && (state == XamlWriteState.ValueWritten || state == XamlWriteState.ObjectStarted))
				throw CreateError(String.Format("Namespace declarations cannot be written at {0} state", state));
			ns_pushed = true;
		}

		public void NamespaceCleanedUp()
		{
			ns_pushed = false;
		}

		void CheckState(XamlNodeType next)
		{
			switch (state)
			{
				case XamlWriteState.Initial:
					switch (next)
					{
						case XamlNodeType.StartObject:
							return;
					}
					break;
				case XamlWriteState.ObjectStarted:
					switch (next)
					{
						case XamlNodeType.StartMember:
						case XamlNodeType.EndObject:
							return;
					}
					break;
				case XamlWriteState.MemberStarted:
					switch (next)
					{
						case XamlNodeType.StartObject:
						case XamlNodeType.Value:
						case XamlNodeType.GetObject:
							return;
						case XamlNodeType.EndMember:
							if (allow_empty_member)
								return;
							break;
					}
					break;
				case XamlWriteState.ObjectWritten:
					switch (next)
					{
						case XamlNodeType.StartObject:
						case XamlNodeType.Value:
						case XamlNodeType.EndMember:
							return;
					}
					break;
				case XamlWriteState.ValueWritten:
					switch (next)
					{
						case XamlNodeType.Value:
							if (allow_parallel_values | accept_multiple_values)
								return;
							break;
						case XamlNodeType.StartObject:
							if (allow_object_after_value || (frames.Count > 0 && frames.Peek().IsCollection))
								return;
							break;
						case XamlNodeType.EndMember:
							return;
					}
					break;
				case XamlWriteState.MemberDone:
					switch (next)
					{
						case XamlNodeType.StartMember:
						case XamlNodeType.EndObject:
							return;
					}
					break;
			}
			throw CreateError(String.Format("{0} is not allowed at current state {1}", next, state));
		}

		void RejectNamespaces(XamlNodeType next)
		{
			if (ns_pushed)
			{
				// strange, but on WriteEndMember it throws XamlXmlWriterException, while for other nodes it throws IOE.
				string msg = String.Format("Namespace declarations cannot be written before {0}", next);
				if (next == XamlNodeType.EndMember)
					throw CreateError(msg);
				else
					throw CreateNamespaceError(msg);
			}
		}

		public abstract Exception CreateError(string msg);
		public abstract Exception CreateNamespaceError(string msg);
	}
}
