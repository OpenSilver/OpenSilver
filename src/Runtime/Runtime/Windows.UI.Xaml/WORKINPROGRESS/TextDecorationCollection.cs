

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
{
    [SupportsDirectContentViaTypeFromStringConverters]
    public sealed partial class TextDecorationCollection
    {
        static TextDecorationCollection()
        {
            TypeFromStringConverters.RegisterConverter(typeof(TextDecorationCollection), INTERNAL_ConvertFromString);
        }

        internal TextDecorationCollection(TextDecoration textDecoration)
        {
            if (textDecoration == null)
            {
                textDecoration = new TextDecoration(0);
            }
            this.Decoration = textDecoration;
        }

        internal TextDecoration Decoration { get; private set; }

        internal static object INTERNAL_ConvertFromString(string textDecorationAsString)
        {
            switch ((textDecorationAsString ?? string.Empty).ToLower())
            {
                case "underline":
                    return TextDecorations.Underline;
                case "strikethrough":
                    return TextDecorations.Strikethrough;
                case "overline":
                    return TextDecorations.OverLine;
                default:
                    return TextDecorations.None;
            }
        }

        public override string ToString()
        {
            switch (this.Decoration.Decoration)
            {
                //case 1:
                    //return "Baseline";
                case 2:
                    return "Overline";
                case 3:
                    return "Strikethrough";
                case 4:
                    return "Underline";
                default:
                    return "None";
            }
        }

        public override bool Equals(object o)
        {
            if (o is TextDecorationCollection)
            {
                TextDecorationCollection tdc = (TextDecorationCollection)o;
                return TextDecoration.Equals(this.Decoration, tdc.Decoration);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Decoration.GetHashCode();
        }

        public static bool operator ==(TextDecorationCollection left, TextDecorationCollection right)
        {
            return TextDecoration.Equals(left?.Decoration, right?.Decoration);
        }

        public static bool operator !=(TextDecorationCollection left, TextDecorationCollection right)
        {
            return !TextDecoration.Equals(left?.Decoration, right?.Decoration);
        }
    }
#if no
    [SupportsDirectContentViaTypeFromStringConverters]
    public sealed partial class TextDecorationCollection : Animatable, IList, IList<TextDecoration>
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
#endif
