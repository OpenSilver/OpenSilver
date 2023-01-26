using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace TestApplication.OpenSilver.Tests.DragDropTarget
{
    public partial class ListBoxDragDropTargetTest : Page
    {
        public ListBoxDragDropTargetTest()
        {
            InitializeComponent();
            DataElementsTab.DataContext = new DragDropViewModel();
        }
    }

    public class DragDropViewModel
    {
        private static readonly List<Item> _allItems = new List<Item>
        {
            new Item { Name = "1" },
            new Item { Name = "2" },
            new Item { Name = "3" },
            new Item { Name = "4" },
            new Item { Name = "5" },
        };

        private static readonly ObservableCollection<Item> _droppedItems = new ObservableCollection<Item> { _allItems[3], _allItems[4] };

        public CollectionViewSource SourceCollection { get; } = new CollectionViewSource { Source = _allItems };
        public CollectionViewSource TargetCollection { get; } = new CollectionViewSource { Source = _droppedItems };

        public DragDropViewModel()
        {
            FilterAvailableItems();
        }

        private void FilterAvailableItems()
        {
            SourceCollection.Filter += (s, e) =>
            {
                var item = e.Item as Item;
                e.Accepted = !_droppedItems.Any(p => p.Name == item.Name);
            };
        }

        public void OnItemDropped(object sender, Microsoft.Windows.DragEventArgs e)
        {
            e.Handled = true;

            var itemDragArgs = e.Data.GetData(typeof(ItemDragEventArgs)) as ItemDragEventArgs;
            var selections = itemDragArgs.Data as Collection<Selection>;

            foreach (var selection in selections)
            {
                if (selection.Item is Item item)
                {
                    // do not allow drop "1" item.
                    if (item.Name == "1")
                    {
                        continue;
                    }

                    _droppedItems.Add(item);
                    FilterAvailableItems();
                }
            }
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public override string ToString() => $"Item {Name}";
    }
}
