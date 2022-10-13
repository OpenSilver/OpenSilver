using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xaml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Net;


namespace DotNetForHtml5.LocalServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Website _website;
        string _firewallRuleName;
        SimpleHTTPServer _simpleHttpServer;
        string _url;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            RemoveFirewallRuleIfAny(); // Note: must be done before stopping web server otherwise _website is null.
            StopWebServerIfAny();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize:
            _url = null;
            _website = null;
            _simpleHttpServer = null;
            _firewallRuleName = null;

            // Show the "Loading..." page:
            this.MainContainer.Child = new PageWhenLoading();

            // Read the input parameters:
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 3)
            {
                string localhostOrNotArgument = args[1];
                string outputPathAbsolute = args[2].Trim('"');

                // Determine if we should use "localhost" or the machine IP:
                bool useLocalhost = (localhostOrNotArgument == "uselocalhost" ? true : false);

                // Determine the IP address to use:
                string ipAddressToUse = null;
                string exceptionMessage = null;
                bool cancel = false;
                if (useLocalhost)
                {
                    ipAddressToUse = "localhost";
                }
                else
                {
                    try
                    {
                        ipAddressToUse = IPHelper.GetLocalIPAddress();
                    }
                    catch (Exception ex)
                    {
                        exceptionMessage = "Error while attempting to determine the machine IP address:\r\n\r\n" + ex.Message;
                    }
                }

                // Start the web server:
                if (ipAddressToUse != null)
                {
                    _url = null;
                    try
                    {
                        //create server with given port:
                        if (useLocalhost)
                        {
                            _simpleHttpServer = new SimpleHTTPServer(outputPathAbsolute);
                            _url = "http://" + ipAddressToUse + ":" + _simpleHttpServer.Port.ToString() + "/";
                        }
                        else
                        {
                            //var ipByteArray = IPAddress.Parse(ipAddressToUse).GetAddressBytes();
                            //_simpleHttpServer = new SimpleHTTPServer(outputDir, ipByteArray);

                            _website = Website.Create(outputPathAbsolute, ip: ipAddressToUse);
                            _website.Start();
                            _url = _website.Url;
                            if (!string.IsNullOrEmpty(ipAddressToUse))
                                _url = _url.Replace("localhost", ipAddressToUse).Replace("127.0.0.1", ipAddressToUse);
                        }


                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("REGDB_E_CLASSNOTREG"))
                        {
                            cancel = true;
                            _website = null;
                            _simpleHttpServer = null;
                            this.MainContainer.Child = new PageWhenIISNotInstalled();
                        }
                        else
                        {
                            exceptionMessage = "Error while attempting to start the web server:\r\n\r\n" + ex.Message;
                        }
                    }

                    if (_url != null)
                    {
                        // Display the OK window:
                        this.MainContainer.Child = new PageWhenConnected(_url, useLocalhost);

                        // Attempt to open the firewall:
                        // Note: we use the "Dispatcher" as a "DoEvents" way so that the window appears before any possible error message box related to Windows Firewall.
                        Dispatcher.BeginInvoke((Action)(() =>
                            {
                                try
                                {
                                    _firewallRuleName = string.Format("CSHTML5 - Allow port {0}", this.GetPort());
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = "netsh",
                                        Arguments = string.Format("advfirewall firewall add rule name=\"{0}\" dir=in protocol=tcp localport={1} remoteip=localsubnet action=allow", _firewallRuleName, this.GetPort()),
                                        Verb = "runas",
                                        UseShellExecute = true,
                                        WindowStyle = ProcessWindowStyle.Normal
                                    }).WaitForExit();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(
                                        string.Format(
                                        "We were unable to automatically configure the Windows Firewall for you.\r\n\r\nTo allow your mobile device to connect to this computer, please temporarily disable the Windows Firewall, or manually set the Windows Firewall to allow incoming connections to: {0}\r\n\r\nIf the problem persists, please report it to support@cshtml5.com\r\n\r\n------------------------------\r\nException details:\r\n" + ex.Message,
                                        _url));
                                }

                                //// Ensure access rights from other machines to the URL:
                                //if (_url != null)
                                //{
                                //    try
                                //    {
                                //        NetAclChecker.AddAddress(_url);
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        MessageBox.Show("Error while attempting to give access rights to the new web server URL: " + url + "\r\n\r\nException details:\r\n\r\n" + ex.Message);
                                //    }
                                //}
                            }));

                    }
                    else
                    {
                        if (!cancel)
                        {
                            MessageBox.Show(exceptionMessage ?? "Unable to start the web server. Please report this issue to support@cshtml5.com");
                            Quit();
                        }
                    }
                }
                else
                {
                    MessageBox.Show(exceptionMessage ?? "Unable to determine the machine IP address. Please report this issue to support@cshtml5.com");
                    Quit();
                }
            }
            else
            {
                MessageBox.Show("Invalid startup parameters. Please report this issue to support@cshtml5.com");
                Quit();
            }
        }

        void StopWebServerIfAny()
        {
            if (_website != null)
            {
                try
                {
                    _website.Stop();
                }
                catch
                {
                    //Ignored...

                    //MessageBox.Show("Error while attempting to stop the web server:\r\n\r\n" + ex.Message);
                }
                _website = null;
            }

            if (_simpleHttpServer != null)
            {
                try
                {
                    _simpleHttpServer.Stop();
                }
                catch
                {
                    //Ignored...

                    //MessageBox.Show("Error while attempting to stop the web server:\r\n\r\n" + ex.Message);
                }
                _simpleHttpServer = null;
            }

            this.MainContainer.Child = null;

        }

        void RemoveFirewallRuleIfAny()
        {
            if ((_simpleHttpServer != null || _website != null) && _firewallRuleName != null)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = string.Format("advfirewall firewall delete rule name=\"{0}\"", _firewallRuleName),
                        Verb = "runas",
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    }).WaitForExit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(
                        "We were unable to automatically remove the Windows Firewall rule that allows incoming connections to {0}\r\n\r\nFor best security, it is recommended that you manually remove it. To do so, please go to Control Panel -> Windows Firewall -> Advanced Settings -> Inbound Rules, select the rule named \"{1}\", and then click Delete.\r\n\r\nIf the problem persists, please report it to support@cshtml5.com\r\n\r\n------------------------------\r\nException details:\r\n" + ex.Message,
                        _url ?? "(n/a)", _firewallRuleName));
                }
            }
        }

        public void Quit()
        {
            this.MainContainer.Child = null;
            RemoveFirewallRuleIfAny(); // Note: must be done before stopping web server otherwise _website is null.
            StopWebServerIfAny();
            Application.Current.Shutdown();
        }

        string GetPort()
        {
            if (_website != null)
                return _website.Port.ToString();
            else if (_simpleHttpServer != null)
                return _simpleHttpServer.Port.ToString();
            else
            {
                string error = "Error in GetPort(): cannot get the Port because no server was started. Please report this error to support@cshtml5.com";
                MessageBox.Show(error);
                throw new Exception(error);
            }
        }
    }
}
