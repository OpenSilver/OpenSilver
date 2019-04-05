
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
#if !BRIDGE
using System.Runtime.Serialization.Formatters.Binary;
#endif
using System.Text;
using System.Threading.Tasks;

namespace System.IO.IsolatedStorage
{
    internal class IsolatedStorageSettingsForCSharp : IDictionary<string, Object>
    {
        #region Constants/Variables

        private const string Filename = "Settings.bin";
        private static readonly Dictionary<string, object> AppDictionary = new Dictionary<string, object>();

        private static readonly IsolatedStorageSettingsForCSharp StaticIsolatedStorageSettings = new IsolatedStorageSettingsForCSharp();

        //TODO implemente bellow with BRIDGE (seems long)
#if !BRIDGE
        private static readonly IFormatter Formatter = new BinaryFormatter();
#endif

        #endregion

        #region Singleton Implementation

#if !BRIDGE
        /// <summary>
        ///     Its static constructor.
        /// </summary>
        static IsolatedStorageSettingsForCSharp()
        {
            LoadData();
        }
#endif

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

        //TODO : verify we don't need the method below using Bridge
#if !BRIDGE
        // public acces´s for tests
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
#endif

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
#if !BRIDGE
                Save();
#endif
            }
        }

        /// <summary>
        ///     It serializes dictionary in binary format and stores it in a binary file.
        /// </summary>
        public void Save()
        {
            //BRIDGETODO : implemente the code below
#if !BRIDGE
            // IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
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
#endif
        }

        #endregion

        #region IDictionary<string, object> Members

        public void Add(string key, object value)
        {
            AppDictionary.Add(key, value);
#if !BRIDGE
            Save();
#endif
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
#if !BRIDGE
                Save();
#endif
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
#if BRIDGE
                return INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(AppDictionary).ToList();
#else
                return AppDictionary.Values;
#endif
            }
        }

        public object this[string key]
        {
            get { return AppDictionary[key]; }
            set
            {
                AppDictionary[key] = value;
#if !BRIDGE
                Save();
#endif
            }
        }


        public void Add(KeyValuePair<string, object> item)
        {
            AppDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            AppDictionary.Clear();
#if !BRIDGE
            Save();
#endif
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
