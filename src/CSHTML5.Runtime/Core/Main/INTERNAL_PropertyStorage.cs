using System;
using System.Collections.Generic;
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

        private FullValueSource _source;
        private object _value;
        private readonly object[] _values;

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
            this._source = (FullValueSource)BaseValueSourceInternal.Default;
            this.Owner = owner;
            this.Property = property;
            this.TypeMetadata = typeMetadata;
            this.Value = typeMetadata.DefaultValue;
        }

        #endregion

        #region Properties

        public DependencyObject Owner { get; }
        public DependencyProperty Property { get; }
        public PropertyMetadata TypeMetadata { get; }

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
        internal object Value
        {
            get { return this._value; }
            set { this._value = value; }
        }
        internal List<IPropertyChangedListener> PropertyListeners { get; set; }

        internal BaseValueSourceInternal BaseValueSourceInternal
        {
            get { return (BaseValueSourceInternal)(this._source & FullValueSource.ValueSourceMask); }
            set { this._source = (this._source & ~FullValueSource.ValueSourceMask) | (FullValueSource)value; }
        }

        internal FullValueSource FullValueSource
        {
            get { return this._source; }
        }

        #endregion

        #region Methods

        private void WritePrivateFlag(FullValueSource bit, bool value)
        {
            if (value)
            {
                this._source |= bit;
            }
            else
            {
                this._source &= ~bit;
            }
        }

        private bool ReadPrivateFlag(FullValueSource bit)
        {
            return (this._source & bit) != 0;
        }

        #endregion

        #region Modified Value
        internal ModifiedValue ModifiedValue
        {
            get
            {
                if (this._value != null)
                {
                    return this._value as ModifiedValue;
                }
                return null;
            }
        }
        internal bool IsExpression
        {
            get { return this.ReadPrivateFlag(FullValueSource.IsExpression); }
            private set { this.WritePrivateFlag(FullValueSource.IsExpression, value); }
        }
        internal bool IsExpressionFromStyle
        {
            get { return this.ReadPrivateFlag(FullValueSource.IsExpressionFromStyle); }
            private set { this.WritePrivateFlag(FullValueSource.IsExpressionFromStyle, value); }
        }
        internal bool IsCoerced
        {
            get { return this.ReadPrivateFlag(FullValueSource.IsCoerced); }
            private set { this.WritePrivateFlag(FullValueSource.IsCoerced, value); }
        }
        internal bool IsCoercedWithCurrentValue
        {
            get { return this.ReadPrivateFlag(FullValueSource.IsCoercedWithCurrentValue); }
            private set { this.WritePrivateFlag(FullValueSource.IsCoercedWithCurrentValue, value); }
        }
        internal bool IsAnimatedOverLocal
        {
            get { return this.ReadPrivateFlag(FullValueSource.IsLocalOverAnimatedValue); }
            set { this.WritePrivateFlag(FullValueSource.IsLocalOverAnimatedValue, value); }
        }
        internal bool HasModifiers
        {
            get { return (this._source & FullValueSource.ModifiersMask) != 0; }
        }
        private ModifiedValue EnsureModifiedValue()
        {
            ModifiedValue modifiedValue;
            if (this._value == null)
            {
                this._value = modifiedValue = new ModifiedValue();
            }
            else
            {
                modifiedValue = this._value as ModifiedValue;
                if (modifiedValue == null)
                {
                    modifiedValue = new ModifiedValue();
                    modifiedValue.BaseValue = this._value;
                    this._value = modifiedValue;
                }
            }
            return modifiedValue;
        }
        internal void SetCoercedValue(object value, object baseValue, bool coerceWithCurrentValue)
        {
            ModifiedValue modifiedValue = this.EnsureModifiedValue();
            if (this.IsCoercedWithCurrentValue)
            {
                baseValue = modifiedValue.BaseValue;
            }
            modifiedValue.CoercedValue = value;
            this.IsCoerced = true;
            this.IsCoercedWithCurrentValue = coerceWithCurrentValue;
            global::System.Diagnostics.Debug.Assert(object.Equals(baseValue, modifiedValue.BaseValue) ||
                                                    object.Equals(baseValue, modifiedValue.ExpressionValue));
        }
        internal void SetExpressionValue(object value, object baseValue)
        {
            ModifiedValue modifiedValue = this.EnsureModifiedValue();
            modifiedValue.ExpressionValue = value;
            this.IsExpression = true;
            global::System.Diagnostics.Debug.Assert(object.Equals(baseValue, modifiedValue.BaseValue));
        }
        internal void SetExpressionFromStyleValue(object value, object baseValue)
        {
            ModifiedValue modifiedValue = this.EnsureModifiedValue();
            global::System.Diagnostics.Debug.Assert(modifiedValue.ExpressionValue == null, "Can't set expression from style if local expression is set");
            modifiedValue.ExpressionValue = value;
            this.IsExpressionFromStyle = true;
            global::System.Diagnostics.Debug.Assert(object.Equals(baseValue, modifiedValue.BaseValue));
        }

        internal void ResetCoercedValue()
        {
            if (this.IsCoerced)
            {
                ModifiedValue modifiedValue = this.ModifiedValue;
                modifiedValue.CoercedValue = null;
                this.IsCoerced = false;

                if (!this.HasModifiers)
                {
                    this.Value = modifiedValue.BaseValue;
                }
            }
        }
        internal void ResetExpressionValue()
        {
            if (this.IsExpression)
            {
                ModifiedValue modifiedValue = this.ModifiedValue;
                modifiedValue.ExpressionValue = null;
                this.IsExpression = false;

                if (!this.HasModifiers)
                {
                    this.Value = modifiedValue.BaseValue;
                }
            }
        }
        internal void ResetExpressionFromStyleValue()
        {
            if (this.IsExpressionFromStyle)
            {
                ModifiedValue modifiedValue = this.ModifiedValue;
                modifiedValue.ExpressionValue = null;
                this.IsExpressionFromStyle = false;

                if (!this.HasModifiers)
                {
                    this.Value = modifiedValue.BaseValue;
                }
            }
        }
        internal void ResetValue()
        {
            if (this.HasModifiers)
            {
                ModifiedValue modifiedValue = this.ModifiedValue;
                this._source &= FullValueSource.ValueSourceMask;
                this.Value = modifiedValue.BaseValue; 
            }
        }

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
        IsLocalOverAnimatedValue = 0x0400 // 512
    }

    // Note that these enum values are arranged in the reverse order of
    // precendence for these sources. Local value has highest
    // precedence and Default value has the least.
    internal enum BaseValueSourceInternal : short
    {
        Default = 0,
        Inherited = 1,
        LocalStyle = 2,
        ThemeStyle = 3,
        Animated = 4, // Note: in WPF, animated values are stored in the ModifiedValue
        Local = 5,
    }
}
