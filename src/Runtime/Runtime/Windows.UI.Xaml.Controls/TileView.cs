
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
using System.Diagnostics;

#if MIGRATION
using System.Windows.Data;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public class TileView : Selector
    {
        // Notes: We consider that the minimized items go on the right side of the grid, this should change at some point
        //        We put the first item as Maximized, we need to deal with TileViewItem.IsMaximized as an improvement.
        //        I'd say we should have VisualStates for Minimized and Maximized for the TileViewItems and this control tells them to GoToState
        //        Should we do like ItemsControl and have an ItemsSource to allows adding a whole bunch of items programmatically ?
        private TileViewItem _maximizedTile = null;

        public TileView()
        {
            this.DefaultStyleKey = typeof(TileView);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TileViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TileViewItem;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            TileViewItem tile = element as TileViewItem;
            if (tile != null)
            {
                tile._tileViewParent = null;
                tile.ClearValue(Grid.RowProperty);
                tile.ClearValue(Grid.ColumnProperty);
                tile.ClearValue(Grid.RowSpanProperty);
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            
            TileViewItem container = element as TileViewItem;
            if (container != null)
            {
                container._tileViewParent = this;
            }
        }

        internal void MaximizeTile(TileViewItem tileToMaximize)
        {
            //we check if the tile asking to be maximized is not the one already maximized:
            if (tileToMaximize != this._maximizedTile)
            {
                // we switch the Grid.Row and Column between those tiles and tell
                // them that they are maximized/minimized
                if (this._maximizedTile != null)
                {
                    Grid.SetRow(this._maximizedTile, Grid.GetRow(tileToMaximize));
                    Grid.SetColumn(this._maximizedTile, Grid.GetColumn(tileToMaximize));
                    Grid.SetRowSpan(this._maximizedTile, 1);
                    this._maximizedTile.Minimize();
                }

                Grid.SetRow(tileToMaximize, 0);
                Grid.SetColumn(tileToMaximize, 0);
                Grid.SetRowSpan(tileToMaximize, int.MaxValue);
                
                tileToMaximize.Maximize();
                this._maximizedTile = tileToMaximize;
            }
        }

        public GridLength MinimizedColumnWidth
        {
            get { return (GridLength)GetValue(MinimizedColumnWidthProperty); }
            set { SetValue(MinimizedColumnWidthProperty, value); }
        }

        public static readonly DependencyProperty MinimizedColumnWidthProperty =
            DependencyProperty.Register(
                "MinimizedColumnWidth", 
                typeof(GridLength), 
                typeof(TileView), 
                new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

        internal TileViewItem MaximizedTile
        {
            get { return this._maximizedTile; }
        }
    }

    /// <summary>
    /// This panel is meant to be used to display a <see cref="TileView"/> children.
    /// </summary>
    public class TileViewPanel : Panel
    {
        private TileView _owner;
        private GridNotLogical _contentGrid;

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            // we need the TileView to be set before calling OnChildrenReset,
            // which is done in the base implementation.
            this._owner = ItemsControl.GetItemsOwner(this) as TileView;

            if (this._contentGrid == null)
            {
                var grid = new GridNotLogical();
                var column1 = new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) };
                var column2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column1);
                grid.ColumnDefinitions.Add(column2);
                this._contentGrid = grid;
            }

            BindingOperations.SetBinding(
                this._contentGrid.ColumnDefinitions[1], 
                ColumnDefinition.WidthProperty, 
                new Binding("MinimizedColumnWidth") { Source = this._owner });

            base.INTERNAL_OnAttachedToVisualTree();

            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(this._contentGrid, this, 0);
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            this._owner = null;
            this._contentGrid = null;
        }

        internal override void OnChildrenAdded(UIElement newChild, int index)
        {
            if (this._contentGrid != null)
            {
                this._contentGrid.Children.Insert(index, newChild);
                this.ArrangeInternal();
            }
        }

        internal override void OnChildrenRemoved(UIElement oldChild, int index)
        {
            if (this._contentGrid != null)
            {
                Debug.Assert(this._contentGrid.Children[index] == oldChild);
                this._contentGrid.Children.RemoveAt(index);
                this.ArrangeInternal();
            }
        }

        internal override void OnChildrenReplaced(UIElement oldChild, UIElement newChild, int index)
        {
            if (this._contentGrid != null)
            {
                Debug.Assert(this._contentGrid.Children[index] == oldChild);
                this._contentGrid.Children[index] = newChild;
                this.ArrangeInternal();
            }
        }

        internal override void OnChildrenReset()
        {
            if (this._contentGrid != null)
            {
                this._contentGrid.Children.Clear();

                if (this.HasChildren)
                {
                    foreach (var child in this.Children)
                    {
                        this._contentGrid.Children.Add(child);
                    }
                    this.ArrangeInternal();
                }
            }
        }

        private void ArrangeInternal()
        {
            int count = this.Children.Count;

            this._contentGrid.RowDefinitions.Clear();

            if (count == 0)
            {
                return;
            }
            
            int rowsCount = Math.Max(1, count - 1);
            for (int i = 0; i < rowsCount; ++i)
            {
                this._contentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            // First get the index of the maximized tile.
            int maximizedTileIndex = 0;
            for (int i = 0; i < count; ++i)
            {
                if ((TileViewItem)this.Children[i] == this._owner.MaximizedTile)
                {
                    maximizedTileIndex = i;
                    break;
                }
            }

            TileViewItem child;
            for (int i = 0; i < count; ++i)
            {
                child = (TileViewItem)this.Children[i];

                if (i == maximizedTileIndex)
                {
                    Grid.SetRow(child, 0);
                    Grid.SetRowSpan(child, int.MaxValue);
                    Grid.SetColumn(child, 0);
                }
                else if (i == 0)
                {
                    Grid.SetRow(child, maximizedTileIndex);
                    Grid.SetRowSpan(child, 1);
                    Grid.SetColumn(child, 1);
                }
                else
                {
                    Grid.SetRow(child, i - 1);
                    Grid.SetRowSpan(child, 1);
                    Grid.SetColumn(child, 1);
                }
            }

            if (this._owner.MaximizedTile == null)
            {
                this._owner.MaximizeTile((TileViewItem)this.Children[maximizedTileIndex]);
            }
        }
    }
}
