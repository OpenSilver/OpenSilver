using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [ContentProperty("Items")]
    public class TileView : Control
    {
        //Notes: We consider that the minimized items go on the right side of the grid, this should change at some point
        //       We put the first item as Maximized, we need to deal with TileViewItem.IsMaximized as an improvement.
        //       I'd say we should have VisualStates for Minimized and Maximized for the TileViewItems and this control tells them to GoToState
        //       Should we do like ItemsControl and have an ItemsSource to allows adding a whole bunch of items programmatically ?
        ObservableCollection<TileViewItem> _items;
        TileViewItem _maximizedTile = null;
        Grid _contentGrid = null; //Note: this is intended to be solely used to display the Items, nothing else that mighrt have been added by the user in the template.

        public TileView()
        {
            this.DefaultStyleKey = typeof(TileView);
        }

        public ObservableCollection<TileViewItem> Items
        {
            get
            {
                if (this._items == null)
                {
                    this._items = new ObservableCollection<TileViewItem>();
                    this._items.CollectionChanged += items_CollectionChanged;
                }
                return this._items;
            }
        }

        private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //todo: make the corresponding changes. I guess we don't care what the changes are as long as we're not in the Visual tree.
        }

        private void AddItemsToVisualTree(ObservableCollection<TileViewItem> oldItems, ObservableCollection<TileViewItem> newItems)
        {
            //Note: this method initializes the grid that will contain the children and adds them (the children being the elements in newItems)
            //todo: rename/clean up/change this method so it reflects what it actually does.
            if (_contentGrid != null)
            {
                //clear the grid's rows and columns (just in case... if the user wants to add rows and columns, they need to add another grid around this one)
                if (_contentGrid.ColumnDefinitions == null || _contentGrid.ColumnDefinitions.Count != 2)
                {
                    _contentGrid.ColumnDefinitions.Clear();
                    _contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    _contentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                }
                if (_contentGrid.RowDefinitions == null || _contentGrid.RowDefinitions.Count != 0)
                {
                    _contentGrid.RowDefinitions.Clear();
                }
                if (oldItems != newItems)
                {
                    if (oldItems != null) // Note: this part is currently useless since we only call this method in OnApplyTemplate
                    {
                        //remove the former elements if they were in the tree:
                        if (_contentGrid != null)
                        {
                            foreach (TileViewItem oldItem in oldItems)
                            {
                                _contentGrid.Children.Remove(oldItem);
                            }
                        }
                    }
                    if (newItems != null)
                    {
                        //todo: deal with an eventual item.IsMaximized or something like that.
                        bool isFirst = true;
                        int n = 0;
                        foreach (var item in newItems)
                        {
                            if (!isFirst)
                            {
                                Grid.SetColumn(item, 1);
                                Grid.SetRow(item, n);
                                ++n;
                            }
                            else
                            {
                                Grid.SetRowSpan(item, int.MaxValue); //Note: this might not be good enough when we add an additional item since if I remember correctly, we change the value put in the html due to the fact that the css grid automatically adds rows when an Item.row is bigger (which we don't want) so we probably also do it for RowSpan. 
                                item.Maximize();
                                _maximizedTile = item;
                            }
                            _contentGrid.Children.Add(item);
                            item._TileViewParent = this;
                            isFirst = false;
                        }

                        int rowsToAdd = oldItems != null ? newItems.Count - oldItems.Count : newItems.Count;
                        if (rowsToAdd > 0)
                        {
                            for (int i = 0; i < rowsToAdd; ++i)
                            {
                                _contentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                            }
                        }
                        else
                        {
                            for (int i = 0; i > rowsToAdd; --i)
                            {
                                _contentGrid.RowDefinitions.RemoveAt(_contentGrid.RowDefinitions.Count);
                            }
                        }
                    }
                }
            }
        }


        internal void MaximizeTile(TileViewItem tileToMaximize)
        {
            //we check if the tile asking to be maximized is not the one already maximized:
            if (tileToMaximize != _maximizedTile)
            {
                //we switch the Grid.Row and Column between those tiles and tell them that they are maximized/minimized:
                if (_maximizedTile != null)
                {
                    int maximizedTileRow = Grid.GetRow(_maximizedTile);
                    int maximizedTileColumn = Grid.GetColumn(_maximizedTile);
                    Grid.SetRow(_maximizedTile, Grid.GetRow(tileToMaximize));
                    Grid.SetColumn(_maximizedTile, Grid.GetColumn(tileToMaximize));
                    Grid.SetRowSpan(tileToMaximize, Grid.GetRowSpan(_maximizedTile));
                    Grid.SetRow(tileToMaximize, maximizedTileRow);
                    Grid.SetColumn(tileToMaximize, maximizedTileColumn);
                    Grid.SetRowSpan(_maximizedTile, 1);
                }
                else
                {
                    Grid.SetRow(tileToMaximize, 0);
                    Grid.SetColumn(tileToMaximize, 0);
                    Grid.SetRowSpan(tileToMaximize, int.MaxValue);
                }
                _maximizedTile.Minimize();
                tileToMaximize.Maximize();
                _maximizedTile = tileToMaximize;
            }
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate() 
#endif
        {
            base.OnApplyTemplate();
            _contentGrid = GetTemplateChild("PART_ItemsDisplayGrid") as Grid;
            AddItemsToVisualTree(null, Items);
        }
    }
}
