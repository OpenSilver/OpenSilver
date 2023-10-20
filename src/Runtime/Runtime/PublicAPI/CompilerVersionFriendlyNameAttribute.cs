
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

namespace CSHTML5.Internal.Attributes
{
    /// <summary>
    /// Specifies the friendly name of the version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class CompilerVersionFriendlyNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the CompilerVersionFriendlyNameAttribute class with the
        /// friendly name of the version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.
        /// </summary>
        /// <param name="versionFriendlyName">The friendly name of the version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.</param>
        public CompilerVersionFriendlyNameAttribute(string versionFriendlyName)
        {
            VersionFriendlyName = versionFriendlyName;
        }

        /// <summary>
        /// The friendly name of the version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed
        /// </summary>
        public string VersionFriendlyName { get; private set; }
    }
}
