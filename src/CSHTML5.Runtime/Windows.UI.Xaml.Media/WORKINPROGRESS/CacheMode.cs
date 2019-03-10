
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using CSHTML5.Internal;
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
    public abstract class CacheMode : DependencyObject
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
