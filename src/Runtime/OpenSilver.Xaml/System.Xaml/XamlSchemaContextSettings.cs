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

namespace System.Xaml
{
	/// <summary>
	/// Options for member and type invokers.
	/// </summary>
	[EnhancedXaml, Flags]
	public enum XamlInvokerOptions
	{
		/// <summary>
		/// Use reflection, which is much slower than compiled code on each call.
		/// </summary>
		None = 0,

		/// <summary>
		/// Compile an expression tree to get/set/add values, which has higher startup cost but is an 
		/// order of magnatude faster than reflection.
		/// </summary>
		Compile = 1,

		/// <summary>
		/// Use reflection while the expression tree is compiled in a background thread.
		/// </summary>
		/// <remarks>
		/// This (might) be the best of both worlds, where it uses reflection initially, but will compile the expression tree
		/// in a background thread and use it when it is ready.
		/// </remarks>
		DeferCompile = 3
	}

	public class XamlSchemaContextSettings
	{
		public XamlSchemaContextSettings()
		{
		}

		public XamlSchemaContextSettings(XamlSchemaContextSettings settings)
		{
			// null is allowed.
			var s = settings;
			if (s == null)
				return;
			FullyQualifyAssemblyNamesInClrNamespaces = s.FullyQualifyAssemblyNamesInClrNamespaces;
			SupportMarkupExtensionsWithDuplicateArity = s.SupportMarkupExtensionsWithDuplicateArity;
			InvokerOptions = s.InvokerOptions;
		}

		public bool FullyQualifyAssemblyNamesInClrNamespaces { get; set; }

		public bool SupportMarkupExtensionsWithDuplicateArity { get; set; }

		[EnhancedXaml]
		public XamlInvokerOptions InvokerOptions { get; set; } = XamlInvokerOptions.DeferCompile;

	}
}
