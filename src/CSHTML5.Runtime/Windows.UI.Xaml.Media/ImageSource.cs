
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
using System.Windows.Media.Imaging;
#else
using Windows.UI.Xaml.Media.Imaging;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Provides an object source type for Source and ImageSource.
    /// </summary>
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(ImageSourceConverter))]
#endif
    public partial class ImageSource : DependencyObject
    {
        static ImageSource()
        {
            TypeFromStringConverters.RegisterConverter(typeof(ImageSource), INTERNAL_ConvertFromString);
        }


        internal static object INTERNAL_ConvertFromString(string str1)
        {
            UriKind uriKind;
            string str = str1;
            string pa = "pack://application:,,,/";
            if (str.StartsWith(pa))
            {
                str = str1.Substring(pa.Length);
                uriKind = UriKind.Relative;
            }
            else if (str.Contains(@":/"))
                uriKind = UriKind.Absolute;
            else
                uriKind = UriKind.Relative;
            BitmapImage returnValue = new BitmapImage(new Uri(str, uriKind));
            return returnValue;
        }

    }
}