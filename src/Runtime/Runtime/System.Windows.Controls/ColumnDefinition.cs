

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
            MinWidth = 0;
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
        /// Identifies the MaxWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register(
                nameof(MaxWidth), 
                typeof(double), 
                typeof(ColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity, MaxWidth_Changed));

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
        /// Identifies the MinWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                nameof(MinWidth), 
                typeof(double), 
                typeof(ColumnDefinition),
                new PropertyMetadata(double.PositiveInfinity, MinWidth_Changed));

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

            Grid grid = columnDefinition.Parent;

            if (grid != null
                && INTERNAL_VisualTreeManager.IsElementInVisualTree(grid))
            {
                if (Grid_InternalHelpers.isCSSGridSupported())
                {
                    //-------------
                    // CSS Grid
                    //-------------

                    // We refresh all the columns:
                    Grid_InternalHelpers.RefreshAllColumnsWidth_CSSVersion(grid);
                }
                else
                {
                    //-------------
                    // Non-CSS Grid
                    //-------------

                    bool isStar = (e.OldValue != null && ((GridLength)e.OldValue).IsStar) || (e.NewValue != null && ((GridLength)e.NewValue).IsStar);
                    if (isStar)
                    {
                        // If we are dealing with a "Star" column, we need to refresh all the columns (because of the need to recalculate percentages normalization etc.):
                        Grid_InternalHelpers.RefreshAllColumnsWidth_NonCSSVersion(grid);
                    }
                    else
                    {
                        // Only refresh the current column:
                        if (grid._columnDefinitionsOrNull != null)
                        {
                            int columnIndex = grid._columnDefinitionsOrNull.IndexOf(columnDefinition);
                            bool isTheOnlyColumn = grid._columnDefinitionsOrNull == null || grid._columnDefinitionsOrNull.Count < 2;
                            Grid_InternalHelpers.RefreshColumnWidth_NonCSSVersion(grid, columnIndex, isTheOnlyColumn); //Note: we do not need to pass the normalized column definition because this method will only be called when we change the column's width without any star measurement involved (nor star before nor after).
                        }
                    }
                }
            }

            columnDefinition.Parent?.InvalidateMeasure();
        }

        #region ActualWidth / ActualHeight

        /// <summary>
        /// Gets the rendered width of a FrameworkElement. The FrameworkElement must be in the visual tree,
        /// otherwise this property will return double.NaN.
        /// </summary>
        public double ActualWidth
        {
            get
            {
                if (Parent != null)
                {
                    if (Parent.CustomLayout || Parent.IsUnderCustomLayout)
                    {
                        return _measureArrangeSize;
                    }
                    else
                    {
                        return Parent.GetColumnActualWidth(this);
                    }
                }

                return double.NaN;
            }
        }

        ///// <summary>
        ///// Gets the rendered height of a FrameworkElement. The FrameworkElement must be in the visual tree,
        ///// otherwise this property will return double.NaN.
        ///// </summary>
        //public double ActualHeight
        //{
        //    get
        //    {
        //        return Parent.GetColumnActualHeight(this);
        //    }
        //}

        #endregion


        internal Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Visibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register(
                nameof(Visibility), 
                typeof(Visibility), 
                typeof(ColumnDefinition),
                new PropertyMetadata(Visibility.Visible, Visibility_Changed));

        private static void Visibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnDefinition columnDefinition = (ColumnDefinition)d;

            if (columnDefinition.Parent != null)
            {
                Grid_InternalHelpers.RefreshColumnVisibility(columnDefinition.Parent, columnDefinition, (Visibility)e.NewValue);
            }

            columnDefinition.Parent?.InvalidateMeasure();
        }
    }
}
