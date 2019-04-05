
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
