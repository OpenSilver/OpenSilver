
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

namespace System.Windows
{
    /// <summary>
    /// Provides a format-independent mechanism for transferring data.
    /// </summary>
    public interface IDataObject
    {
        /// <summary>
        /// Retrieves a data object in a specified format; the data format is specified by a string.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format in which to retrieve the data. See the <see cref="DataFormats"/>
        /// class for a set of predefined data formats.
        /// </param>
        /// <returns>
        /// A data object that has the data in the specified format; or null, if the data is not available 
        /// in the specified format.
        /// </returns>
        object GetData(string format);

        /// <summary>
        /// Retrieves a data object in a specified format; the data format is specified by a 
        /// <see cref="Type"/> object.
        /// </summary>
        /// <param name="format">
        /// A System.Type object that specifies the format in which to retrieve the data.
        /// See the <see cref="DataFormats"/> class for a set of predefined data formats.
        /// </param>
        /// <returns>
        /// A data object that has the data in the specified format; or null, if the data
        /// is not available in the specified format.
        /// </returns>
        object GetData(Type format);

        /// <summary>
        /// Retrieves a data object in a specified format, and optionally, converts the data
        /// to the specified format.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format in which to retrieve the data. See the 
        /// <see cref="DataFormats"/> class for a set of predefined data formats.
        /// </param>
        /// <param name="autoConvert">
        /// true to attempt to automatically convert the data to the specified format; false
        /// to perform no data format conversion.If this parameter is false, the method returns
        /// data in the specified format if it is available; or returns null if the data
        /// is not available in the specified format.
        /// </param>
        /// <returns>
        /// A data object that has the data in the specified format; or null, if the data
        /// is not available in the specified format.
        /// </returns>
        object GetData(string format, bool autoConvert);

        /// <summary>
        /// Checks whether the data is available in, or can be converted to, a specified
        /// format; the data format is specified by a string.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format to check for. See the <see cref="DataFormats"/>
        /// class for a set of predefined data formats.
        /// </param>
        /// <returns>
        /// true if the data is in, or can be converted to, the specified format; otherwise, false.
        /// </returns>
        bool GetDataPresent(string format);

        /// <summary>
        /// Checks to see whether the data is available in, or can be converted to, a specified
        /// format. The data format is specified by a <see cref="Type"/> object.
        /// </summary>
        /// <param name="format">
        /// A <see cref="Type"/> that specifies the format to check for.
        /// </param>
        /// <returns>
        /// true if the data is in, or can be converted to, the specified format; otherwise, false.
        /// </returns>
        bool GetDataPresent(Type format);

        /// <summary>
        /// Checks whether the data is available in, or can be converted to, a specified
        /// format. If the data is not already available in the specified format, a Boolean
        /// flag indicates whether to check if the data can be converted to the specified
        /// format.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format to check for. See the <see cref="DataFormats"/>
        /// class for a set of predefined data formats.
        /// </param>
        /// <param name="autoConvert">
        /// false to only check for the specified format; true to also check whether the
        /// data that is stored in this data object can be converted to the specified format.
        /// </param>
        /// <returns>
        /// true if the data is in, or can be converted to, the specified format; otherwise, false.
        /// </returns>
        bool GetDataPresent(string format, bool autoConvert);

        /// <summary>
        /// Returns a list of all formats that the data in this data object is stored in,
        /// or can be converted to.
        /// </summary>
        /// <returns>
        /// An array of strings, with each string specifying the name of a format that is
        /// supported by this data object.
        /// </returns>
        string[] GetFormats();

        /// <summary>
        /// Returns a list of all formats that the data in this data object is stored in.
        /// A Boolean flag indicates whether to also include formats that the data can be
        /// automatically converted to.
        /// </summary>
        /// <param name="autoConvert">
        /// true to retrieve all formats that the data in this data object is stored in,
        /// or can be converted to; false to retrieve only the formats in which the data
        /// in this data object is stored (excludes formats that the data is not stored in,
        /// but can be automatically converted to).
        /// </param>
        /// <returns>
        /// An array of strings, with each string specifying the name of a format that is
        /// supported by this data object.
        /// </returns>
        string[] GetFormats(bool autoConvert);

        /// <summary>
        /// Stores the specified data in this data object, and automatically converts the
        /// data format from the source object type.
        /// </summary>
        /// <param name="data">
        /// The data to store in this data object.
        /// </param>
        void SetData(object data);

        /// <summary>
        /// Stores the specified data in this data object, using one or more specified data
        /// formats. The data format is specified by a string.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format in which to store the data. See the <see cref="DataFormats"/>
        /// class for a set of predefined data formats.
        /// </param>
        /// <param name="data">
        /// The data to store in this data object.
        /// </param>
        void SetData(string format, object data);

        /// <summary>
        /// Stores the specified data in this data object, using one or more specified data
        /// formats. The data format is specified by a <see cref="Type"/> class.
        /// </summary>
        /// <param name="format">
        /// A <see cref="Type"/> that specifies the format in which to store the data.
        /// </param>
        /// <param name="data">
        /// The data to store in this data object.
        /// </param>
        void SetData(Type format, object data);

        /// <summary>
        /// Stores the specified data in this data object, using one or more specified data
        /// formats. This overload includes a Boolean flag to indicate whether the data may
        /// be converted to another format on retrieval.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format in which to store the data. See the <see cref="DataFormats"/>
        /// class for a set of predefined data formats.
        /// </param>
        /// <param name="data">
        /// The data to store in this data object.
        /// </param>
        /// <param name="autoConvert">
        /// true to allow the data to be converted to another format on retrieval; false
        /// to prohibit the data from being converted to another format on retrieval.
        /// </param>
        void SetData(string format, object data, bool autoConvert);
    }
}
