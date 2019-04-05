
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

namespace System.Xml
{
    /// <summary>
    /// Table of atomized string objects.
    /// </summary>
#if !BRIDGE
    [JSIL.Meta.JSStubOnly]
#endif
    public abstract class XmlNameTable
    {
        /// <summary>
        /// Initializes a new instance of the System.Xml.XmlNameTable class.
        /// </summary>
        protected XmlNameTable()
        {
            throw new NotImplementedException();
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        /// <summary>
        /// When overridden in a derived class, atomizes the specified string and adds
        /// it to the XmlNameTable.
        /// </summary>
        /// <param name="array">The name to add.</param>
        /// <returns>The new atomized string or the existing one if it already exists.</returns>
        public abstract string Add(string array);
        
        // Exceptions:
        //   System.IndexOutOfRangeException:
        //     0 > offset-or- offset >= array.Length -or- length > array.Length The above
        //     conditions do not cause an exception to be thrown if length =0.
        //
        //   System.ArgumentOutOfRangeException:
        //     length < 0.
        /// <summary>
        /// When overridden in a derived class, atomizes the specified string and adds
        /// it to the XmlNameTable.
        /// </summary>
        /// <param name="array">The character array containing the name to add.</param>
        /// <param name="offset">Zero-based index into the array specifying the first character of the name.</param>
        /// <param name="length">The number of characters in the name.</param>
        /// <returns>
        /// The new atomized string or the existing one if it already exists. If length
        /// is zero, String.Empty is returned.
        /// </returns>
        public abstract string Add(char[] array, int offset, int length);
       
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        /// <summary>
        /// When overridden in a derived class, gets the atomized string containing the
        /// same value as the specified string.
        /// </summary>
        /// <param name="array">The name to look up.</param>
        /// <returns>The atomized string or null if the string has not already been atomized.</returns>
        public abstract string Get(string array);
        
        // Exceptions:
        //   System.IndexOutOfRangeException:
        //     0 > offset-or- offset >= array.Length -or- length > array.Length The above
        //     conditions do not cause an exception to be thrown if length =0.
        //
        //   System.ArgumentOutOfRangeException:
        //     length < 0.
        /// <summary>
        /// When overridden in a derived class, gets the atomized string containing the
        /// same characters as the specified range of characters in the given array.
        /// </summary>
        /// <param name="array">The character array containing the name to look up.</param>
        /// <param name="offset">The zero-based index into the array specifying the first character of the name.</param>
        /// <param name="length">The number of characters in the name.</param>
        /// <returns>
        /// The atomized string or null if the string has not already been atomized.
        /// If length is zero, String.Empty is returned.
        /// </returns>
        public abstract string Get(char[] array, int offset, int length);
    }
}
