using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript.Debugging
{
    /// <summary>
    /// Interaction logic for ChromiumDevTools.xaml
    /// </summary>
    public partial class ChromiumDevTools : Window
    {
        public ChromiumDevTools(string debugUrl)
        {
            InitializeComponent();

            DevToolsBrowser.Browser.LoadURL(debugUrl);
        }
    }
}
