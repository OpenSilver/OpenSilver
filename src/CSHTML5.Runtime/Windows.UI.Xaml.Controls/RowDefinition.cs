
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



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
    public sealed partial class RowDefinition : DependencyObject
    {
        internal Grid Parent;

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
            DependencyProperty.Register("MaxHeight", typeof(double), typeof(RowDefinition), new PropertyMetadata(double.PositiveInfinity, MaxHeight_Changed));
        static void MaxHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: set the new value in a proper manner
            var rowDefinition = (RowDefinition)d;
            double newValue = (double)e.NewValue;
            //if (INTERNAL_VisualTreeManager.IsElementInVisualTree(rowDefinition))
            //    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(rowDefinition).maxHeight = newValue + "px";
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
            DependencyProperty.Register("MinHeight", typeof(double), typeof(RowDefinition), new PropertyMetadata(double.PositiveInfinity, MinHeight_Changed));
        static void MinHeight_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: set the new value in a proper manner
            var rowDefinition = (RowDefinition)d;
            double newValue = (double)e.NewValue;
            //if (INTERNAL_VisualTreeManager.IsElementInVisualTree(rowDefinition))
            //    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(rowDefinition).minHeight = newValue + "px";
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
            DependencyProperty.Register("Height", typeof(GridLength), typeof(RowDefinition), new PropertyMetadata(new GridLength(1.0, GridUnitType.Star), Height_Changed));
        static void Height_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rowDefinition = (RowDefinition)d;
            Grid grid = rowDefinition.Parent;

            if (grid != null
                && INTERNAL_VisualTreeManager.IsElementInVisualTree(grid))
            {
                if (Grid_InternalHelpers.isCSSGridSupported())
                {
                    //-------------
                    // CSS Grid
                    //-------------

                    // We refresh all the rows:
                    Grid_InternalHelpers.RefreshAllRowsHeight_CSSVersion(grid);
                }
                else
                {
                    //-------------
                    // Non-CSS Grid
                    //-------------

                    bool isStar = (e.OldValue != null && ((GridLength)e.OldValue).IsStar) || (e.NewValue != null && ((GridLength)e.NewValue).IsStar);
                    if (isStar)
                    {
                        // If we are dealing with a "Star" row, we need to refresh all the rows (because of the need to recalculate percentages normalization etc.):
                        Grid_InternalHelpers.RefreshAllRowsHeight_NonCSSVersion(grid);
                    }
                    else
                    {
                        // Only refresh the current row:
                        if (grid._columnDefinitionsOrNull != null)
                        {
                            int rowIndex = grid._rowDefinitionsOrNull.IndexOf(rowDefinition);
                            bool isTheOnlyRow = grid._columnDefinitionsOrNull == null || grid._columnDefinitionsOrNull.Count < 2;
                            Grid_InternalHelpers.RefreshRowHeight_NonCSSVersion(grid, rowIndex, isTheOnlyRow); // Note: we do not need to pass the normalized row definition because this method will only be called when we change the row's height without any star measurement involved (nor star before nor after).
                        }
                    }

                }
            }
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
                return Parent.GetRowActualHeight(this);
            }
        }

        #endregion
    }
}
