

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
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
using System.Windows;

namespace OpenSilver.Simulator
{
    static class CrossDomainCallsHelper
    {
        public static bool IsBypassCORSErrors
        {
            get
            {
                bool isBypassCORSErrors  = Properties.Settings.Default.IsBypassCORSErrors;
                return isBypassCORSErrors;
            }
            set
            {
                Properties.Settings.Default.IsBypassCORSErrors = value;

                // SAVE:
                Properties.Settings.Default.Save();

                MessageBox.Show("Please restart the Simulator for the changes to take effect.");
            }
        }
    }
}
