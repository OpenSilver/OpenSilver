
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

using System.ComponentModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// Defines column-specific properties that apply to <see cref="Grid"/> objects.
    /// </summary>
    public sealed class ColumnDefinition : DependencyObject, IDefinitionBase
    {
        private double _effectiveMinSize;
        private double _sizeCache;
        private double _finalOffset;
        private GridUnitType _effectiveUnitType;
        private double _actualWidth;
        private Grid _parent;

        public ColumnDefinition()
        {
            _effectiveMinSize = 0;
            _effectiveUnitType = GridUnitType.Auto;
            _finalOffset = 0;
            _sizeCache = 0;
        }

        /// <summary>
        /// Returns a copy of the current <see cref="ColumnDefinition"/>.
        /// </summary>
        public ColumnDefinition Clone() =>
            new()
            {
                MinWidth = MinWidth,
                MaxWidth = MaxWidth,
                Width = Width.Clone()
            };

        /// <summary>
        /// Gets or sets a value that represents the maximum width of a <see cref="ColumnDefinition"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the maximum width in pixels. The default is 
        /// <see cref="double.PositiveInfinity"/>.
        /// </returns>
        public double MaxWidth
        {
            get => (double)GetValue(MaxWidthProperty);
            set => SetValueInternal(MaxWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MaxWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                nameof(MaxWidth),
                typeof(double),
                typeof(ColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity, OnMaxWidthChanged),
                FrameworkElement.IsMaxWidthHeightValid);

        private static void OnMaxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnDefinition)d)._parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets a value that represents the minimum width of a <see cref="ColumnDefinition"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the minimum width in pixels. The default is 0.
        /// </returns>
        public double MinWidth
        {
            get => (double)GetValue(MinWidthProperty);
            set => SetValueInternal(MinWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                nameof(MinWidth),
                typeof(double),
                typeof(ColumnDefinition),
                new PropertyMetadata(0d, OnMinWidthChanged),
                FrameworkElement.IsMinWidthHeightValid);

        private static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnDefinition)d)._parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets the calculated width of a <see cref="ColumnDefinition"/> element,
        /// or sets the <see cref="GridLength"/> value of a column that is defined by the
        /// <see cref="ColumnDefinition"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="GridLength"/> that represents the width of the column. The default
        /// value is 1.0.
        /// </returns>
        public GridLength Width
        {
            get => (GridLength)GetValue(WidthProperty);
            set => SetValueInternal(WidthProperty, value);
        }

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(GridLength),
                typeof(ColumnDefinition),
                new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), OnWidthChanged));

        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnDefinition)d)._parent?.InvalidateMeasure();
        }

        private static readonly PropertyMetadata _actualWidthMetadata = new ReadOnlyPropertyMetadata(0.0, GetActualWidth);

        private static readonly DependencyPropertyKey ActualWidthPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ActualWidth),
                typeof(double),
                typeof(ColumnDefinition),
                _actualWidthMetadata);

        private static object GetActualWidth(DependencyObject d)
        {
            ColumnDefinition column = (ColumnDefinition)d;
            return column._actualWidth;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that represents the actual calculated width of a <see cref="ColumnDefinition"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the actual calculated width in pixels. The default is 0.
        /// </returns>
        public double ActualWidth
        {
            get => _actualWidth;
            private set
            {
                double previousWidth = _actualWidth;
                if (previousWidth != value)
                {
                    _actualWidth = value;
                    NotifyPropertyChange(
                        new DependencyPropertyChangedEventArgs(
                            previousWidth,
                            value,
                            ActualWidthProperty,
                            _actualWidthMetadata));
                }
            }
        }

        internal void SetParent(Grid parent) => _parent = parent;

        double IDefinitionBase.MinLength => MinWidth;

        double IDefinitionBase.MaxLength => MaxWidth;

        GridLength IDefinitionBase.Length => Width;

        double IDefinitionBase.GetUserMaxSize() => MaxWidth;

        double IDefinitionBase.GetUserMinSize() => MinWidth;

        GridUnitType IDefinitionBase.GetUserSizeType() => Width.GridUnitType;

        double IDefinitionBase.GetUserSizeValue() => Width.Value;

        void IDefinitionBase.UpdateEffectiveMinSize(double newValue) => _effectiveMinSize = Math.Max(_effectiveMinSize, newValue);

        void IDefinitionBase.SetEffectiveUnitType(GridUnitType type) => _effectiveUnitType = type;

        void IDefinitionBase.SetEffectiveMinSize(double value) => _effectiveMinSize = value;

        double IDefinitionBase.GetEffectiveMinSize() => _effectiveMinSize;

        GridUnitType IDefinitionBase.GetEffectiveUnitType() => _effectiveUnitType;

        void IDefinitionBase.SetMeasureArrangeSize(double value) => ActualWidth = value;

        double IDefinitionBase.GetMeasureArrangeSize() => ActualWidth;

        void IDefinitionBase.SetSizeCache(double value) => _sizeCache = value;

        double IDefinitionBase.GetSizeCache() => _sizeCache;

        double IDefinitionBase.GetPreferredSize()
        {
            double actualWidth = ActualWidth;
            return (_effectiveUnitType != GridUnitType.Auto && _effectiveMinSize < actualWidth)
                ? actualWidth : _effectiveMinSize;
        }

        double IDefinitionBase.GetFinalOffset() => _finalOffset;

        void IDefinitionBase.SetFinalOffset(double value) => _finalOffset = value;
    }
}
