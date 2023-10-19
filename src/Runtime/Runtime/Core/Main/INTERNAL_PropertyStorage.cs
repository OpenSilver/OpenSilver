
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
using System.Windows;

namespace CSHTML5.Internal
{
    internal sealed class INTERNAL_PropertyStorage
    {
        private object _local;
        private object _animated;
        private object _localStyle;
        private object _themeStyle;
        private object _inherited;

        /// <summary>
        /// This value should always be false, except when the value as seen in the Visual tree does not match the value in C#. It happens during animations that use Velocity.
        /// If it is set to true, it means that the next time the property's value value is set, we will have to force it to go through the Property changed callbacks so we can be sure the visuals fit the C# value.
        /// Note: In Silverlight, these animations update the C# value throughout the animation, but we do not do that to reduce the impact on performance.
        /// </summary>
        internal bool INTERNAL_IsVisualValueDirty;

        internal INTERNAL_PropertyStorage()
        {
            _local = DependencyProperty.UnsetValue;
            _animated = DependencyProperty.UnsetValue;
            _localStyle = DependencyProperty.UnsetValue;
            _themeStyle = DependencyProperty.UnsetValue;
            _inherited = DependencyProperty.UnsetValue;
        }

        internal static INTERNAL_PropertyStorage CreateDefaultValueEntry(object value)
        {
            return new INTERNAL_PropertyStorage
            {
                Entry = new EffectiveValueEntry(value)
            };
        }

        internal EffectiveValueEntry Entry { get; set; }

        internal bool IsAnimatedOverLocal { get; set; }

        internal object LocalValue
        {
            get => _local;
            set
            {
                _local = value;
                IsAnimatedOverLocal = (value == DependencyProperty.UnsetValue && AnimatedValue != DependencyProperty.UnsetValue);
            }
        }

        internal object AnimatedValue
        {
            get => _animated;
            set
            {
                _animated = value;
                IsAnimatedOverLocal = (value != DependencyProperty.UnsetValue);
            }
        }

        internal object LocalStyleValue
        {
            get => _localStyle;
            set => _localStyle = value;
        }

        internal object ThemeStyleValue
        {
            get => _themeStyle;
            set => _themeStyle = value;
        }

        internal object InheritedValue
        {
            get => _inherited;
            set => _inherited = value;
        }
    }
}
