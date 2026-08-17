
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Specifies which set of conventions (template part names and associated behaviors) a
    /// control uses when a <see cref="ControlTemplate"/> is applied.
    /// </summary>
    public enum TemplateMode
    {
        /// <summary>
        /// The control inspects the applied template and automatically determines whether it is
        /// a WPF-style or a Silverlight-style template. This is the default value.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// The control uses WPF behaviors and looks for WPF template part names.
        /// </summary>
        Wpf = 1,

        /// <summary>
        /// The control uses Silverlight behaviors and looks for Silverlight template part names.
        /// </summary>
        Silverlight = 2,
    }
}
