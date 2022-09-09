using System;
using System.Collections.Generic;
using OpenSilver.Internal.Data;

#if MIGRATION
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace CSHTML5.Internal
{
    internal class INTERNAL_PropertyStorage
    {
        #region Data

        private readonly object[] _values;
        /// <summary>
        /// This value should always be false, except when the value as seen in the Visual tree does not match the value in C#. It happens during animations that use Velocity.
        /// If it is set to true, it means that the next time the property's value value is set, we will have to force it to go through the Property changed callbacks so we can be sure the visuals fit the C# value.
        /// Note: In Silverlight, these animations update the C# value throughout the animation, but we do not do that to reduce the impact on performance.
        /// </summary>
        internal bool INTERNAL_IsVisualValueDirty;

        #endregion

        #region Constructor

        public INTERNAL_PropertyStorage(DependencyObject owner, DependencyProperty property, PropertyMetadata typeMetadata)
        {
            this._values = new object[5] 
            {
                DependencyProperty.UnsetValue, //Local
                DependencyProperty.UnsetValue, //Animated
                DependencyProperty.UnsetValue, //LocalStyle
                DependencyProperty.UnsetValue, //ThemeStyle
                DependencyProperty.UnsetValue, //Inherited
            };

            this.Entry = new EffectiveValueEntry(typeMetadata.DefaultValue);
            this.Owner = owner;
            this.Property = property;
            this.TypeMetadata = typeMetadata;
        }

        #endregion

        #region Properties

        public DependencyObject Owner { get; }
        public DependencyProperty Property { get; }
        public PropertyMetadata TypeMetadata { get; }

        internal EffectiveValueEntry Entry { get; set; }

        internal bool IsAnimatedOverLocal { get; set; }

        internal object LocalValue
        {
            get { return this._values[4]; }
            set
            {
                this._values[4] = value;
                this.IsAnimatedOverLocal = (value == DependencyProperty.UnsetValue && this.AnimatedValue != DependencyProperty.UnsetValue);
            }
        }

        internal object AnimatedValue
        {
            get { return this._values[3]; }
            set
            {
                this._values[3] = value;
                this.IsAnimatedOverLocal = (value != DependencyProperty.UnsetValue);
            }
        }

        internal object LocalStyleValue
        {
            get { return this._values[2]; }
            set { this._values[2] = value; }
        }

        internal object ThemeStyleValue
        {
            get { return this._values[1]; }
            set { this._values[1] = value; }
        }

        internal object InheritedValue
        {
            get { return this._values[0]; }
            set { this._values[0] = value; }
        }

        internal List<IPropertyChangedListener> PropertyListeners { get; set; }

        #endregion
    }

    internal class ModifiedValue
    {
        private object _baseValue;
        private object _expressionValue;
        private object _coercedValue;

        internal object BaseValue
        {
            get { return this._baseValue; }
            set { this._baseValue = value; }
        }

        internal object ExpressionValue
        {
            get { return this._expressionValue; }
            set { this._expressionValue = value; }
        }

        internal object CoercedValue
        {
            get { return this._coercedValue; }
            set { this._coercedValue = value; }
        }
    }

    internal enum FullValueSource : short
    {
        // Bit used to store BaseValueSourceInternal = 0x01
        // Bit used to store BaseValueSourceInternal = 0x02
        // Bit used to store BaseValueSourceInternal = 0x04
        // Bit used to store BaseValueSourceInternal = 0x08

        ValueSourceMask = 0x000F, // 15
        ModifiersMask = 0x0070,  // 112
        IsExpression = 0x0010, // 16
        IsExpressionFromStyle = 0x0020, // 32
        IsCoerced = 0x0040, // 64
        IsCoercedWithCurrentValue = 0x0200, // 256
    }

    // Note that these enum values are arranged in the reverse order of
    // precendence for these sources. Local value has highest
    // precedence and Default value has the least.
    internal enum BaseValueSourceInternal : short
    {
        Default = 0,
        Inherited = 1,
        ThemeStyle = 2,
        LocalStyle = 3,
        Animated = 4, // Note: in WPF, animated values are stored in the ModifiedValue
        Local = 5,
    }
}
