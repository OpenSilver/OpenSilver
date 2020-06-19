using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace DotNetForHtml5.LocalServer
{
    public partial class PageWhenIISNotInstalled : UserControl
    {
        string _url;

        public PageWhenIISNotInstalled()
        {
            InitializeComponent();
        }

        void ButtonTroubleshooting_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"To create a local web server, C#/XAML for HTML5 requires IIS (Internet Information Services). It is a Windows feature provided by Microsoft that can easily be turned on or off from the Control Panel. It is used by C#/XAML for HTML5 to create a private URL such as http://192.168.1.x:xxxxx/, so that you can easily test your app from Localhost, or from a smartphone or tablet by opening the mobile web browser and navigating to that URL.

-------------------------------------
What does the button 'Enable IIS' do?
-------------------------------------
-> Clicking the button 'Enable IIS' will attempt to turn the following Windows features on:
     - IIS-ApplicationDevelopment
     - IIS-CommonHttpFeatures
     - IIS-DefaultDocument
     - IIS-ISAPIExtensions
     - IIS-ISAPIFilter
     - IIS-ManagementConsole
     - IIS-NetFxExtensibility
     - IIS-RequestFiltering
     - IIS-Security
     - IIS-StaticContent
     - IIS-WebServer
     - IIS-WebServerRole

-------------------------------
It doesn't work. What can I do?
-------------------------------
If the button doesn't work, please manually turn those features on by going to ""Control Panel"" --> ""Programs and Features"" --> ""Turn Windows features on or off"", then checking the ""Internet Information Services"" box and clicking OK.

You may need to reboot for the changes to take effect.

If you still encounter any issues, please contact us at: support@cshtml5.com
");
        }

        void ButtonEnableIIS_Click(object sender, RoutedEventArgs e)
        {
            // Credits: http://stackoverflow.com/questions/16079030/better-way-to-install-iis7-programmatically
            try
            {
                var featureNames = new[] 
            {
                "IIS-ApplicationDevelopment",
                "IIS-CommonHttpFeatures",
                "IIS-DefaultDocument",
                "IIS-ISAPIExtensions",
                "IIS-ISAPIFilter",
                "IIS-ManagementConsole",
                "IIS-NetFxExtensibility",
                "IIS-RequestFiltering",
                "IIS-Security",
                "IIS-StaticContent",
                "IIS-WebServer",
                "IIS-WebServerRole",
            };

                string currentAssemblyDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                string pathToBatchToInstallIIS = Path.Combine(currentAssemblyDirectory, "Install_IIS.bat");

                if (File.Exists(pathToBatchToInstallIIS))
                {
                    Run(pathToBatchToInstallIIS, string.Format("/Online /Enable-Feature {0}", string.Join(" ", featureNames.Select(name => string.Format("/FeatureName:{0}", name))))); // Note: Add /NoRestart to prevent asking the user to restart.

                    MessageBox.Show(@"Done.

You can now use the feature.

Please note that you may need to restart the computer for the changes to take effect. If this message keeps appearing after trying multiple times, please contact: support@cshtml5.com");

                    //Quit the app:
                    Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Application.Current.Shutdown();
                        }));
                }
                else
                {
                    MessageBox.Show("File not found:" + Environment.NewLine + Environment.NewLine + pathToBatchToInstallIIS + Environment.NewLine + Environment.NewLine + "Please report this issue to: support@cshtml5.com");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        static void Run(string fileName, string arguments)
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Normal,
                //RedirectStandardOutput = true,
                UseShellExecute = false,
            }))
            {
                process.WaitForExit();
                //return process.StandardOutput.ReadToEnd();
            }
        }
    }
}
