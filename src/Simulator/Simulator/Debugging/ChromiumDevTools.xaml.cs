using DotNetBrowser.Browser;
using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript.Debugging
{
    /// <summary>
    /// Interaction logic for ChromiumDevTools.xaml
    /// </summary>
    public partial class ChromiumDevTools : Window
    {
        public ChromiumDevTools(string debugUrl, IBrowser browser)
        {
            InitializeComponent();

            DevToolsBrowser.InitializeFrom(browser);
            browser.Navigation.LoadUrl(debugUrl);
        }
    }
}
