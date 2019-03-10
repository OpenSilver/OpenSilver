
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


using System;
using System.Collections.Generic;
using System.Security;
using System.Linq;

#if MIGRATION
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    /// <summary>
    /// Provides a basic implementation of the IDataObject interface,
    /// which defines a format-independent mechanism for transferring data.
    /// </summary>
    public sealed class DataObject : IDataObject
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public DataObject()
        {
        }

        /*spublic DataObject(object data) : this()
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            _data.Add(data.GetType().ToString(),data);
        }*/

        /// <summary>
        /// Retrieves a data object in a specified format; the data format is specified 
        /// by a string. 
        /// </summary>
        /// <param name="format">A string that specifies what format to retrieve the data as.</param>
        /// <returns>
        /// A data object that has the data in the specified format, or null if the data
        /// is not available in the specified format.
        /// </returns>
        public object GetData(string format)
        {
            if (_data.ContainsKey(format))
                return _data[format];
            else
                return null;
        }

        /// <summary>
        /// Checks to see whether the data is available in, or can be converted to, a
        /// specified format; the data format is specified by a string.
        /// </summary>
        /// <param name="format">A string that specifies the format to check for.</param>
        /// <returns>True if the data is in, or can be converted to, the specified format; otherwise,
        /// false.</returns>
        public bool GetDataPresent(string format)
        {
            return _data.ContainsKey(format);
        }

        /// <summary>
        /// Returns a list of all formats that the data in this data object is stored
        /// in, or can be converted to.
        /// </summary>
        /// <returns>
        /// An array of strings, with each string specifying the name of a format supported
        /// by this data object.
        /// </returns>
        public string[] GetFormats()
        {
            return _data.Keys.ToArray();
        }

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="data">The data to store in this data object.</param>
        public void SetData(object data)
        {
            _data[data.GetType().ToString()] = data;
        }

        /// <summary>
        /// Stores the specified data in this data object. The data format is specified by a string.
        /// </summary>
        /// <param name="format">A string that specifies what format to store the data in.</param>
        /// <param name="data">The data to store in this data object.</param>
        public void SetData(string format, object data)
        {
            _data[format] = data;
        }


    }
}
