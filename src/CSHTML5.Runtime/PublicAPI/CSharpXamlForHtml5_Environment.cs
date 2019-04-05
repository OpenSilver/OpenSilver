
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



using CSHTML5;
using CSHTML5.Internal;
#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

public static partial class CSharpXamlForHtml5
{
    /// <summary>
    /// A class that allows to know the current Environnement (between Running in C# or in Javascript)
    /// </summary>
    public static partial class Environment
    {
        /// <summary>
        /// Gets a boolean saying if we are currently running in Javascript.
        /// </summary>
        public static bool IsRunningInJavaScript
        {
            get
            {
                return !INTERNAL_InteropImplementation.IsRunningInTheSimulator();
            }
        }
    }

}
