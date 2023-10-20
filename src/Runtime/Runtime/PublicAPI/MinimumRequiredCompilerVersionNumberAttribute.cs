
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
    /// Specifies the minimum version of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class MinimumRequiredCompilerVersionNumberAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the MinimumRequiredCompilerVersionNumberAttribute class with the minimum
        /// version number of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed.
        /// </summary>
        /// <param name="versionNumber">The minimum version number of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed.</param>
        public MinimumRequiredCompilerVersionNumberAttribute(string versionNumber)
        {
            VersionNumber = versionNumber;
        }

        /// <summary>
        /// The minimum version number of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed
        /// </summary>
        public string VersionNumber
        {
            get => Version?.ToString();
            set => Version = new Version(value);
        }

        /// <summary>
        /// The minimum version of the C#/XAML for HTML5 compiler that is required to process the assembly being attributed
        /// </summary>
        public Version Version { get; private set; }
    }
}
