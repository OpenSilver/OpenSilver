
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
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class SettingsAttribute : Attribute
    {
        public SettingsAttribute()
        {
        }

        public bool GenerateJavaScriptDuringBuild { get; set; }
    }
}
