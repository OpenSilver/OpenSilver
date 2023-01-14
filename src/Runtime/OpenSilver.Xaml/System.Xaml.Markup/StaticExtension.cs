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
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Markup;

namespace System.Xaml.Markup
{
	[MarkupExtensionReturnType(typeof(object))]
	[TypeConverter(typeof(StaticExtensionConverter))]
    internal class StaticExtension : MarkupExtension
	{
		public StaticExtension()
		{
		}

		public StaticExtension(string member)
		{
			Member = member;
		}

		[ConstructorArgument("member")]
		public string Member { get; set; }

		[DefaultValue(null)]
		public Type MemberType { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Member == null)
				throw new InvalidOperationException("Member property must be set to StaticExtension before calling ProvideValue method.");

			if (MemberType == null && serviceProvider != null)
			{
				// support [ns:]Type.Member
				var typeResolver = (IXamlTypeResolver)serviceProvider.GetService(typeof(IXamlTypeResolver));
				if (typeResolver != null)
				{
					var memberIndex = Member.IndexOf('.');
					if (memberIndex > 0)
					{
						var typeName = Member.Substring(0, memberIndex);
						MemberType = typeResolver.Resolve(typeName);
						if (MemberType != null)
						{
							Member = Member.Substring(memberIndex + 1);
						}
					}
				}
			}

			if (MemberType != null)
			{
				var pi = MemberType.GetRuntimeProperty(Member);
				if (pi != null && (pi.GetPrivateGetMethod()?.IsStatic ?? false))
					return pi.GetValue(null, null);
				var fi = MemberType.GetRuntimeField(Member);
				if (fi != null && fi.IsStatic)
					return fi.GetValue(null);
			}

			throw new ArgumentException(String.Format("Member '{0}' could not be resolved to a static member", Member));
		}
	}
}
