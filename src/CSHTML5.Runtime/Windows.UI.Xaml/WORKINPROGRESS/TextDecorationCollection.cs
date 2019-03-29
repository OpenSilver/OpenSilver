
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;
#if MIGRATION
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    [SupportsDirectContentViaTypeFromStringConverters]
    public sealed class TextDecorationCollection : Animatable, IList, IList<TextDecoration>
    {

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(TextDecoration item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, TextDecoration item)
        {
            throw new NotImplementedException();
        }

        TextDecoration IList<TextDecoration>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(TextDecoration item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(TextDecoration item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TextDecoration[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TextDecoration item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<TextDecoration> IEnumerable<TextDecoration>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        static TextDecorationCollection()
        {
            TypeFromStringConverters.RegisterConverter(typeof(TextDecorationCollection), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string textDecorationCollectionAsString)
        {
            return new TextDecorationCollection();
        }
    }
#endif
}
