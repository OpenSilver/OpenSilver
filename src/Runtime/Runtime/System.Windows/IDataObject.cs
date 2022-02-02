

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


using System;

#if MIGRATION
namespace Microsoft.Windows
#else
namespace System.Windows
#endif
{
    /// <summary>
    /// Provides a format-independent mechanism for transferring data.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public partial interface IDataObject
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
        /// Retrieves a data object in a specified format; the data format is specified by
        /// a System.Type object.
        /// </summary>
        /// <param name="format">A System.Type object that specifies what format to retrieve the data as.</param>
        /// <returns>A data object with the data in the specified format, or null if the data
        /// is not available in the specified format.</returns>
        object GetData(Type format);

        /// <summary>
        /// Checks to see whether the data is available in, or can be converted 
        /// to, a specified format; the data format is specified by a string.
        /// </summary>
        /// <param name="format">A string that specifies what format to check for.</param>
        /// <returns>True if the data is in, or can be converted to, the specified format; otherwise,
        /// false.</returns>
        bool GetDataPresent(string format);



        /// <summary>
        /// Returns a list of formats in which the data in this data object is stored,
        /// or can be converted to.
        /// </summary>
        /// <returns>An array of strings, with each string specifying the name of a format that
        /// this data object supports.</returns>
        string[] GetFormats();



        /// <summary>
        /// Stores the specified data in this data object, automatically determining
        /// the data format from the source object type.
        /// </summary>
        /// <param name="data">An object that represents the data to store in this data object.</param>
        void SetData(object data);

        /// <summary>
        /// Stores the specified data in this data object. The data format is specified by a string.
        /// </summary>
        /// <param name="format">A string that specifies what format to store the data in.</param>
        /// <param name="data">The data to store in this data object.</param>
        void SetData(string format, object data);




    }
}