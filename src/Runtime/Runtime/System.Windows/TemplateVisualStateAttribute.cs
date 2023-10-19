

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

namespace System.Windows
{
    /// <summary>
    /// Define an expected VisualState in the contract between a Control and its
    /// ControlTemplate for use with the VisualStateManager.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed partial class TemplateVisualStateAttribute : Attribute
    {
        /// <summary>
        /// Name of the VisualState.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the VisualStateGroup containing this state.
        /// </summary>
        public string GroupName
        {
            get;
            set;
        }
    }
}
