
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
    /// Specifies the version number of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class CompilerVersionNumberAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the CompilerVersionNumberAttribute class with the
        /// version number of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.
        /// </summary>
        /// <param name="versionNumber">The version number of the C#/XAML for HTML5 compiler used to compile the assembly being attributed.</param>
        public CompilerVersionNumberAttribute(string versionNumber)
        {
            VersionNumber = versionNumber;
        }

        /// <summary>
        /// The version number of the C#/XAML for HTML5 compiler used to compile the assembly being attributed
        /// </summary>
        public string VersionNumber
        {
            get => Version?.ToString();
            set => Version = new Version(value);
        }

        /// <summary>
        /// The version of the C#/XAML for HTML5 compiler used to compile the assembly being attributed
        /// </summary>
        public Version Version { get; private set; }
    }
}
