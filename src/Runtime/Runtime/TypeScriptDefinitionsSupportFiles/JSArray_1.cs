

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


using CSHTML5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeScriptDefinitionsSupport
{
    public class JSArray<T> : JSObject, IList<T>
    {
        public int Count
        {
            get
            {
                return Convert.ToInt32(OpenSilver.Interop.ExecuteJavaScript("$0.length", this.UnderlyingJSInstance));
            }
        }

        public bool IsReadOnly { get { return false; } }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException();

                object value = OpenSilver.Interop.ExecuteJavaScript("$0[$1]", this.UnderlyingJSInstance, index);
                if (typeof(IJSObject).IsAssignableFrom(typeof(T)))
                {
                    IJSObject o = (IJSObject)(object)Activator.CreateInstance<T>();
                    o.UnderlyingJSInstance = value; 
                    return (T)(object)o;
                }
                else
                    return (T)value;
                // ----------------
                // Disabled this portion of code because it doesn't work well once translated into js,
                // instead the value is directly cast and return, will throw anyway if it's not compatible
                // ----------------
                //
                //else if (JSObject.Helper_IsBuiltInType<T>())
                //    return JSObject.Helper_ConvertTo<T>(value);
                //else
                //    throw new Exception("The generic type parameter '" + typeof(T).Name + "' is not a built-in type or a JSObject.");
            }
            set
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException();

                object _value;

                if (typeof(IJSObject).IsAssignableFrom(typeof(T)))
                    _value = ((IJSObject)(object)value).UnderlyingJSInstance;
                else
                    _value = value;
                // ----------------
                // Disabled this portion of code because it doesn't work well once translated into js,
                // instead the value is directly cast and return, will throw anyway if it's not compatible
                // ----------------
                //else if (JSObject.Helper_IsBuiltInType<T>())
                //    _value = value;
                //else
                //    throw new Exception("The generic type parameter '" + typeof(T).Name + "' is not a built-in type or a JSObject.");
                OpenSilver.Interop.ExecuteJavaScriptAsync("$0[$1] = $2", this.UnderlyingJSInstance, index, _value);
            }
        }

        public void Add(T item)
        {
            object value;

            if (item == null)
                value = null;
            else if (typeof(IJSObject).IsAssignableFrom(typeof(T)))
                value = ((IJSObject)(object)item).UnderlyingJSInstance;
            else
                value = item;
            // ----------------
            // Disabled this portion of code because it doesn't work well once translated into js,
            // instead the value is directly cast and return, will throw anyway if it's not compatible
            // ----------------
            //else if (JSObject.Helper_IsBuiltInType<T>())
            //    value = item;
            //else
            //    throw new Exception("The generic type parameter '" + typeof(T).Name + "' is not a built-in type or a JSObject.");

            OpenSilver.Interop.ExecuteJavaScriptAsync("$0.push($1)", this.UnderlyingJSInstance, value);
        }

        public void Clear()
        {
            OpenSilver.Interop.ExecuteJavaScriptAsync("$0.splice(0, $0.length)", this.UnderlyingJSInstance);
        }

        public bool Contains(T item)
        {
            object value;

            if (item == null)
                value = null;
            else if (typeof(IJSObject).IsAssignableFrom(typeof(T)))
                value = ((IJSObject)(object)item).UnderlyingJSInstance;
            else
                value = item;
            // ----------------
            // Disabled this portion of code because it doesn't work well once translated into js,
            // instead the value is directly cast and return, will throw anyway if it's not compatible
            // ----------------
            //else if (JSObject.Helper_IsBuiltInType<T>())
            //    value = item;
            //else
            //    throw new Exception("The generic type parameter '" + typeof(T).Name + "' is not a built-in type or a JSObject.");

            int index = Convert.ToInt32(OpenSilver.Interop.ExecuteJavaScript("$0.indexOf($1)", this.UnderlyingJSInstance, value));

            // If the element is not found by indexOf, -1 is returned
            return index != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            if (this.Count > array.Length - arrayIndex)
                throw new ArgumentException();

            for (int i = 0; i < this.Count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (typeof(IJSObject).IsAssignableFrom(typeof(T)))
            {
                for (int i = 0; i < this.Count; i++)
                {
                    object value = OpenSilver.Interop.ExecuteJavaScript("$0[$1]", this.UnderlyingJSInstance, i);
                    IJSObject o = (IJSObject)(object)Activator.CreateInstance<T>();
                    o.UnderlyingJSInstance = value;
                    yield return (T)(object)o;
                }
            }
            else
            //else if (JSObject.Helper_IsBuiltInType<T>())
            {
                for (int i = 0; i < this.Count; i++)
                {
                    object value = OpenSilver.Interop.ExecuteJavaScript("$0[$1]", this.UnderlyingJSInstance, i);
                    yield return JSObject.Helper_ConvertTo<T>(value);
                }
            }
            // ----------------
            // Disabled this portion of code because it doesn't work well once translated into js,
            // instead the value is directly cast and return, will throw anyway if it's not compatible
            // ----------------
            //else
            //    throw new Exception("The generic type parameter '" + typeof(T).Name + "' is not a built-in type or a JSObject.");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            object value;

            if (item == null)
                value = null;
            else if (typeof(IJSObject).IsAssignableFrom(typeof(T)))
                value = ((IJSObject)(object)item).UnderlyingJSInstance;
            else
                value = item;
            // ----------------
            // Disabled this portion of code because it doesn't work well once translated into js,
            // instead the value is directly cast and return, will throw anyway if it's not compatible
            // ----------------
            //else if (JSObject.Helper_IsBuiltInType<T>())
            //    value = item;
            //else
            //    throw new Exception("The generic type parameter '" + typeof(T).Name + "' is not a built-in type or a JSObject.");

            return Convert.ToInt32(OpenSilver.Interop.ExecuteJavaScript("$0.indexOf($1)", this.UnderlyingJSInstance, value));
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > this.Count)
                throw new ArgumentOutOfRangeException();

            if(index == this.Count)
            {
                this.Add(item);
                return;
            }

            object value;

            if (item == null)
                value = null;
            else if (typeof(IJSObject).IsAssignableFrom(typeof(T)))
                value = ((IJSObject)(object)item).UnderlyingJSInstance;
            else
                value = item;

            // ----------------
            // Disabled this portion of code because it doesn't work well once translated into js,
            // instead the value is directly cast and return, will throw anyway if it's not compatible
            // ----------------
            //else if (JSObject.Helper_IsBuiltInType<T>())
            //    value = item;
            //else
            //    throw new Exception("The generic type parameter '" + typeof(T).Name + "' is not a built-in type or a JSObject.");

            OpenSilver.Interop.ExecuteJavaScriptAsync("$0.splice($1, 0, $2)", this.UnderlyingJSInstance, index, value);
        }

        public bool Remove(T item)
        {
            object value;

            if (item == null)
                value = null;
            else if (typeof(IJSObject).IsAssignableFrom(typeof(T)))
                value = ((IJSObject)(object)item).UnderlyingJSInstance;
            else
                value = item;
            // ----------------
            // Disabled this portion of code because it doesn't work well once translated into js,
            // instead the value is directly cast and return, will throw anyway if it's not compatible
            // ----------------
            //else if (JSObject.Helper_IsBuiltInType<T>())
            //    value = item;
            //else
            //    throw new Exception("The generic type parameter '" + typeof(T).Name + "' is not a built-in type or a JSObject.");

            int index = Convert.ToInt32(OpenSilver.Interop.ExecuteJavaScript("$0.indexOf($1)", this.UnderlyingJSInstance, value));

            if (index != -1)
                OpenSilver.Interop.ExecuteJavaScriptAsync("$0.splice($1, 1)", this.UnderlyingJSInstance, index);

            return index != -1;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
                throw new ArgumentOutOfRangeException();

            OpenSilver.Interop.ExecuteJavaScriptAsync("$0.splice($1, 1)", this.UnderlyingJSInstance, index);
        }

        public JSArray()
        {
            this.UnderlyingJSInstance = OpenSilver.Interop.ExecuteJavaScript("[]");
        }

        public JSArray(object jsObj)
        {
            this.UnderlyingJSInstance = jsObj;
        }

        public static JSArray<T> FromJavaScriptInstance(object jsObj)
        {
            return new JSArray<T>(jsObj);
        }


    }
}
