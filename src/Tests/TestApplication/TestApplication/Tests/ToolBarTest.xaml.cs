using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class ToolBarTest : Page
    {
        private int _addedToolBarCount;
        private int _addedItemCount;

        public ToolBarTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void SetOutput(string message)
        {
            if (OutputTextBlock != null)
            {
                OutputTextBlock.Text = message;
            }
        }

        private void OnToolBarItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is ContentControl cc)
            {
                SetOutput($"Clicked: {cc.Content}");
            }
        }

        private void OnFontComboChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item)
            {
                SetOutput($"Font changed: {item.Content}");
            }
        }

        private void OnToggleOverflowClick(object sender, RoutedEventArgs e)
        {
            OverflowToolBar.IsOverflowOpen = !OverflowToolBar.IsOverflowOpen;
            SetOutput($"IsOverflowOpen set to {OverflowToolBar.IsOverflowOpen}");
        }

        private void OnLockTrayChanged(object sender, RoutedEventArgs e)
        {
            SetOutput($"LockableTray.IsLocked = {LockableTray.IsLocked} (inherited by ToolBars without a local override)");
        }

        private void OnLockToolBarChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox)
            {
                return;
            }

            ToolBar toolBar = ItemsControl.ItemsControlFromItemContainer(checkBox) as ToolBar;
            if (toolBar != null)
            {
                if (checkBox.IsChecked == true)
                {
                    // Local value overrides the value inherited from the tray.
                    ToolBarTray.SetIsLocked(toolBar, true);
                }
                else
                {
                    // Clearing the local value lets the ToolBar fall back to the inherited tray value.
                    toolBar.ClearValue(ToolBarTray.IsLockedProperty);
                }
            }

            SetOutput($"{checkBox.Tag}: local IsLocked override = {checkBox.IsChecked == true}");
        }

        private void OnOrientationRadioChecked(object sender, RoutedEventArgs e)
        {
            if (VerticalTray == null)
            {
                return;
            }

            VerticalTray.Orientation = VerticalOrientationRadio.IsChecked == true
                ? Orientation.Vertical
                : Orientation.Horizontal;
            SetOutput($"VerticalTray.Orientation = {VerticalTray.Orientation}");
        }

        private void OnAddToolBarClick(object sender, RoutedEventArgs e)
        {
            _addedToolBarCount++;

            var toolBar = new ToolBar
            {
                Band = 1,
                BandIndex = MutableTray.ToolBars.Count,
            };
            toolBar.Items.Add(new Button
            {
                Content = $"Added TB #{_addedToolBarCount}",
            });

            MutableTray.ToolBars.Add(toolBar);
            SetOutput($"Added a ToolBar. Tray now has {MutableTray.ToolBars.Count} toolbar(s).");
        }

        private void OnRemoveToolBarClick(object sender, RoutedEventArgs e)
        {
            if (MutableTray.ToolBars.Count > 0)
            {
                MutableTray.ToolBars.RemoveAt(MutableTray.ToolBars.Count - 1);
                SetOutput($"Removed a ToolBar. Tray now has {MutableTray.ToolBars.Count} toolbar(s).");
            }
            else
            {
                SetOutput("No ToolBars left to remove.");
            }
        }

        private void OnAddItemClick(object sender, RoutedEventArgs e)
        {
            if (MutableTray.ToolBars.Count == 0)
            {
                SetOutput("There is no ToolBar to add an item to.");
                return;
            }

            _addedItemCount++;

            ToolBar firstToolBar = MutableTray.ToolBars[0];
            var button = new Button
            {
                Content = $"New Item {_addedItemCount}",
            };
            button.Click += OnToolBarItemClick;
            firstToolBar.Items.Add(button);

            SetOutput($"Added an item to the first ToolBar (now {firstToolBar.Items.Count} items).");
        }
    }
}
