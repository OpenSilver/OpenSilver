
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
    /// Defines the element tree that is applied as the control template for a control.
    /// </summary>
    public sealed class ControlTemplate : FrameworkTemplate
    {
        private Type _targetType;

        /// <summary>
        /// Initializes a new instance of the ControlTemplate class.
        /// </summary>
        public ControlTemplate() { }

        /// <summary>
        /// Gets or sets the type to which the ControlTemplate is applied.
        /// </summary>
        public Type TargetType
        {
            get => _targetType;
            set { CheckSealed(); _targetType = value; }
        }
    }
}
