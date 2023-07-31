

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines column-specific properties that apply to Grid objects.
    /// </summary>
    public sealed partial class ColumnDefinition : DependencyObject, IDefinitionBase
    {
        internal Grid Parent;
        public ColumnDefinition()
        {
            _effectiveMinSize = 0;
            _effectiveUnitType = GridUnitType.Auto;
            _measureArrangeSize = 0;
            _finalOffset = 0;
            _sizeCache = 0;
        }
        
        double IDefinitionBase.MinLength { get { return MinWidth; } }
        double IDefinitionBase.MaxLength { get { return MaxWidth; } }
        GridLength IDefinitionBase.Length { get { return Width; } }
        
        private double _effectiveMinSize;
        private double _measureArrangeSize;
        private double _sizeCache;
        private double _finalOffset;

        private GridUnitType _effectiveUnitType;

        double IDefinitionBase.GetUserMaxSize()
        {
            return MaxWidth;
        }

        double IDefinitionBase.GetUserMinSize()
        {
            return MinWidth;
        }

        GridUnitType IDefinitionBase.GetUserSizeType()
        {
            return Width.GridUnitType;
        }

        double IDefinitionBase.GetUserSizeValue()
        {
            return Width.Value;
        }

        void IDefinitionBase.UpdateEffectiveMinSize(double newValue)
        {
            _effectiveMinSize = Math.Max(_effectiveMinSize, newValue);
        }

        void IDefinitionBase.SetEffectiveUnitType(GridUnitType type)
        {
            _effectiveUnitType = type;
        }
        void IDefinitionBase.SetEffectiveMinSize(double value)
        {
            _effectiveMinSize = value;
        }

        double IDefinitionBase.GetEffectiveMinSize()
        {
            return _effectiveMinSize;
        }

        GridUnitType IDefinitionBase.GetEffectiveUnitType()
        {
            return _effectiveUnitType;
        }

        void IDefinitionBase.SetMeasureArrangeSize(double value)
        {
            _measureArrangeSize = value;
        }

        double IDefinitionBase.GetMeasureArrangeSize()
        {
            return _measureArrangeSize;
        }

        void IDefinitionBase.SetSizeCache(double value)
        {
            _sizeCache = value;
        }

        double IDefinitionBase.GetSizeCache()
        {
            return _sizeCache;
        }

        double IDefinitionBase.GetPreferredSize()
        {
            return
                (_effectiveUnitType != GridUnitType.Auto
                 && _effectiveMinSize < _measureArrangeSize)
                    ? _measureArrangeSize
                    : _effectiveMinSize;
        }

        double IDefinitionBase.GetFinalOffset()
        {
            return _finalOffset;
        }

        void IDefinitionBase.SetFinalOffset(double value)
        {
            _finalOffset = value;
        }


        /// <summary>
        /// Returns a copy of the current ColumnDefinition.
        /// </summary>
        /// <returns>A copy of the current ColumnDefinition.</returns>
        public ColumnDefinition Clone()
        {
            return new ColumnDefinition()
            {
                //Parent = this.Parent,
                MinWidth = this.MinWidth,
                MaxWidth = this.MaxWidth,
                Width = this.Width.Clone()
            };
        }

        /// <summary>
        /// Gets or sets a value that represents the maximum width of a ColumnDefinition. Returns a Double that represents the maximum width in pixels. The default is PositiveInfinity.
        /// </summary>
        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaxWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                nameof(MaxWidth), 
                typeof(double), 
                typeof(ColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity, MaxWidth_Changed),
                FrameworkElement.IsMaxWidthHeightValid);

        private static void MaxWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnDefinition)d).Parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets a value that represents the minimum width of a ColumnDefinition. Returns a Double that represents the minimum width in pixels. The default is 0.
        /// </summary>
        public double MinWidth
        {
            get { return (double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                nameof(MinWidth), 
                typeof(double), 
                typeof(ColumnDefinition),
                new PropertyMetadata(0d, MinWidth_Changed),
                FrameworkElement.IsMinWidthHeightValid);

        private static void MinWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColumnDefinition)d).Parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets the calculated width of a ColumnDefinition element, or sets the GridLength value of a row that is defined by the ColumnDefinition.
        /// Returns the GridLength that represents the width of the row. The default value is 1.0
        /// </summary>
        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width), 
                typeof(GridLength), 
                typeof(ColumnDefinition),
                new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), Width_Changed));

        private static void Width_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var columnDefinition = (ColumnDefinition)d;

            columnDefinition.Parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets the rendered width of a FrameworkElement. The FrameworkElement must be in the visual tree,
        /// otherwise this property will return double.NaN.
        /// </summary>
        public double ActualWidth
        {
            get
            {
                return Parent is not null ? _measureArrangeSize : double.NaN;
            }
        }
    }
}
