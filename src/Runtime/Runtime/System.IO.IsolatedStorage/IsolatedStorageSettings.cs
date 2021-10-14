﻿

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
#if !BRIDGE
//BRIDGETODO:
//Apparently, using below is useless
using System.Runtime.Serialization.Formatters.Binary;
using JSIL.Meta;
#else
using Bridge;
#endif
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif
#if OPENSILVER
using OpenSilver;
#else
using CSHTML5;
#endif

namespace System.IO.IsolatedStorage
{
    //Note: we remove the interfaces because they are useless for now.
    //
    // Exceptions:
    //   System.ArgumentNullException:
    //     key is null. This exception is thrown when you attempt to reference an instance
    //     of the class by using an indexer and the variable you pass in for the key
    //     value is null.

    /// <summary>
    /// Provides a System.Collections.Generic.Dictionary&lt;TKey,TValue&gt; that stores
    /// key-value pairs in isolated storage.
    /// </summary>
    /// <example>
    /// Here is how to use the IsolatedStorageSettings:
    /// <code lang="C#">
    /// //Write in the IsolatedStorageSettings:
    /// IsolatedStorageSettings.ApplicationSettings["someKey"] = "someValue";
    /// //Read from it:
    /// string value;
    /// string myString = IsolatedStorageSettings.ApplicationSettings.TryGetValue("someKey", out value);
    /// </code>
    /// </example>
    public sealed partial class IsolatedStorageSettings : IEnumerable, IEnumerable<KeyValuePair<string, object>> // : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IDictionary, ICollection
    {
        string _fullApplicationName = null;

#if !BRIDGE
        //[JSReplacement("true")]
#else
        [Template("true")]
#endif
        static bool IsRunningInJavascript() //must be static to work properly
        {
            return !Interop.IsRunningInTheSimulator;
        }

#if !BRIDGE
        [JSReplacement("undefined")]
#else
        [Template("undefined")]
#endif
        static dynamic GetUndefined() { return null; } //must be static to work properly

        //[JSIL.Meta.JSReplacement("window.localStorage")]
        object GetLocalStorage()
        {
#if !OPENSILVER
            if (IsRunningInJavascript())
            {
#if !BRIDGE
                dynamic localStorage = JSIL.Verbatim.Expression(@"
function(){
    return window.localStorage;
}()
");
#else
                object localStorage = Script.Write<object>(@"
(function(){
    return window.localStorage;
}());
");
#endif
                if (localStorage == null || localStorage == GetUndefined())
                {
#if !BRIDGE
                    JSIL.Verbatim.Expression(@"
if(window.IE_VERSION && document.location.protocol === ""file:"") {
    JSIL.RuntimeError(""The local storage - used to persist data - is not available on Internet Explorer or Edge when running the website from the local file system (ie. the URL starts with 'c:\' or 'file:///'). To solve the problem, please run the website from a web server instead (ie. the URL must start with 'http://' or 'https://') or test the local storage using a different browser."")
}");
                    return null;
#else
                    throw new Exception("The local storage - used to persist data - is not available on Internet Explorer or Edge when running the website from the local file system (ie. the URL starts with 'c:\' or 'file:///'). To solve the problem, please run the website from a web server instead (ie. the URL must start with 'http://' or 'https://') or test the local storage using a different browser.");
#endif
                }
                else
                {
                    return localStorage;
                }
            }
            else
            {
#endif
            //Note: The whole part in the #if !OPENSILVER above only serves to throw an exception when in IE or Edge if we are on a local file system (ie url starts with c:\ or similar). It should be useless in OpenSilver and can most definitely be simplified in Bridge.
            return Interop.ExecuteJavaScript("window.localStorage");
#if !OPENSILVER
            }
#endif
        }


        private int GetLength()
        {
            object localStorage = GetLocalStorage();
            return Convert.ToInt32(Interop.ExecuteJavaScript("$0.length", localStorage));
        }

        private string GetKey(int index)
        {
            object localStorage = GetLocalStorage();
            return Convert.ToString(Interop.ExecuteJavaScript("$0.key($1)", localStorage, index));
        }

        string GetKeysFirstPart()
        {
            return "storage_" + _fullApplicationName + "_settings_";
        }

        IsolatedStorageSettings()
        {
            _fullApplicationName = Application.Current.ToString();
        }

        static IsolatedStorageSettings _applicationSettings = null;
        /// <summary>
        /// Gets an instance of System.IO.IsolatedStorage.IsolatedStorageSettings that
        /// contains the contents of the application's System.IO.IsolatedStorage.IsolatedStorageFile,
        /// scoped at the application level, or creates a new instance of System.IO.IsolatedStorage.IsolatedStorageSettings
        /// if one does not exist.
        /// </summary>
        public static IsolatedStorageSettings ApplicationSettings
        {
            get
            {
                if (_applicationSettings == null)
                {
                    _applicationSettings = new IsolatedStorageSettings();
                }
                return _applicationSettings;
            }
        }



        /// <summary>
        /// Gets the number of key-value pairs that are stored in the dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                if (IsRunningInJavascript())
                {
                    object localStorage = GetLocalStorage();
                    int length = GetLength();

                    int count = 0;
                    for (int i = 0; i < length; ++i)
                    {
                        string temp = GetKey(i);
                        if (temp.StartsWith(GetKeysFirstPart()))
                        {
                            ++count;
                        }
                    }
                    return count;
                }
                else
                {
                    return IsolatedStorageSettingsForCSharp.Instance.Count;
                }
            }
        }




        /// <summary>
        /// Gets a collection that contains the keys in the dictionary.
        /// </summary>
        public ICollection Keys
        {
            get
            {
                if (IsRunningInJavascript())
                {
                    object localStorage = GetLocalStorage();
                    List<string> keysList = new List<string>();
                    int length = GetLength();

                    int lengthOfPartToRemoveFromKey = (GetKeysFirstPart()).Length;
                    for (int i = 0; i < length; ++i)
                    {
                        string temp = GetKey(i);

                        if (temp.StartsWith(GetKeysFirstPart()))
                        {
                            keysList.Add(temp.Substring(lengthOfPartToRemoveFromKey));
                        }
                    }
                    return keysList;
                }
                else
                {
                    return IsolatedStorageSettingsForCSharp.Instance.Keys.ToList<string>();
                }
            }
        }

        ////
        //// Summary:
        ////     Gets an instance of System.IO.IsolatedStorage.IsolatedStorageSettings that
        ////     contains the contents of the application's System.IO.IsolatedStorage.IsolatedStorageFile,
        ////     scoped at the domain level, or creates a new instance of System.IO.IsolatedStorage.IsolatedStorageSettings
        ////     if one does not exist.
        ////
        //// Returns:
        ////     An System.IO.IsolatedStorage.IsolatedStorageSettings object that contains
        ////     the contents of the application's System.IO.IsolatedStorage.IsolatedStorageFile,
        ////     scoped at the domain level. If an instance does not already exist, a new
        ////     instance is created.
        //public static IsolatedStorageSettings SiteSettings { get; }

        /// <summary>
        /// Gets a collection that contains the values in the dictionary.
        /// </summary>
        public ICollection Values
        {
            get
            {
                if (IsRunningInJavascript())
                {
                    List<object> valuesList = new List<object>();
                    int length = GetLength();
                    for (int i = 0; i < length; ++i)
                    {
                        string temp = GetKey(i);
                        if (temp.StartsWith(GetKeysFirstPart()))
                        {
                            valuesList.Add(this[temp].ToString());
                        }
                    }
                    return valuesList;
                }
                else
                {
#if BRIDGE
                    return (ICollection)(INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible<string, Object>(IsolatedStorageSettingsForCSharp.Instance).ToList<object>());
#else
                    return (ICollection)IsolatedStorageSettingsForCSharp.Instance.Values.ToList<object>();
#endif

                }
            }
        }
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the item to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not
        /// found, a get operation throws a System.Collections.Generic.KeyNotFoundException,
        /// and a set operation creates a new element that has the specified key.
        /// </returns>
        public object this[string key]
        {
            get
            {
                if (!Interop.IsRunningInTheSimulator)
                {
                    object localStorage = GetLocalStorage();
                    return Interop.ExecuteJavaScript("$0[$1]", localStorage, GetKeysFirstPart() + key);
                }
                else
                {
                    return IsolatedStorageSettingsForCSharp.Instance[key];
                }
            }
            set
            {
                if (!Interop.IsRunningInTheSimulator)
                {
                    object localStorage = GetLocalStorage();
                    string applicationSpecificKey = GetKeysFirstPart() + key;
                    Interop.ExecuteJavaScript("$0[$1] = $2", localStorage, GetKeysFirstPart() + key, value);
                }
                else
                {
                    IsolatedStorageSettingsForCSharp.Instance[key] = value;
                }
            }
        }

        /// <summary>
        /// Adds an entry to the dictionary for the key-value pair.
        /// </summary>
        /// <param name="key">The key for the entry to be stored.</param>
        /// <param name="value">The value to be stored.</param>
        public void Add(string key, object value)
        {
            if (IsRunningInJavascript())
            {
                object localStorage = GetLocalStorage();
                Interop.ExecuteJavaScript("$0[$1] = $2", localStorage, GetKeysFirstPart() + key, value);
            }
            else
            {
                IsolatedStorageSettingsForCSharp.Instance.Add(key, value);
                IsolatedStorageSettingsForCSharp.Instance.Save();
            }
        }

        /// <summary>
        /// Resets the count of items stored in System.IO.IsolatedStorage.IsolatedStorageSettings
        /// to zero and releases all references to elements in the collection.
        /// </summary>
        public void Clear()
        {
            if (IsRunningInJavascript())
            {
                object localStorage = GetLocalStorage();
                List<string> keys = (List<string>)Keys;
                foreach (string key in keys)
                {
                    Interop.ExecuteJavaScript("$0.removeItem($1)", localStorage, GetKeysFirstPart() + key);
                }
            }
            else
            {
                IsolatedStorageSettingsForCSharp.Instance.Clear();
            }
        }

        /// <summary>
        /// Determines if the application settings dictionary contains the specified
        /// key.
        /// </summary>
        /// <param name="key">The key for the entry to be located.</param>
        /// <returns>true if the dictionary contains the specified key; otherwise, false.</returns>
        public bool Contains(string key)
        {
            if (IsRunningInJavascript())
            {
                object localStorage = GetLocalStorage();
                var temp = Interop.ExecuteJavaScript("$0.getItem($1)", localStorage, GetKeysFirstPart() + key);

                return (temp != null);
            }
            else
            {
                return IsolatedStorageSettingsForCSharp.Instance.ContainsKey(key);
            }
        }

        /// <summary>
        /// Removes the entry with the specified key.
        /// </summary>
        /// <param name="key">The key for the entry to be deleted.</param>
        /// <returns>true if the specified key was removed; otherwise, false.</returns>
        public bool Remove(string key)
        {
            if (IsRunningInJavascript())
            {
                object localStorage = GetLocalStorage();
                if (Interop.ExecuteJavaScript("$0.getItem($1)", localStorage, GetKeysFirstPart() + key) == null)
                {
                    return false;
                }
                Interop.ExecuteJavaScript("$0.removeItem($1)", localStorage, GetKeysFirstPart() + key);
                return true;
            }
            else
            {
                bool ret = IsolatedStorageSettingsForCSharp.Instance.Remove(key);
                IsolatedStorageSettingsForCSharp.Instance.Save();
                return ret;
            }
        }

        //below is commented because the data is directly saved when changed.
        ///// <summary>
        ///// Saves data written to the current System.IO.IsolatedStorage.IsolatedStorageSettings
        ///// object.
        ///// </summary>
        //public void Save()
        //{
        //    //todo.
        //}


        /// <summary>
        /// Gets a value for the specified key.
        /// </summary>
        /// <typeparam name="T">The System.Type of the value parameter.</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key if
        /// the key is found; otherwise, the default value for the type of the value
        /// parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if the specified key is found; otherwise, false.</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            if (!Interop.IsRunningInTheSimulator)
            {
                object localStorage = GetLocalStorage();
                var temp = Interop.ExecuteJavaScript("$0.getItem($1)", localStorage, GetKeysFirstPart() + key);
                if (temp == null)
                {
                    value = default(T);
                    return false;
                }
                else
                {
                    value = (T)Convert.ChangeType(temp, typeof(T), Globalization.CultureInfo.InvariantCulture);
                    return true;
                }
            }
            else
            {
                object temp;
                bool ret = IsolatedStorageSettingsForCSharp.Instance.TryGetValue(key, out temp);
                value = (T)temp;
                return ret;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (KeyValuePair<string, object> kv in EnumerateKeyValues())
            {
                yield return kv;
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (KeyValuePair<string, object> kv in EnumerateKeyValues())
            {
                yield return kv;
            }
        }

        IEnumerable<KeyValuePair<string, object>> EnumerateKeyValues()
        {
            if (IsRunningInJavascript())
            {
                object localStorage = GetLocalStorage();
                List<string> keys = (List<string>)Keys;
                foreach (string key in keys)
                {
                    object item = Interop.ExecuteJavaScript("$0.getItem($1)", localStorage, GetKeysFirstPart() + key);
                    yield return new KeyValuePair<string, object>(key, item);
                }
            }
            else
            {
                foreach (KeyValuePair<string, object> kv in IsolatedStorageSettingsForCSharp.Instance)
                {
                    yield return kv;
                }
            }
        }

        #region for the interfaces that we remove for now
        //public void Add(KeyValuePair<string, object> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Contains(KeyValuePair<string, object> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsReadOnly
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public bool Remove(KeyValuePair<string, object> item)
        //{
        //    throw new NotImplementedException();
        //}





        //IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Add(object key, object value)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool Contains(object key)
        //{
        //    throw new NotImplementedException();
        //}

        //IDictionaryEnumerator IDictionary.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsFixedSize
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public void Remove(object key)
        //{
        //    throw new NotImplementedException();
        //}

        //public object this[object key]
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //    set
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public void CopyTo(Array array, int index)
        //{
        //    throw new NotImplementedException();
        //}

        //public bool IsSynchronized
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public object SyncRoot
        //{
        //    get { throw new NotImplementedException(); }
        //}


        //public bool ContainsKey(string key)
        //{
        //    throw new NotImplementedException();
        //}

        //ICollection<string> IDictionary<string, object>.Keys
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public bool TryGetValue(string key, out object value)
        //{
        //    throw new NotImplementedException();
        //}

        //ICollection<object> IDictionary<string, object>.Values
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //void ICollection.CopyTo(Array array, int index)
        //{
        //    throw new NotImplementedException();
        //}

        //int ICollection.Count
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //bool ICollection.IsSynchronized
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //object ICollection.SyncRoot
        //{
        //    get { throw new NotImplementedException(); }
        //}
        #endregion
    }
}
