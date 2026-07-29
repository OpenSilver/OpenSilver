using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace TestApplication.Tests
{
    public partial class StatusBarTest : Page
    {
        private readonly ObservableCollection<string> _notifications = new ObservableCollection<string>();
        private readonly ObservableCollection<object> _mixedEntries = new ObservableCollection<object>();
        private int _notificationCount;
        private int _addedItemCount = 1;
        private int _mixedEntryCount;
        private DispatcherTimer _clockTimer;

        public StatusBarTest()
        {
            InitializeComponent();

            NotificationsStatusBar.ItemsSource = _notifications;
            OnAddNotificationClick(this, null);
            OnAddNotificationClick(this, null);

            MixedStatusBar.UsesItemContainerTemplate = true;
            MixedStatusBar.ItemContainerTemplateSelector = new MixedStatusBarItemTemplateSelector(
                (DataTemplate)Resources["MixedStatusItemTemplate"],
                (DataTemplate)Resources["MixedSeparatorTemplate"]);
            MixedStatusBar.ItemsSource = _mixedEntries;

            _mixedEntries.Add("CPU: 12%");
            _mixedEntries.Add(new SeparatorPlaceholder());
            _mixedEntries.Add("Battery: 87%");
            _mixedEntries.Add(new SeparatorPlaceholder());
            _mixedEntries.Add("Wi-Fi: Connected");
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_clockTimer == null)
            {
                _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                _clockTimer.Tick += (_, __) => UpdateClock();
                _clockTimer.Start();
                UpdateClock();
            }
        }

        private void SetOutput(string message)
        {
            if (OutputTextBlock != null)
            {
                OutputTextBlock.Text = message;
            }
        }

        private void UpdateClock()
        {
            if (ClockTextBlock != null)
            {
                ClockTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
            }

            if (FillTextBlock != null)
            {
                FillTextBlock.Text = $"(this item is last, so it fills the remaining space) — updated at {DateTime.Now:HH:mm:ss}";
            }
        }

        // ---------------------------------------------------------------
        // SECTION 3: Interactive content
        // ---------------------------------------------------------------

        private void OnCapsLockChanged(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggle)
            {
                bool isOn = toggle.IsChecked == true;
                toggle.Content = $"Caps Lock: {(isOn ? "On" : "Off")}";
                SetOutput($"CapsLockToggle.IsChecked = {isOn}");
            }
        }

        private void OnProgressSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TaskProgressBar != null)
            {
                TaskProgressBar.Value = e.NewValue;
            }
        }

        private void OnResetProgressClick(object sender, RoutedEventArgs e)
        {
            ProgressSlider.Value = 0;
            TaskProgressBar.Value = 0;
            SetOutput("Progress reset to 0.");
        }

        // ---------------------------------------------------------------
        // SECTION 4: Data-bound StatusBar (ItemsSource / ItemTemplate / ItemContainerStyle)
        // ---------------------------------------------------------------

        private void OnAddNotificationClick(object sender, RoutedEventArgs e)
        {
            _notificationCount++;
            _notifications.Add($"Notification #{_notificationCount} at {DateTime.Now:HH:mm:ss}");
            SetOutput($"Added notification #{_notificationCount}.");
        }

        private void OnClearNotificationsClick(object sender, RoutedEventArgs e)
        {
            _notifications.Clear();
            SetOutput("Cleared all notifications.");
        }

        // ---------------------------------------------------------------
        // SECTION 5: Runtime mutation of StatusBar.Items
        // ---------------------------------------------------------------

        private void OnAddItemClick(object sender, RoutedEventArgs e)
        {
            _addedItemCount++;
            MutableStatusBar.Items.Add(new StatusBarItem { Content = $"Item {_addedItemCount}" });
            SetOutput($"Added Item {_addedItemCount}. MutableStatusBar now has {MutableStatusBar.Items.Count} item(s).");
        }

        private void OnAddSeparatorClick(object sender, RoutedEventArgs e)
        {
            MutableStatusBar.Items.Add(new Separator());
            SetOutput($"Added a Separator. MutableStatusBar now has {MutableStatusBar.Items.Count} item(s).");
        }

        private void OnRemoveLastClick(object sender, RoutedEventArgs e)
        {
            if (MutableStatusBar.Items.Count > 0)
            {
                MutableStatusBar.Items.RemoveAt(MutableStatusBar.Items.Count - 1);
                SetOutput($"Removed last item. MutableStatusBar now has {MutableStatusBar.Items.Count} item(s).");
            }
            else
            {
                SetOutput("No items left to remove.");
            }
        }

        private void OnClearItemsClick(object sender, RoutedEventArgs e)
        {
            MutableStatusBar.Items.Clear();
            _addedItemCount = 0;
            SetOutput("Cleared all items from MutableStatusBar.");
        }

        // ---------------------------------------------------------------
        // SECTION 6: UsesItemContainerTemplate / ItemContainerTemplateSelector
        // ---------------------------------------------------------------

        private void OnAddMixedEntryClick(object sender, RoutedEventArgs e)
        {
            _mixedEntryCount++;
            _mixedEntries.Add($"Custom entry #{_mixedEntryCount} at {DateTime.Now:HH:mm:ss}");
            SetOutput("Added a plain string item — the selector produced a StatusBarItem container for it.");
        }

        private void OnAddMixedSeparatorClick(object sender, RoutedEventArgs e)
        {
            _mixedEntries.Add(new SeparatorPlaceholder());
            SetOutput("Added a SeparatorPlaceholder item — the selector produced a real Separator container for it.");
        }

        private void OnClearMixedEntriesClick(object sender, RoutedEventArgs e)
        {
            _mixedEntries.Clear();
            SetOutput("Cleared MixedStatusBar's data-bound collection.");
        }

        /// <summary>
        /// A plain data marker (not a UI element) used in <see cref="_mixedEntries"/> to indicate
        /// where a real <see cref="Separator"/> container should be generated.
        /// </summary>
        private sealed class SeparatorPlaceholder
        {
        }

        /// <summary>
        /// Selects the container template based on the kind of data item: a
        /// <see cref="SeparatorPlaceholder"/> gets a template rooted in a <see cref="Separator"/>,
        /// anything else gets a template rooted in a <see cref="StatusBarItem"/>.
        /// </summary>
        private sealed class MixedStatusBarItemTemplateSelector : ItemContainerTemplateSelector
        {
            private readonly DataTemplate _statusItemTemplate;
            private readonly DataTemplate _separatorTemplate;

            public MixedStatusBarItemTemplateSelector(DataTemplate statusItemTemplate, DataTemplate separatorTemplate)
            {
                _statusItemTemplate = statusItemTemplate;
                _separatorTemplate = separatorTemplate;
            }

            public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl) =>
                item is SeparatorPlaceholder ? _separatorTemplate : _statusItemTemplate;
        }
    }
}
