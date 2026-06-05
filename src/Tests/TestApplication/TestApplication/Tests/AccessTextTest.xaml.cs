using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace TestApplication.Tests
{
    public partial class AccessTextTest : Page
    {
        private const string ManualKey = "G";

        private readonly ObservableCollection<string> _log = new ObservableCollection<string>();
        private bool _manualKeyRegistered;

        public AccessTextTest()
        {
            InitializeComponent();

            LogList.ItemsSource = _log;

            ShowAccessKey(At1, At1Key);
            ShowAccessKey(At2, At2Key);
            ShowAccessKey(At3, At3Key);

            AccessKeyManager.Register(ManualKey, GoButton);
            _manualKeyRegistered = true;
            UpdateRegistrationStatus();

            var defaultedWatcher = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            defaultedWatcher.Tick += (_, __) =>
                DefaultedStatus.Text = $"DefBtn.IsDefaulted = {DefBtn.IsDefaulted}";
            defaultedWatcher.Start();
        }

        private static void ShowAccessKey(AccessText source, TextBlock display)
        {
            char key = source.AccessKey;
            display.Text = key == '\0'
                ? "no access key (escaped or none)"
                : $"AccessKey = '{key}'";
        }

        private void Demo_Click(object sender, RoutedEventArgs e)
        {
            string label = (sender as FrameworkElement)?.Tag as string ?? "(unknown)";
            Log($"activated: {label}");
        }

        private void ProcessKey_Click(object sender, RoutedEventArgs e)
        {
            string key = (string)((FrameworkElement)sender).Tag;
            bool moreMatches = AccessKeyManager.ProcessKey(null, key, false);
            Log($"ProcessKey('{key}') -> moreMatches={moreMatches}");
        }

        private void ToggleRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (_manualKeyRegistered)
            {
                AccessKeyManager.Unregister(ManualKey, GoButton);
                _manualKeyRegistered = false;
                Log($"Unregister('{ManualKey}')");
            }
            else
            {
                AccessKeyManager.Register(ManualKey, GoButton);
                _manualKeyRegistered = true;
                Log($"Register('{ManualKey}')");
            }

            UpdateRegistrationStatus();
        }

        private void UpdateRegistrationStatus()
        {
            bool registered = AccessKeyManager.IsKeyRegistered(null, ManualKey);
            RegStatus.Text = $"IsKeyRegistered(null, \"{ManualKey}\") = {registered}";
            ToggleRegButton.Content = _manualKeyRegistered ? $"Unregister '{ManualKey}'" : $"Register '{ManualKey}'";
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e) => _log.Clear();

        private void Log(string message) => _log.Insert(0, $"{DateTime.Now:HH:mm:ss}  {message}");
    }
}
