// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows
{
    /// <summary>
    /// Provides a basic implementation of the System.Windows.IDataObject interface,
    /// which defines a format-independent mechanism for transferring data.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class DataObject : IDataObject
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        private object Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the DataObject class.
        /// </summary>
        public DataObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataObject class.
        /// </summary>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        public DataObject(object data) : this()
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Returns data in a format specified by a string.
        /// </summary>
        /// <param name="format">A string that specifies the format for the 
        /// data.</param>
        /// <returns>An object that contains the data in the specified format, 
        /// or null if the data is unavailable in the specified format.</returns>
        public object GetData(string format)
        {
            return (Data != null && Data.GetType().FullName == format) ? Data : null;
        }

        /// <summary>
        /// Returns a data object in a format specified by a System.Type object.
        /// </summary>
        /// <param name="format">A System.Type that specifies the format for the data.</param>
        /// <returns>A data object with the data in the specified format, or null if the data
        /// is unavailable in the specified format.</returns>
        public object GetData(Type format)
        {
            if (Data != null && format == Data.GetType())
            {
                return Data;
            }
            return null;
        }

        /// <summary>
        /// Returns data in a format specified by a string.
        /// </summary>
        /// <param name="format">A string that specifies the format for the 
        /// data.</param>
        /// <param name="autoConvert">True to attempt to automatically convert 
        /// the data to the specified format; false for no data format conversion.</param>
        /// <returns>An object that contains the data in the specified format, 
        /// or null if the data is unavailable in the specified format.</returns>
        public object GetData(string format, bool autoConvert)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the data is available in, or can be converted to, a format
        /// specified by a string.
        /// </summary>
        /// <param name="format">A string that specifies the format for the data.</param>
        /// <returns>True if the data is in, or can be converted to, the specified format; otherwise,
        /// false.</returns>
        public bool GetDataPresent(string format)
        {
            return (Data != null && Data.GetType().FullName == format);
        }

        /// <summary>
        /// Determines whether the data is available in, or can be converted to, a format
        /// specified by a System.Type object.
        /// </summary>
        /// <param name="format">A System.Type that specifies the format for the data.</param>
        /// <returns>True if the data is in, or can be converted to, the specified format; otherwise,
        /// false.</returns>
        public bool GetDataPresent(Type format)
        {
            return (Data != null && Data.GetType() == format);
        }

        /// <summary>
        /// Determines whether the data is available in, or can be converted to, a format
        /// specified by a System.Type object.
        /// </summary>
        /// <param name="format">A System.Type that specifies the format for the data.</param>
        /// <param name="autoConvert">True to attempt to automatically convert 
        /// the data to the specified format; false for no data format conversion.</param>
        /// <returns>An object that contains the data in the specified format, 
        /// or null if the data is unavailable in the specified format.</returns>
        public bool GetDataPresent(string format, bool autoConvert)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of formats in which the data in this data object is stored,
        /// or can be converted to.
        /// </summary>
        /// <returns>An array of strings, with each string specifying the name of a format that
        /// this data object supports.</returns>
        public string[] GetFormats()
        {
            if (Data == null)
            {
                return Array.Empty<string>();
            }
            return new string[] { Data.GetType().FullName };
        }

        /// <summary>
        /// Returns a list of formats in which the data in this data object is stored.
        /// A Boolean flag indicates whether to also include formats that the data can
        /// be automatically converted to.
        /// </summary>
        /// <param name="autoConvert">True to attempt to automatically convert 
        /// the data to the specified format; false for no data format conversion.</param>
        /// <returns>An array of strings, with each string specifying the name of a format that
        /// this data object supports.</returns>
        public string[] GetFormats(bool autoConvert)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        public void SetData(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            Data = data;
        }

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="format">A string that specifies the format for the data.</param>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        public void SetData(string format, object data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="format">A System.Type that specifies the format for the data.</param>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        public void SetData(Type format, object data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="format">A string that specifies the format for the data.</param>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        /// <param name="autoConvert">True to attempt to automatically convert 
        /// the data to the specified format; false for no data format conversion.</param>
        public void SetData(string format, object data, bool autoConvert)
        {
            throw new NotImplementedException();
        }
    }
}