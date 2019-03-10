
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
    public class ImageSource : DependencyObject
    {
        static ImageSource()
        {
            TypeFromStringConverters.RegisterConverter(typeof(ImageSource), INTERNAL_ConvertFromString);
        }


        internal static object INTERNAL_ConvertFromString(string str)
        {
            UriKind uriKind;
            if (str.Contains(@":/"))
                uriKind = UriKind.Absolute;
            else
                uriKind = UriKind.Relative;
            BitmapImage returnValue = new BitmapImage(new Uri(str, uriKind));
            return returnValue;
        }

    }
}