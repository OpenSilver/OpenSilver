
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
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines row-specific properties that apply to <see cref="Grid"/> elements.
    /// </summary>
    public sealed class RowDefinition : DependencyObject, IDefinitionBase
    {
        private GridUnitType _effectiveUnitType;
        private double _effectiveMinSize;
        private double _sizeCache;
        private double _finalOffset;
        internal Grid Parent;

        public RowDefinition()
        {
            _effectiveMinSize = 0;
            _effectiveUnitType = GridUnitType.Auto;
            _finalOffset = 0;
            _sizeCache = 0;
        }

        /// <summary>
        /// Returns a copy of this <see cref="RowDefinition"/>.
        /// </summary>
        public RowDefinition Clone() =>
            new()
            {
                MinHeight = MinHeight,
                MaxHeight = MaxHeight,
                Height = Height.Clone()
            };

        /// <summary>
        /// Gets or sets a value that represents the maximum height of a <see cref="RowDefinition"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the maximum height.
        /// </returns>
        public double MaxHeight
        {
            get => (double)GetValue(MaxHeightProperty);
            set => SetValue(MaxHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MaxHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(
                nameof(MaxHeight),
                typeof(double),
                typeof(RowDefinition),
                new PropertyMetadata(double.PositiveInfinity, OnMaxHeightChanged),
                FrameworkElement.IsMaxWidthHeightValid);

        private static void OnMaxHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RowDefinition)d).Parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets a value that represents the minimum allowed height of a <see cref="RowDefinition"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the minimum allowed height. The default value is 0.
        /// </returns>
        public double MinHeight
        {
            get => (double)GetValue(MinHeightProperty);
            set => SetValue(MinHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MinHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(
                nameof(MinHeight),
                typeof(double),
                typeof(RowDefinition),
                new PropertyMetadata(0d, OnMinHeightChanged),
                FrameworkElement.IsMinWidthHeightValid);

        private static void OnMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RowDefinition)d).Parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets the calculated height of a <see cref="RowDefinition"/> element,
        /// or sets the <see cref="GridLength"/> value of a row that is defined by the <see cref="RowDefinition"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="GridLength"/> that represents the height of the row. The default value is 1.0.
        /// </returns>
        public GridLength Height
        {
            get => (GridLength)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(GridLength),
                typeof(RowDefinition),
                new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), OnHeightChanged));

        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RowDefinition)d).Parent?.InvalidateMeasure();
        }

        private static readonly DependencyPropertyKey ActualHeightPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(ActualHeight),
                typeof(double),
                typeof(ColumnDefinition),
                new PropertyMetadata(0.0));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static readonly DependencyProperty ActualHeightProperty = ActualHeightPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that represents the calculated height of the <see cref="RowDefinition"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="double"/> that represents the calculated height in pixels. The default value is 0.
        /// </returns>
        public double ActualHeight
        {
            get => (double)GetValue(ActualHeightProperty);
            private set => SetValue(ActualHeightPropertyKey, value);
        }

        double IDefinitionBase.MinLength => MinHeight;

        double IDefinitionBase.MaxLength => MaxHeight;

        GridLength IDefinitionBase.Length => Height;

        double IDefinitionBase.GetUserMaxSize() => MaxHeight;

        double IDefinitionBase.GetUserMinSize() => MinHeight;

        GridUnitType IDefinitionBase.GetUserSizeType() => Height.GridUnitType;

        double IDefinitionBase.GetUserSizeValue() => Height.Value;

        void IDefinitionBase.UpdateEffectiveMinSize(double newValue) => _effectiveMinSize = Math.Max(_effectiveMinSize, newValue);

        void IDefinitionBase.SetEffectiveUnitType(GridUnitType type) => _effectiveUnitType = type;

        void IDefinitionBase.SetEffectiveMinSize(double value) => _effectiveMinSize = value;

        double IDefinitionBase.GetEffectiveMinSize() => _effectiveMinSize;

        GridUnitType IDefinitionBase.GetEffectiveUnitType() => _effectiveUnitType;

        void IDefinitionBase.SetMeasureArrangeSize(double value) => ActualHeight = value;

        double IDefinitionBase.GetMeasureArrangeSize() => ActualHeight;

        void IDefinitionBase.SetSizeCache(double value) => _sizeCache = value;

        double IDefinitionBase.GetSizeCache() => _sizeCache;

        double IDefinitionBase.GetPreferredSize()
        {
            double actualHeight = ActualHeight;
            return (_effectiveUnitType != GridUnitType.Auto && _effectiveMinSize < actualHeight)
                ? actualHeight : _effectiveMinSize;
        }

        double IDefinitionBase.GetFinalOffset() => _finalOffset;

        void IDefinitionBase.SetFinalOffset(double value) => _finalOffset = value;
    }
}
