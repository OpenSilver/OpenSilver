
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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

namespace CSHTML5.Internal
{
    internal static class INTERNAL_SizeComparisonHelpers
    {
        internal static bool AreSizesEqual(Size size1, Size size2)
        {
            //compare width:
            if (double.IsNaN(size1.Width))
            {
                if (!double.IsNaN(size2.Width))
                {
                    return false;
                }
            }
            else
            {
                if (double.IsNaN(size2.Width))
                    return false;

                if (size1.Width != size2.Width)
                {
                    return false;
                }
            }

            //compare height:
            if (double.IsNaN(size1.Height))
            {
                if (!double.IsNaN(size2.Height))
                {
                    return false;
                }
            }
            else
            {
                if (double.IsNaN(size2.Height))
                    return false;

                if (size1.Height != size2.Height)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
