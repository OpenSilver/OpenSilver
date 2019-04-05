
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
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// [SECURITY CRITICAL] Describes the thickness of a frame around a rectangle.
    /// Four System.Double values describe the Windows.UI.Xaml.Thickness.Left, Windows.UI.Xaml.Thickness.Top,
    /// Windows.UI.Xaml.Thickness.Right, and Windows.UI.Xaml.Thickness.Bottom sides
    /// of the rectangle, respectively.
    /// </summary>
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(ThicknessConverter))]
#endif
#if WORKINPROGRESS
    [MethodToTranslateXamlValueToCSharp("TranslateXamlValueToCSharp")]
#endif
    [SupportsDirectContentViaTypeFromStringConverters]
    public struct Thickness
    {
        double _left, _top, _right, _bottom;

#if WORKINPROGRESS
        public static string TranslateXamlValueToCSharp(string xamlValue)
        {
            string convertedXamlValue;
            string[] xamlValueSplittedOnComas = xamlValue.Split(',');
            if (xamlValueSplittedOnComas.Length == 2)
            {
                convertedXamlValue = xamlValue + "," + xamlValue;
            }
            else
            {
                convertedXamlValue = xamlValue;
            }
#if MIGRATION
            return "new global::System.Windows(" + convertedXamlValue + ")";
#else
            return "new global::Windows.UI.Xaml(" + convertedXamlValue + ")";
#endif
        }
#endif

        /// <summary>
        /// [SECURITY CRITICAL] Initializes a Windows.UI.Xaml.Thickness structure that
        /// has the specified uniform length on each side.
        /// </summary>
        /// <param name="uniformLength">The uniform length applied to all four sides of the bounding rectangle.</param>
        public Thickness(double uniformLength)
        {
            _left = uniformLength;
            _top = uniformLength;
            _right = uniformLength;
            _bottom = uniformLength;
        }
       
        /// <summary>
        /// [SECURITY CRITICAL] Initializes a Windows.UI.Xaml.Thickness structure that
        /// has specific lengths (supplied as a System.Double) applied to each side of
        /// the rectangle.
        /// </summary>
        /// <param name="left">The thickness for the left side of the rectangle.</param>
        /// <param name="top">The thickness for the upper side of the rectangle.</param>
        /// <param name="right">The thickness for the right side of the rectangle</param>
        /// <param name="bottom">The thickness for the lower side of the rectangle.</param>
        public Thickness(double left, double top, double right, double bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        
        /// <summary>
        /// [SECURITY CRITICAL] Compares two Windows.UI.Xaml.Thickness structures for
        /// inequality.
        /// </summary>
        /// <param name="t1">The first structure to compare.</param>
        /// <param name="t2">The other structure to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.Thickness are not equal; otherwise,
        /// false.
        /// </returns>
        public static bool operator !=(Thickness t1, Thickness t2)
        {
            return (t1.Left != t2.Left || t1.Top != t2.Top || t1.Bottom != t2.Bottom || t1.Right != t2.Right);
        }
      
        /// <summary>
        /// [SECURITY CRITICAL] Compares the value of two Windows.UI.Xaml.Thickness structures
        /// for equality.</summary>
        /// <param name="t1">The first structure to compare.</param>
        /// <param name="t2">The other structure to compare.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.Thickness are equal; otherwise,
        /// false.
        /// </returns>
        public static bool operator ==(Thickness t1, Thickness t2)
        {
            return (t1.Left == t2.Left && t1.Top == t2.Top && t1.Bottom == t2.Bottom && t1.Right == t2.Right);
        }

       
        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the width, in pixels, of the lower side
        /// of the bounding rectangle.
        /// </summary>
        public double Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the width, in pixels, of the left side of
        /// the bounding rectangle.
        /// </summary>
        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }
        
        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the width, in pixels, of the right side
        /// of the bounding rectangle.
        /// </summary>
        public double Right
        {
            get { return _right; }
            set { _right = value; }
        }
        
        /// <summary>
        /// [SECURITY CRITICAL] Gets or sets the width, in pixels, of the upper side
        /// of the bounding rectangle.
        /// </summary>
        public double Top
        {
            get { return _top; }
            set { _top = value; }
        }

        
        /// <summary>
        /// [SECURITY CRITICAL] Compares this Windows.UI.Xaml.Thickness structure to
        /// another System.Object for equality.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the two objects are equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Thickness && ((Thickness)obj) == this);
        }

       
        /// <summary>
        /// [SECURITY CRITICAL] Compares this Windows.UI.Xaml.Thickness structure to
        /// another Windows.UI.Xaml.Thickness structure for equality.
        /// </summary>
        /// <param name="thickness">An instance of Windows.UI.Xaml.Thickness to compare for equality.</param>
        /// <returns>
        /// true if the two instances of Windows.UI.Xaml.Thickness are equal; otherwise,
        /// false.
        /// </returns>
        public bool Equals(Thickness thickness)
        {
            return thickness == this;
        }

        /// <summary>
        /// [SECURITY CRITICAL] Returns the hash code of the structure.
        /// </summary>
        /// <returns>A hash code for this instance of Windows.UI.Xaml.Thickness.</returns>
        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }

        /// <summary>
        /// [SECURITY CRITICAL] Returns the string representation of the Windows.UI.Xaml.Thickness
        /// structure.
        /// </summary>
        /// <returns>A System.String that represents the Windows.UI.Xaml.Thickness value.</returns>
        public override string ToString()
        {
            return Left + "," + Top + "," + Right + "," + Bottom;
        }



        static Thickness()
        {
            TypeFromStringConverters.RegisterConverter(typeof(Thickness), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string thicknessAsString)
        {
            char splitter = ',';
            string trimmedThicknessAsString = thicknessAsString.Trim(); //we trim the string so that we don't get random spaces at the beginning and at the end act as separators (for example: Margin=" 5")
            if (!trimmedThicknessAsString.Contains(','))
            {
                splitter = ' ';
            }
            string[] splittedString = trimmedThicknessAsString.Split(splitter);
            if (splittedString.Length == 1)
            {
                double thickness = 0d;
                if (double.TryParse(splittedString[0], out thickness))
                {
                    return new Thickness(thickness);
                }
            }
            else if (splittedString.Length == 2)
            {
                double leftAndRight = 0d;
                double topAndBottom = 0d;

                bool isParseOK = double.TryParse(splittedString[0], out leftAndRight);
                isParseOK = isParseOK && double.TryParse(splittedString[1], out topAndBottom);

                if (isParseOK)
                    return new Thickness(leftAndRight, topAndBottom, leftAndRight, topAndBottom);
            }
            else if (splittedString.Length == 4)
            {
                double left = 0d;
                double top = 0d;
                double right = 0d;
                double bottom = 0d;

                bool isParseOK = double.TryParse(splittedString[0], out left);
                isParseOK = isParseOK && double.TryParse(splittedString[1], out top);
                isParseOK = isParseOK && double.TryParse(splittedString[2], out right);
                isParseOK = isParseOK && double.TryParse(splittedString[3], out bottom);

                if (isParseOK)
                    return new Thickness(left, top, right, bottom);
            }
            throw new FormatException(thicknessAsString + " is not an eligible value for Thickness");
        }
    }
}