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
using System.Text;

namespace System.Xaml.Markup
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public sealed class ValueSerializerAttribute : Attribute
	{
		private Type _valueSerializerType;
		private string _valueSerializerTypeName;

		public Type ValueSerializerType
		{
			get
			{
				if (_valueSerializerType == null && _valueSerializerTypeName != null)
					_valueSerializerType = Type.GetType(_valueSerializerTypeName);
				return _valueSerializerType;
			}
		}

		public string ValueSerializerTypeName
		{
			get
			{
				if (_valueSerializerType != null)
					return _valueSerializerType.AssemblyQualifiedName;
				else
					return _valueSerializerTypeName;
			}
		}

		public ValueSerializerAttribute(Type valueSerializerType)
		{
			_valueSerializerType = valueSerializerType;
		}

		public ValueSerializerAttribute(string valueSerializerTypeName)
		{
			_valueSerializerTypeName = valueSerializerTypeName;
		}
	}
}
