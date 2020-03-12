

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


#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    public sealed partial class EasingDoubleKeyFrame : DoubleKeyFrame
    {
        public static readonly DependencyProperty EasingFunctionProperty =  DependencyProperty.Register("EasingFunction",
                                                                                                        typeof(EasingFunctionBase),
                                                                                                        typeof(EasingDoubleKeyFrame),
                                                                                                        new PropertyMetadata(null)
                                                                                                        { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        

#if WORKINPROGRESS
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
#else
        /// <summary>
        /// EasingFunction
        /// </summary>
        public EasingFunctionBase EasingFunction
        {
            get
            {
                return (EasingFunctionBase)GetValue(EasingFunctionProperty);
            }
            set
            {
                SetValue(EasingFunctionProperty, value);
            }
        }
#endif

        internal override EasingFunctionBase INTERNAL_GetEasingFunction()
        {
            return (EasingFunctionBase)EasingFunction;
        }
    }
}
