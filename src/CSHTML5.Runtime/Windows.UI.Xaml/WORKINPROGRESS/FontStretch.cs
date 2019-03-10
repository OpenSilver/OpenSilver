
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
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    #region Not supported yet
    [SupportsDirectContentViaTypeFromStringConverters]
    public struct FontStretch : IFormattable
    {
        public string ToString(string format, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        static FontStretch()
        {
            TypeFromStringConverters.RegisterConverter(typeof(FontStretch), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string fontStretchAsString)
        {
            return new FontStretch();
        }
    }
    #endregion
#endif
}
