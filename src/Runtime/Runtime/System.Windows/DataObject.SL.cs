
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
using System.Security;

namespace System.Windows
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IDataObject"/> interface,
    /// which defines a format-independent mechanism for transferring data.
    /// </summary>
    public class DataObject : IDataObject
    {
#pragma warning disable CS0649
        private bool _isDropMode;
#pragma warning restore CS0649

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObject"/> class.
        /// </summary>
        public DataObject() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataObject"/> class, with specified
        /// initial data.
        /// </summary>
        /// <param name="data">
        /// An object that represents the data to store in this data object.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        public DataObject(object data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a data object in a specified format; the data format is specified by
        /// a string.
        /// </summary>
        /// <param name="format">
        /// A string that specifies what format to retrieve the data as. Use the constant
        /// System.Windows.DataFormats.FileDrop.
        /// </param>
        /// <returns>
        /// A data object that has the data in the specified format, or null if the data
        /// </returns>
        /// <exception cref="ArgumentException">
        /// format is not equivalent to System.Windows.DataFormats.FileDrop.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public object GetData(string format)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a data object in a specified format; the data format is specified by
        /// a <see cref="Type"/> object. Always throws an exception.
        /// </summary>
        /// <param name="format">
        /// A <see cref="Type"/> object that specifies the format in which to retrieve the data.
        /// </param>
        /// <returns>
        /// Always throws an exception.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public object GetData(Type format)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a data object in a specified format, optionally converting the data
        /// to the specified format. Always throws an exception.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format in which to retrieve the data.
        /// </param>
        /// <param name="autoConvert">
        /// true to attempt to automatically convert the data to the specified format; false
        /// for no data format conversion.
        /// </param>
        /// <returns>
        /// Always throws an exception.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public object GetData(string format, bool autoConvert)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks to see whether the data is available in, or can be converted to, a specified
        /// format; the data format is specified by a string.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format to check for. See the System.Windows.DataFormats
        /// class for a set of predefined data formats.
        /// </param>
        /// <returns>
        /// true if the data is in the specified format, and is not null. false if format
        /// is not equivalent to <see cref="DataFormats.FileDrop"/>, or if the data is null.
        /// </returns>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public bool GetDataPresent(string format)
        {
            CheckIsDropMode();
            return format == DataFormats.FileDrop;
        }

        /// <summary>
        /// Checks whether the data is available in, or can be converted to, a specified
        /// format. The data format is specified by a <see cref="Type"/> object. Always throws an
        /// exception.
        /// </summary>
        /// <param name="format">
        /// A <see cref="Type"/> that specifies the format to check for.
        /// </param>
        /// <returns>
        /// Always throws an exception.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public bool GetDataPresent(Type format)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the data is available in, or can be converted to, a specified
        /// format. A Boolean flag indicates whether to check if the data can be converted
        /// to the specified format, if it is not available in that format. Always throws
        /// an exception.
        /// </summary>
        /// <param name="format">
        /// A string that specifies the format to check for. See the <see cref="DataFormats"/>
        /// class for a set of predefined data formats.
        /// </param>
        /// <param name="autoConvert">
        /// false to only check for the specified format; true to also check whether data
        /// stored in this data object can be converted to the specified format.
        /// </param>
        /// <returns>
        /// Always throws an exception.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public bool GetDataPresent(string format, bool autoConvert)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of all formats that the data in this data object is stored in,
        /// or can be converted to.
        /// </summary>
        /// <returns>
        /// An array of strings, with each string specifying the name of a format supported
        /// by this data object.
        /// </returns>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public string[] GetFormats()
        {
            CheckIsDropMode();
            return new string[1] { DataFormats.FileDrop };
        }

        /// <summary>
        /// Returns a list of all formats that the data in this data object is stored in.
        /// A Boolean flag indicates whether to also include formats that the data can be
        /// automatically converted to. Always throws an exception.
        /// </summary>
        /// <param name="autoConvert">
        /// true to retrieve all formats that data in this data object is stored in, or converted
        /// to; false to retrieve only formats that data stored in this data object is stored
        /// in (excluding formats that the data is not stored in, but can be automatically
        /// converted to).
        /// </param>
        /// <returns>
        /// Always throws an exception.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public string[] GetFormats(bool autoConvert)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the specified data in this data object and automatically converts the
        /// data format from the source object type. Always throws an exception.
        /// </summary>
        /// <param name="data">
        /// The data to store in this data object.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public void SetData(object data)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Stores the specified data in this data object, along with one or more specified
        /// data formats. The data format is specified by a string. Always throws an exception.
        /// </summary>
        /// <param name="format">
        /// A string that specifies what format to store the data in. See the <see cref="DataFormats"/>
        /// class for a set of predefined data formats.
        /// </param>
        /// <param name="data">
        /// The data to store in this data object.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public void SetData(string format, object data)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the specified data in this data object, together with one or more specified
        /// data formats. The data format is specified by a <see cref="Type"/> class. Always throws
        /// an exception.
        /// </summary>
        /// <param name="format">
        /// A <see cref="Type"/> that specifies the format in which to store the data.
        /// </param>
        /// <param name="data">
        /// The data to store in this data object.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public void SetData(Type format, object data)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Stores the specified data in this data object, together with one or more specified
        /// data formats. This overload includes a Boolean flag to indicate whether the data
        /// may be converted to another format on retrieval. Always throws an exception.
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
        /// <exception cref="NotImplementedException">
        /// Not implemented.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Attempted access in a mode other than Drop.
        /// </exception>
        public void SetData(string format, object data, bool autoConvert)
        {
            CheckIsDropMode();
            throw new NotImplementedException();
        }

        private void CheckIsDropMode()
        {
            if (!_isDropMode)
            {
                throw new SecurityException();
            }
        }
    }
}
