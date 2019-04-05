
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