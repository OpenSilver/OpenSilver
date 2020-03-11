

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
    public partial struct Size
    {
        private static Size emptySize;

        double _width;
        double _height;

        static Size()
        {
            emptySize = new Size
            {
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };
            TypeFromStringConverters.RegisterConverter(typeof(Size), INTERNAL_ConvertFromString);
        }

        /// <summary>
        /// Initializes a new instance of the Windows.Foundation.Size
        /// structure and assigns it an initial width and height.
        /// </summary>
        /// <param name="width">The initial width of the instance of Windows.Foundation.Size.</param>
        /// <param name="height">The initial height of the instance of Windows.Foundation.Size.</param>
        public Size(double width, double height)
        {
#if !BRIDGE

            if ((!double.IsNaN(width) && width < 0) || (!double.IsNaN(height) && height < 0))
            {
                throw new ArgumentException("Width and Height cannot be negative.");
            }
#else
            if(width < 0 || height < 0)
            {
                throw new ArgumentException("Width and Height cannot be negative.");
            }
#endif
            this._width = width;
            this._height = height;
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
                return emptySize;
            }
        }

        /// <summary>
        /// Gets or sets the height of this instance of Windows.Foundation.Size in pixels. The default is 0. The value cannot be negative.
        /// </summary>
        public double Height
        {
            get
            {
                return this._height;
            }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("Cannot modify Empty size.");
                }
                if(value < 0)
                {
                    throw new ArgumentException("Height cannot be negative.");
                }
                this._height = value;
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
                return this._width < 0;
            }
        }
      
        /// <summary>
        /// Gets or sets the width of this instance of Windows.Foundation.Size.
        /// </summary>
        public double Width
        {
            get
            {
                return this._width;
            }
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("Cannot modify Empty size.");
                }
                if(value < 0)
                {
                    throw new ArgumentException("Width cannot be negative.");
                }
                this._width = value;
            }
        }

        public static bool Equals(Size size1, Size size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            else
            {
                return size1.Width.Equals(size2.Width) && size1.Height.Equals(size2.Height);
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
            if ((null == o) || !(o is Size))
            {
                return false;
            }

            Size value = (Size)o;
            return Size.Equals(this, value);
        }


        /// <summary>
        /// Compares a value to an instance of Windows.Foundation.Size
        /// for equality.
        /// </summary>
        /// <param name="value">The size to compare to this current instance of Windows.Foundation.Size.</param>
        /// <returns>true if the instances of Windows.Foundation.Size are equal; otherwise, false.</returns>
        public bool Equals(Size value)
        {
            return Size.Equals(this, value);
        }

        /// <summary>
        /// Gets the hash code for this instance of Windows.Foundation.Size.
        /// </summary>
        /// <returns>The hash code for this instance of Windows.Foundation.Size.</returns>
        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            else
            {
                // Perform field-by-field XOR of HashCodes
                return this.Width.GetHashCode() ^ this.Height.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a string representation of this Windows.Foundation.Size.
        /// </summary>
        /// <returns>A string representation of this Windows.Foundation.Size.</returns>
        public override string ToString()
        {
            if (this.IsEmpty)
            {
                return "Empty";
            }
            return Width + "," + Height;
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