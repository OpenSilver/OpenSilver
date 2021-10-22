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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Xaml.Schema
{
	public class XamlValueConverter<TConverterBase> : IEquatable<XamlValueConverter<TConverterBase>>
		where TConverterBase : class
	{
		public XamlValueConverter(Type converterType, XamlType targetType)
			: this(converterType, targetType, null)
		{
		}

		public XamlValueConverter(Type converterType, XamlType targetType, string name)
		{
			if (converterType == null && targetType == null && name == null)
				throw new ArgumentException("Either of converterType, targetType or name must be non-null");
			ConverterType = converterType;
			TargetType = targetType;
			Name = name;
		}

		TConverterBase converter_instance;
		public TConverterBase ConverterInstance
		{
			get
			{
				if (converter_instance == null)
					converter_instance = CreateInstance();
				return converter_instance;
			}
			internal set
			{
				converter_instance = value;
			}
		}
		public Type ConverterType { get; private set; }
		public string Name { get; private set; }
		public XamlType TargetType { get; private set; }


		public static bool operator ==(XamlValueConverter<TConverterBase> left, XamlValueConverter<TConverterBase> right)
		{
			return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.Equals(right);
		}

		public static bool operator !=(XamlValueConverter<TConverterBase> left, XamlValueConverter<TConverterBase> right)
		{
			return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : ReferenceEquals(right, null) || left.ConverterType != right.ConverterType || left.TargetType != right.TargetType || left.Name != right.Name;
		}

		public bool Equals(XamlValueConverter<TConverterBase> other)
		{
			return !ReferenceEquals(other, null) && ConverterType == other.ConverterType && TargetType == other.TargetType && Name == other.Name;
		}

		public override bool Equals(object obj)
		{
			var a = obj as XamlValueConverter<TConverterBase>;
			return Equals(a);
		}

		protected virtual TConverterBase CreateInstance()
		{
			if (ConverterType == null)
				return null;

			if (!typeof(TConverterBase).GetTypeInfo().IsAssignableFrom(ConverterType.GetTypeInfo()))
				throw new XamlSchemaException(String.Format("ConverterType '{0}' is not derived from '{1}' type", ConverterType, typeof(TConverterBase)));

			if (ConverterType.GetTypeInfo().GetConstructors().Any(r => r.GetParameters().Length == 0))
				return (TConverterBase)Activator.CreateInstance(ConverterType);

			if (ConverterType.GetTypeInfo().GetConstructors().Any(r => r.GetParameters().Length == 1 && r.GetParameters()[0].ParameterType == typeof(Type)))
				return (TConverterBase)Activator.CreateInstance(ConverterType, TargetType.UnderlyingType);
			return null;
		}

		public override int GetHashCode()
		{
			var ret = ConverterType != null ? ConverterType.GetHashCode() : 0;
			ret <<= 5;
			if (TargetType != null)
				ret += TargetType.GetHashCode();
			ret <<= 5;
			if (Name != null)
				ret += Name.GetHashCode();
			return ret;
		}

		public override string ToString()
		{
			if (Name != null)
				return Name;
			if (ConverterType != null && TargetType != null)
				return String.Concat(ConverterType.Name, "(", TargetType.Name, ")");
			else if (ConverterType != null)
				return ConverterType.Name;
			else
				return TargetType.Name;
		}
	}
}
