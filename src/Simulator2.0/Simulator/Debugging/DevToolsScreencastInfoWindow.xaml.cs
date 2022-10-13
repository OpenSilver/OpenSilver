using System.Windows;

namespace OpenSilver.Simulator.Debugging
{
    /// <summary>
    /// Interaction logic for DevToolsScreencastInfoWindow.xaml
    /// </summary>
    public partial class DevToolsScreencastInfoWindow : Window
    {
        public DevToolsScreencastInfoWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
