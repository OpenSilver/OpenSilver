using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DotNetForHtml5.LocalServer
{
    public partial class PageWhenConnected : UserControl
    {
        string _url;
        bool _useLocalhost;

        public PageWhenConnected(string url, bool useLocalhost)
        {
            InitializeComponent();

            _url = url;
            _useLocalhost = useLocalhost;

            UrlTextBox.Text = _url;

            if (useLocalhost)
            {
                TextWhenRunningFromLocalhost.Visibility = Visibility.Visible;
                TextWhenTestingOnMobileDevice.Visibility = Visibility.Collapsed;
                ButtonTroubleshooting.Visibility = Visibility.Collapsed;

                // Automatically launch the app in the default web browser:
                OpenUri(_url);
            }
            else
            {
                TextWhenRunningFromLocalhost.Visibility = Visibility.Collapsed;
                TextWhenTestingOnMobileDevice.Visibility = Visibility.Visible;
                ButtonTroubleshooting.Visibility = Visibility.Visible;
            }
        }

        void ButtonTroubleshooting_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(String.Format(@"If you encounter any issues, please check the following:

- Make sure that your mobile device and your computer are connected to the same local network.

- Make sure that your Windows Firewall is set to allow incoming connections to {0}
If you are unsure, please try to temporarily disable your Windows Firewall. Note: C#/XAML for HTML5 is supposed to automatically create a new Firewall rule named ""CSHTML5 - Allow port ..."". You can see/add/remove the firewall rules by going to ""Control Panel"" -> ""Windows Firewall"" -> ""Advanced Settings"" -> ""Inbound Rules"".

- Contact support at: support@cshtml5.com
", _url));
        }

        static bool OpenUri(string uri)
        {
            // For safety (to avoid executing unwanted code), we check if the text is a valid URI:
            if (!IsValidUri(uri))
                return false;
            System.Diagnostics.Process.Start(uri);
            return true;
        }

        static bool IsValidUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            Uri tmp;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }
    }
}
