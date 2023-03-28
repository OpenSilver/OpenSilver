

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
    /// Defines row-specific properties that apply to Grid elements.
    /// </summary>
    public sealed partial class RowDefinition : DependencyObject, IDefinitionBase
    {
        internal Grid Parent;
        public RowDefinition()
        {
            MinHeight = 0;
            _effectiveMinSize = 0;
            _effectiveUnitType = GridUnitType.Auto;
            _measureArrangeSize = 0;
            _finalOffset = 0;
            _sizeCache = 0;
        }
        double IDefinitionBase.MinLength { get { return MinHeight; } }
        double IDefinitionBase.MaxLength { get { return MaxHeight; } }
        GridLength IDefinitionBase.Length { get { return Height; } }

        private double _effectiveMinSize;
        private double _measureArrangeSize;
        private double _sizeCache;
        private double _finalOffset;

        private GridUnitType _effectiveUnitType;

        double IDefinitionBase.GetUserMaxSize()
        {
            return MaxHeight;
        }

        double IDefinitionBase.GetUserMinSize()
        {
            return MinHeight;
        }

        GridUnitType IDefinitionBase.GetUserSizeType()
        {
            return Height.GridUnitType;
        }

        double IDefinitionBase.GetUserSizeValue()
        {
            return Height.Value;
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
        /// Returns a copy of this RowDefinition.
        /// </summary>
        /// <returns>A copy of this RowDefinition.</returns>
        public RowDefinition Clone()
        {
            return new RowDefinition()
            {
                //Parent = this.Parent,
                MinHeight = this.MinHeight,
                MaxHeight = this.MaxHeight,
                Height = this.Height.Clone()
            };
        }

        /// <summary>
        /// Gets or sets a value that represents the maximum height of a RowDefinition. Returns a Double that represents the maximum height in pixels. The default is PositiveInfinity.
        /// </summary>
        public double MaxHeight
        {
            get { return (double)GetValue(MaxHeightProperty); }
            set { SetValue(MaxHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the MaxHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.Register(
                nameof(MaxHeight), 
                typeof(double), 
                typeof(RowDefinition),
                new PropertyMetadata(double.PositiveInfinity, MaxHeight_Changed));

        private static void MaxHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RowDefinition)d).Parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets a value that represents the minimum height of a RowDefinition. Returns a Double that represents the minimum height in pixels. The default is 0.
        /// </summary>
        public double MinHeight
        {
            get { return (double)GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the MinHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(
                nameof(MinHeight), 
                typeof(double), 
                typeof(RowDefinition),
                new PropertyMetadata(double.PositiveInfinity, MinHeight_Changed));

        private static void MinHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RowDefinition)d).Parent?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets the calculated height of a RowDefinition element, or sets the GridLength value of a row that is defined by the RowDefinition.
        /// Returns the GridLength that represents the height of the row. The default value is 1.0
        /// </summary>
        public GridLength Height
        {
            get { return (GridLength)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height), 
                typeof(GridLength), 
                typeof(RowDefinition),
                new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), Height_Changed));

        private static void Height_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rowDefinition = (RowDefinition)d;
            Grid grid = rowDefinition.Parent;

            if (grid != null
                && INTERNAL_VisualTreeManager.IsElementInVisualTree(grid))
            {
                //-------------
                // CSS Grid
                //-------------

                // We refresh all the rows:
                Grid_InternalHelpers.RefreshAllRowsHeight_CSSVersion(grid);
            }

            rowDefinition.Parent?.InvalidateMeasure();
        }


        #region ActualWidth / ActualHeight

        ///// <summary>
        ///// Gets the rendered width of a FrameworkElement. The FrameworkElement must be in the visual tree,
        ///// otherwise this property will return double.NaN.
        ///// </summary>
        //public double ActualWidth
        //{
        //    get
        //    {
        //        return Parent.GetRowActualWidth(this);
        //    }
        //}

        /// <summary>
        /// Gets the rendered height of a FrameworkElement. The FrameworkElement must be in the visual tree,
        /// otherwise this property will return double.NaN.
        /// </summary>
        public double ActualHeight
        {
            get
            {
                if (Parent != null)
                {
                    if (Parent.UseCustomLayout)
                    {
                        return _measureArrangeSize;
                    }
                    else
                    {
                        return Parent.GetRowActualHeight(this);
                    }
                }

                return double.NaN;
            }
        }

        #endregion
    }
}
