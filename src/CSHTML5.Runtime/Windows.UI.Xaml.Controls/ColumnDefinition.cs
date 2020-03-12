

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
    public sealed partial class ColumnDefinition : DependencyObject
    {
        internal Grid Parent;

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
            DependencyProperty.Register("MaxWidth", typeof(double), typeof(ColumnDefinition), new PropertyMetadata(double.PositiveInfinity, MaxWidth_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        static void MaxWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: set the new value in a proper manner
            var rowDefinition = (ColumnDefinition)d;
            double newValue = (double)e.NewValue;
            //if (INTERNAL_VisualTreeManager.IsElementInVisualTree(rowDefinition))
            //        INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(rowDefinition).maxWidth = newValue + "px";
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
            DependencyProperty.Register("MinWidth", typeof(double), typeof(ColumnDefinition), new PropertyMetadata(double.PositiveInfinity, MinWidth_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        static void MinWidth_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: set the new value in a proper manner
            var rowDefinition = (ColumnDefinition)d;
            double newValue = (double)e.NewValue;
            //if (INTERNAL_VisualTreeManager.IsElementInVisualTree(rowDefinition))
            //    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(rowDefinition).minWidth = newValue + "px";
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
            DependencyProperty.Register("Width", typeof(GridLength), typeof(ColumnDefinition), new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), Width_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void Width_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
                return Parent.GetColumnActualWidth(this);
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
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(ColumnDefinition), new PropertyMetadata(Visibility.Visible, Visibility_Changed));

        private static void Visibility_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnDefinition columnDefinition = (ColumnDefinition)d;
            if (columnDefinition.Parent != null)
            {
                Grid_InternalHelpers.RefreshColumnVisibility(columnDefinition.Parent, columnDefinition, (Visibility)e.NewValue);
            }
        }
    }
}
