

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
    public sealed class EasingDoubleKeyFrame : DoubleKeyFrame
    {
        public static readonly DependencyProperty EasingFunctionProperty = 
            DependencyProperty.Register(
                "EasingFunction",
                typeof(EasingFunctionBase),
                typeof(EasingDoubleKeyFrame),
                new PropertyMetadata(null));

        public IEasingFunction EasingFunction
        {
            get
            {
                return (IEasingFunction)GetValue(EasingFunctionProperty);
            }
            set
            {
                SetValue(EasingFunctionProperty, value);
            }
        }

        internal override EasingFunctionBase INTERNAL_GetEasingFunction()
        {
            return (EasingFunctionBase)EasingFunction;
        }
    }
}
