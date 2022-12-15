
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

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace CSHTML5.Internal
{
    internal sealed class EffectiveValueEntry
    {
        private FullValueSource _source;
        private object _value;

        public EffectiveValueEntry(object value)
        {
            _source = (FullValueSource)BaseValueSourceInternal.Default;
            _value = value;
        }

        public EffectiveValueEntry(FullValueSource source)
        {
            _source = source;
            _value = DependencyProperty.UnsetValue;
        }

        public EffectiveValueEntry(BaseValueSourceInternal valueSource)
        {
            _source = (FullValueSource)valueSource;
            _value = DependencyProperty.UnsetValue;
        }

        internal object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        internal BaseValueSourceInternal BaseValueSourceInternal
        {
            get { return (BaseValueSourceInternal)(_source & FullValueSource.ValueSourceMask); }
            set { _source = (_source & ~FullValueSource.ValueSourceMask) | (FullValueSource)value; }
        }

        internal FullValueSource FullValueSource
        {
            get { return _source; }
        }

        internal ModifiedValue ModifiedValue
        {
            get
            {
                if (_value != null)
                {
                    return _value as ModifiedValue;
                }

                return null;
            }
        }

        internal bool IsExpression
        {
            get { return ReadPrivateFlag(FullValueSource.IsExpression); }
            private set { WritePrivateFlag(FullValueSource.IsExpression, value); }
        }

        internal bool IsExpressionFromStyle
        {
            get { return ReadPrivateFlag(FullValueSource.IsExpressionFromStyle); }
            private set { WritePrivateFlag(FullValueSource.IsExpressionFromStyle, value); }
        }

        internal bool IsCoerced
        {
            get { return ReadPrivateFlag(FullValueSource.IsCoerced); }
            private set { WritePrivateFlag(FullValueSource.IsCoerced, value); }
        }

        internal bool IsCoercedWithCurrentValue
        {
            get { return ReadPrivateFlag(FullValueSource.IsCoercedWithCurrentValue); }
            private set { WritePrivateFlag(FullValueSource.IsCoercedWithCurrentValue, value); }
        }

        internal bool HasModifiers
        {
            get { return (_source & FullValueSource.ModifiersMask) != 0; }
        }

        private ModifiedValue EnsureModifiedValue()
        {
            ModifiedValue modifiedValue;
            if (_value == null)
            {
                _value = modifiedValue = new ModifiedValue();
            }
            else
            {
                modifiedValue = _value as ModifiedValue;
                if (modifiedValue == null)
                {
                    modifiedValue = new ModifiedValue();
                    modifiedValue.BaseValue = _value;
                    _value = modifiedValue;
                }
            }
            return modifiedValue;
        }

        internal void SetCoercedValue(object value, object baseValue, bool coerceWithCurrentValue)
        {
            ModifiedValue modifiedValue = EnsureModifiedValue();
            if (IsCoercedWithCurrentValue)
            {
                baseValue = modifiedValue.BaseValue;
            }
            modifiedValue.CoercedValue = value;
            IsCoerced = true;
            IsCoercedWithCurrentValue = coerceWithCurrentValue;
            global::System.Diagnostics.Debug.Assert(
                Equals(baseValue, modifiedValue.BaseValue) ||
                Equals(baseValue, modifiedValue.ExpressionValue));
        }

        internal void SetExpressionValue(object value, object baseValue)
        {
            ModifiedValue modifiedValue = EnsureModifiedValue();
            modifiedValue.ExpressionValue = value;
            IsExpression = true;
            global::System.Diagnostics.Debug.Assert(Equals(baseValue, modifiedValue.BaseValue));
        }

        internal void SetExpressionFromStyleValue(object value, object baseValue)
        {
            ModifiedValue modifiedValue = EnsureModifiedValue();
            global::System.Diagnostics.Debug.Assert(modifiedValue.ExpressionValue == null, "Can't set expression from style if local expression is set");
            modifiedValue.ExpressionValue = value;
            IsExpressionFromStyle = true;
            global::System.Diagnostics.Debug.Assert(Equals(baseValue, modifiedValue.BaseValue));
        }

        internal void ResetCoercedValue()
        {
            if (IsCoerced)
            {
                ModifiedValue modifiedValue = ModifiedValue;
                modifiedValue.CoercedValue = null;
                IsCoerced = false;

                if (!HasModifiers)
                {
                    Value = modifiedValue.BaseValue;
                }
            }
        }

        internal void ResetExpressionValue()
        {
            if (IsExpression)
            {
                ModifiedValue modifiedValue = ModifiedValue;
                modifiedValue.ExpressionValue = null;
                IsExpression = false;

                if (!HasModifiers)
                {
                    Value = modifiedValue.BaseValue;
                }
            }
        }

        internal void ResetExpressionFromStyleValue()
        {
            if (IsExpressionFromStyle)
            {
                ModifiedValue modifiedValue = ModifiedValue;
                modifiedValue.ExpressionValue = null;
                IsExpressionFromStyle = false;

                if (!HasModifiers)
                {
                    Value = modifiedValue.BaseValue;
                }
            }
        }

        internal void ResetValue()
        {
            if (HasModifiers)
            {
                ModifiedValue modifiedValue = ModifiedValue;
                _source &= FullValueSource.ValueSourceMask;
                Value = modifiedValue.BaseValue;
            }
        }

        private void WritePrivateFlag(FullValueSource bit, bool value)
        {
            if (value)
            {
                _source |= bit;
            }
            else
            {
                _source &= ~bit;
            }
        }

        private bool ReadPrivateFlag(FullValueSource bit)
        {
            return (_source & bit) != 0;
        }
    }
}
