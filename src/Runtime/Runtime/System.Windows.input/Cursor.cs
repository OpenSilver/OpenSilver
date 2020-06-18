

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Input
{
    /// <summary>
    /// Represents the image used for the mouse pointer.
    /// </summary>
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(CursorConverter))]
#endif
    public sealed partial class Cursor : IDisposable
    {
        bool _isCustom = false;
        internal string _cursorHtmlString;

        internal Cursor()
        {
            
        }

        //todo: When we will add different constructors for Cursor, we will need to modify ConvertingStringToValue.PrepareStringForCursor in the compiler accordingly.

        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     cursorStream is null.
        ////
        ////   System.IO.IOException:
        ////     This constructor was unable to create a temporary file.
        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Input.Cursor class from
        ///// the specified System.IO.Stream.
        ///// </summary>
        ///// <param name="cursorStream">The System.IO.Stream that contains the cursor.</param>
        //public Cursor(Stream cursorStream)
        //{
        //    _isCustom = true; //todo: the rest
        //}
       
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     cursorFile is null.
        ////
        ////   System.ArgumentException:
        ////     cursorFile is not an .ani or .cur file name.
        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Input.Cursor class from
        ///// the specified .ani or a .cur file.
        ///// </summary>
        ///// <param name="cursorFile">The file that contains the cursor.</param>
        //public Cursor(string cursorFile)
        //{
        //    _isCustom = true; //todo: the rest
        //}

        /// <summary>
        /// Releases the resources used by the System.Windows.Input.Cursor class.
        /// </summary>
        public void Dispose()
        {
        }
       
        ///// <summary>
        ///// Returns the string representation of the System.Windows.Input.Cursor.
        ///// </summary>
        ///// <returns>The name of the cursor.</returns>
        //public override string ToString();

        internal static object INTERNAL_ConvertFromString(string cursorTypeString)
        {
            try
            {
                Cursors.INTERNAL_CursorsEnum cursorType = (Cursors.INTERNAL_CursorsEnum)Enum.Parse(typeof(Cursors.INTERNAL_CursorsEnum), cursorTypeString); // Note: "TryParse" does not seem to work in JSIL.
                Cursor cursor = new Cursor();
                cursor._cursorHtmlString = Cursors.INTERNAL_cursorEnumToCursorString[cursorType];
                return cursor;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid cursor: " + cursorTypeString, ex);
            }
        }

        static Cursor()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Cursor), INTERNAL_ConvertFromString);
            Cursors.FillCursorTypeToStringDictionary();
        }
    }
}