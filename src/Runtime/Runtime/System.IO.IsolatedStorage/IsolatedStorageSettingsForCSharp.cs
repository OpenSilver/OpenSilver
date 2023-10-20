
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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.IO.IsolatedStorage
{
    internal sealed class IsolatedStorageSettingsForCSharp : IDictionary<string, object>
    {
        #region Constants/Variables

        private const string Filename = "Settings.bin";
        private static readonly Dictionary<string, object> AppDictionary = new Dictionary<string, object>();

        private static readonly IsolatedStorageSettingsForCSharp StaticIsolatedStorageSettings = new IsolatedStorageSettingsForCSharp();

        private static readonly IFormatter Formatter = new BinaryFormatter();

        #endregion

        #region Singleton Implementation

        /// <summary>
        ///     Its static constructor.
        /// </summary>
        static IsolatedStorageSettingsForCSharp()
        {
            LoadData();
        }

        /// <summary>
        ///     Its a private constructor.
        /// </summary>
        private IsolatedStorageSettingsForCSharp()
        {
        }

        /// <summary>
        ///     Its a static singleton instance.
        /// </summary>
        public static IsolatedStorageSettingsForCSharp Instance
        {
            get { return StaticIsolatedStorageSettings; }
        }

        public static void LoadData()
        {
            // IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForAssembly();
            if (isoStore.GetFileNames(Filename).Length == 0)
            {
                // File not exists. Let us NOT try to DeSerialize it.        
                return;
            }

            // Read the stream from Isolated Storage.    
            Stream stream = new IsolatedStorageFileStream(Filename, FileMode.OpenOrCreate, isoStore);
            try
            {
                // DeSerialize the Dictionary from stream.    
                object bytes = Formatter.Deserialize(stream);

                var appData = (Dictionary<string, object>) bytes;

                // Enumerate through the collection and load our Dictionary.            
                IDictionaryEnumerator enumerator = appData.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AppDictionary[enumerator.Key.ToString()] = enumerator.Value;
                }
            }
            finally
            {
                stream.Close();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     It Checks if Dictionary object has item corresponding to passed key,
        ///     if True then it returns that object else it returns default value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue"></param>
        /// <returns></returns>
        public object this[string key, Object defaultvalue]
        {
            get
            {
                return AppDictionary.ContainsKey(key)
                           ? AppDictionary[key]
                           : defaultvalue;
            }
            set
            {
                AppDictionary[key] = value;
                Save();
            }
        }

        /// <summary>
        ///     It serializes dictionary in binary format and stores it in a binary file.
        /// </summary>
        public void Save()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForAssembly();

            Stream stream = new IsolatedStorageFileStream(Filename, FileMode.Create, isoStore);
            try
            {
                // Serialize dictionary into the IsolatedStorage.                                
                Formatter.Serialize(stream, AppDictionary);
            }
            finally
            {
                stream.Close();
            }      
        }

        #endregion

        #region IDictionary<string, object> Members

        public void Add(string key, object value)
        {
            AppDictionary.Add(key, value);
            Save();
        }

        public bool ContainsKey(string key)
        {
            return AppDictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return AppDictionary.Keys; }
        }

        public bool Remove(string key)
        {
            try
            {
                Save();
                AppDictionary.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryGetValue(string key, out object value)
        {
            return AppDictionary.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get
            {
                return AppDictionary.Values;
            }
        }

        public object this[string key]
        {
            get { return AppDictionary[key]; }
            set
            {
                AppDictionary[key] = value;
                Save();
            }
        }


        public void Add(KeyValuePair<string, object> item)
        {
            AppDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            AppDictionary.Clear();
            Save();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return AppDictionary.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return AppDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return AppDictionary.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return AppDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AppDictionary.GetEnumerator();
        }

        #endregion
    }
}
