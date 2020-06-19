using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
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
