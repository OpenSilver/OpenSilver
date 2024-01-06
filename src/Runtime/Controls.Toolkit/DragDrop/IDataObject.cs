// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Windows
{
    /// <summary>
    /// Provides a format-independent mechanism for transferring data.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public interface IDataObject
    {
        /// <summary>
        /// Retrieves a data object in a specified format; the data format is specified
        /// by a string.
        /// </summary>
        /// <param name="format">A string that specifies what format to retrieve
        /// the data as.</param>
        /// <returns>A data object with the data in the specified format, or null if the data
        /// is not available in the specified format.</returns>
        object GetData(string format);

        /// <summary>
        /// Retrieves a data object in a specified format; the data format is specified
        /// by a System.Type object.
        /// </summary>
        /// <param name="format">A System.Type object that specifies what format
        /// to retrieve the data as.</param>
        /// <returns>A data object with the data in the specified format, or null if the data
        /// is not available in the specified format.</returns>
        object GetData(Type format);

        /// <summary>
        /// Retrieves a data object in a specified format, optionally converting the data to the specified format.
        /// </summary>
        /// <param name="format">A string that specifies what format to retrieve
        /// the data as. See the System.Windows.DataFormats class for a set of predefined data formats.</param>
        /// <param name="autoConvert">True to attempt to automatically convert the data to the specified format;
        /// false for no data format conversion.    If this parameter is false, the method
        /// returns data in the specified format if available, or null if the data is
        /// not available in the specified format.</param>
        /// <returns>A data object with the data in the specified format, or null if the data is not available in the specified format.</returns>
        object GetData(string format, bool autoConvert);

        /// <summary>
        /// Checks to see whether the data is available in, or can be converted 
        /// to, a specified format; the data format is specified by a string.
        /// </summary>
        /// <param name="format">A string that specifies what format to check for.</param>
        /// <returns>True if the data is in, or can be converted to, the specified format; otherwise,
        /// false.</returns>
        bool GetDataPresent(string format);

        /// <summary>
        /// Checks to see whether the data is available in, or can be converted 
        /// to, a specified format. The data format is specified by a 
        /// System.Type object.
        /// </summary>
        /// <param name="format">A System.Type that specifies what format to 
        /// check for. See the System.Windows.DataFormats class for a set of 
        /// predefined data formats.</param>
        /// <returns>True if the data is in, or can be converted to, the 
        /// specified format; otherwise, false.</returns>
        bool GetDataPresent(Type format);

        /// <summary>
        /// Determines whether the data is available in, or can be converted to, a format
        /// specified by a System.Type object.
        /// </summary>
        /// <param name="format">A System.Type that specifies the format for the data.</param>
        /// <param name="autoConvert">True to attempt to automatically convert 
        /// the data to the specified format; false for no data format conversion.</param>
        /// <returns>An object that contains the data in the specified format, 
        /// or null if the data is unavailable in the specified format.</returns>
        bool GetDataPresent(string format, bool autoConvert);

        /// <summary>
        /// Returns a list of formats in which the data in this data object is stored,
        /// or can be converted to.
        /// </summary>
        /// <returns>An array of strings, with each string specifying the name of a format that
        /// this data object supports.</returns>
        string[] GetFormats();

        /// <summary>
        /// Returns a list of formats in which the data in this data object is stored.
        /// A flag indicates whether to also include formats that the data can
        /// be automatically converted to.
        /// </summary>
        /// <param name="autoConvert">True to attempt to automatically convert 
        /// the data to the specified format; false for no data format conversion.</param>
        /// <returns>An array of strings, with each string specifying the name of a format that
        /// this data object supports.</returns>
        string[] GetFormats(bool autoConvert);

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        void SetData(object data);

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="format">A string that specifies the format for the data.</param>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        void SetData(string format, object data);

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="format">A System.Type that specifies the format for the data.</param>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        void SetData(Type format, object data);

        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="format">A string that specifies the format for the data.</param>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        /// <param name="autoConvert">True to attempt to automatically convert 
        /// the data to the specified format; false for no data format conversion.</param>
        void SetData(string format, object data, bool autoConvert);
    }
}