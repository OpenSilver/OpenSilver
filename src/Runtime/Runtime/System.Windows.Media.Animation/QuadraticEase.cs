
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

namespace System.Windows.Media.Animation
{
    /// <summary>
    /// Represents an easing function that creates an animation that accelerates
    /// and/or decelerates using the formula f(t) = t^2
    /// </summary>
    public sealed partial class QuadraticEase : EasingFunctionBase
    {
        const string FUNCTION_TYPE_STRING = "Quad";

        internal override string GetFunctionAsString()
        {
            return GetEasingModeAsString() + FUNCTION_TYPE_STRING;
        }
    }
}
