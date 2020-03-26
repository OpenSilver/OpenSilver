

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
