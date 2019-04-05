
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
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    /// <summary>
    /// Describes the width and height of an object.
    /// </summary>
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(SizeConverter))]
#endif
    public struct Size
    {
        double _width;
        double _height;
        bool _isEmpty;

        /// <summary>
        /// Initializes a new instance of the Windows.Foundation.Size
        /// structure and assigns it an initial width and height.
        /// </summary>
        /// <param name="width">The initial width of the instance of Windows.Foundation.Size.</param>
        /// <param name="height">The initial height of the instance of Windows.Foundation.Size.</param>
        public Size(double width, double height)
        {
            _width = 0;
            _height = 0;
            _isEmpty = false;

            Width = width;
            Height = height;
        }

        /// <summary>
        /// Compares two instances of Windows.Foundation.Size for
        /// inequality.</summary>
        /// <param name="size1">The first instance of Windows.Foundation.Size to compare.</param>
        /// <param name="size2">The second instance of Windows.Foundation.Size to compare.</param>
        /// <returns>
        /// true if the instances of Windows.Foundation.Size are not equal; otherwise
        /// false.
        /// </returns>
        public static bool operator !=(Size size1, Size size2)
        {
            return (size1.Height != size2.Height || size1.Width != size2.Width);
        }

        /// <summary>
        /// Compares two instances of Windows.Foundation.Size for
        /// equality.</summary>
        /// <param name="size1">The first instance of Windows.Foundation.Size to compare.</param>
        /// <param name="size2">The second instance of Windows.Foundation.Size to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.Foundation.Size are equal; otherwise
        /// false.
        /// </returns>
        public static bool operator ==(Size size1, Size size2)
        {
            return (size1.Height == size2.Height && size1.Width == size2.Width);

        }

        /// <summary>
        /// Gets a value that represents a static empty Windows.Foundation.Size.
        /// </summary>
        public static Size Empty
        {
            get
            {
                Size size = new Size();
                size.IsEmpty = true;
                size.Width = double.NegativeInfinity;
                size.Height = double.NegativeInfinity;
                return size;
            }
        }

        /// <summary>
        /// Gets or sets the height of this instance of Windows.Foundation.Size in pixels. The default is 0. The value cannot be negative.
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (!IsEmpty && value < 0 && !double.IsNaN(value))
                {
                    throw new ArgumentException("Height cannot be lower than 0");
                }
                else
                {
                    _height = value;
                }
            }
        }
      
        /// <summary>
        /// Gets a value that indicates whether this instance of
        /// Windows.Foundation.Size is Windows.Foundation.Size.Empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _isEmpty;
            }
            internal set
            {
                _isEmpty = value;
            }
        }
      
        /// <summary>
        /// Gets or sets the width of this instance of Windows.Foundation.Size.
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (!IsEmpty && value < 0 && !double.IsNaN(value))
                {
                    throw new ArgumentException("Width cannot be lower than 0");
                }
                else
                {
                    _width = value;
                }
            }
        }

        /// <summary>
        /// Compares an object to an instance of Windows.Foundation.Size
        /// for equality.
        /// </summary>
        /// <param name="o">The System.Object to compare.</param>
        /// <returns>true if the sizes are equal; otherwise, false.</returns>
        public override bool Equals(object o)
        {
            if (o is Size)
            {
                return ((Size)o) == this;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Compares a value to an instance of Windows.Foundation.Size
        /// for equality.
        /// </summary>
        /// <param name="value">The size to compare to this current instance of Windows.Foundation.Size.</param>
        /// <returns>true if the instances of Windows.Foundation.Size are equal; otherwise, false.</returns>
        public bool Equals(Size value)
        {
            return value == this;
        }

        /// <summary>
        /// Gets the hash code for this instance of Windows.Foundation.Size.
        /// </summary>
        /// <returns>The hash code for this instance of Windows.Foundation.Size.</returns>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
            //todo
        }

        /// <summary>
        /// Returns a string representation of this Windows.Foundation.Size.
        /// </summary>
        /// <returns>A string representation of this Windows.Foundation.Size.</returns>
        public override string ToString()
        {
            return Width + "," + Height;
        }

        static Size()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Size), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string sizeAsString)
        {
            char splitter = ',';
            string trimmedSizeAsString = sizeAsString.Trim(); //we trim the string so that we don't get random spaces at the beginning and at the end act as separators (for example: Margin=" 5")
            if (!trimmedSizeAsString.Contains(','))
            {
                splitter = ' ';
            }
            string[] splittedString = trimmedSizeAsString.Split(splitter);
            if (splittedString.Length == 1)
            {
                throw new FormatException("Failed to create a Size from the string \"" + sizeAsString + "\". Premature string termination encountered while parsing \"" + sizeAsString + "\".");
            }
            else if (splittedString.Length == 2)
            {
                double width = 0d;
                double height = 0d;

                bool isParseOK = double.TryParse(splittedString[0], out width);
                isParseOK = isParseOK && double.TryParse(splittedString[1], out height);

                if (isParseOK)
                    return new Size(width, height);
            }
            throw new FormatException(sizeAsString + " is not an eligible value for a Size");
        }

    }
}