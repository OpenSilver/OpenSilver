
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
using System.Windows;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
#if WORKINPROGRESS
    [SupportsDirectContentViaTypeFromStringConverters]
    public abstract partial class CacheMode : DependencyObject
    {
        static CacheMode()
        {
            TypeFromStringConverters.RegisterConverter(typeof(CacheMode), INTERNAL_ConvertFromString);            
        }

        internal static object INTERNAL_ConvertFromString(string cacheModeAsString)
        {
            string cacheModeAsStringToLower = cacheModeAsString.ToLower();
            switch (cacheModeAsStringToLower)
            {
                case "bitmapcache":
                    return new BitmapCache();
                default:
                    throw new Exception("\"" + cacheModeAsString + "\"" + " is not a supported type.");
            }
        }
    }
#endif
}
