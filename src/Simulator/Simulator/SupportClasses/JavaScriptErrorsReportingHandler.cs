using DotNetBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    internal class JavaScriptErrorsReportingHandler
    {
        const string SimulatorRootFileNameWithoutExtension = "simulator_root";
        const string TipForDebugging = "TIP: You can attempt to pinpoint the cause of this error by looking at the history of the JavaScript calls. To do so, click the 'Tools' icon in the bottom-left corner of the window, and click 'View all JS code executed by the Simulator so far'. Then, copy/paste the log into a text editor, and search for elements related to this error message.\nAlternatively, you can launch the application in the browser, and view the browser console log for details. You can also enable the 'break on exceptions' option in the browser developer tools in order to walk up the call stack until you find the code that belongs to you.";

        bool _doNotDisplayJavaScriptErrors = false;
        bool _raiseExceptionWhenJSErrorOccurs = false;
        bool _corsTutorialAlreadyDisplayed = false;
        JavaScriptExecutionHandler _javaScriptExecutionHandler;
        Dispatcher _dispatcher;
        List<string> _errorsPendingToBeDisplayed = new List<string>();
        StringBuilder _consoleLog = new StringBuilder();

        public JavaScriptErrorsReportingHandler(JavaScriptExecutionHandler javaScriptExecutionHandler, Dispatcher dispatcher)
        {
            _javaScriptExecutionHandler = javaScriptExecutionHandler;
            _dispatcher = dispatcher;
        }

        public void OnConsoleMessageEvent(object sender, DotNetBrowser.Events.ConsoleEventArgs e)
        {
            _consoleLog.AppendLine(e.Message ?? "");

            if (!_doNotDisplayJavaScriptErrors
                && (int)e.Level > 0)
            {
                string friendlyErrorMessage = GenerateFriendlyErrorMessage(_javaScriptExecutionHandler, e);

                // Ignore some of the errors:
                if (!friendlyErrorMessage.Contains("DESCRIPTION: BSSO Telemetry:") // Ignore this error on Azure login
                    && !friendlyErrorMessage.Contains("https://xhr.spec.whatwg.org/")) // Ignore the error "Synchronous XMLHttpRequest on the main thread is deprecated because of its detrimental effects to the end user's experience. For more help, check https://xhr.spec.whatwg.org/."
                    _errorsPendingToBeDisplayed.Add(friendlyErrorMessage);

                // In case of CORS-related errors, display the tutorial:
                if (friendlyErrorMessage.Contains("Access-Control")
                    && !_corsTutorialAlreadyDisplayed
                    && !CrossDomainCallsHelper.IsBypassCORSErrors)
                {
                    _corsTutorialAlreadyDisplayed = true;
                    System.Diagnostics.Process.Start("https://opensilver.net/permalinks/simulator_cors_errors.aspx");
                }

                // Display the message asynchronously on the UI thread:
                _dispatcher.BeginInvoke((Action)(() =>
                {
                    if (!_doNotDisplayJavaScriptErrors) // Note: this check is here because the value can have changed since the check at the beginning of the method due to the fact that we are inside a Dispatcher.BeginInvoke.
                    {
                        if (_errorsPendingToBeDisplayed.Count > 0) // Note: it can be empty if the errors have accumulated and the first Dispatcher.BeginInvoke has displayed all the messages at once.
                        {
                            // Aggregate all the error messages raised so far:
                            string aggregatedErrors = String.Join(Environment.NewLine + Environment.NewLine, _errorsPendingToBeDisplayed);

                            // Clear the list of pending errors to display:
                            _errorsPendingToBeDisplayed.Clear();

                            // Display the message:
                            var stackPanel = new StackPanel() { VerticalAlignment = VerticalAlignment.Center };
                            var checkBox1 = new CheckBox()
                            {
                                Content = "Do not display errors again during this session.",
                                Margin = new Thickness(20, 0, 0, 0),
                                IsChecked = false
                            };
                            //var checkBox2 = new CheckBox()
                            //{
                            //    Content = "Raise an exception immediately when a JS error occurs.",
                            //    Margin = new Thickness(20, 5, 0, 0),
                            //    IsChecked = _raiseExceptionWhenJSErrorOccurs
                            //};
                            //checkBox2.Checked += (s2, e2) => { if (!_raiseExceptionWhenJSErrorOccurs) { MessageBox.Show("TIP: You can relaunch the application by clicking the 'Tools' icon in the bottom-left corner of the Simulator window and then clicking 'Re-instantiate App.xaml.cs'.", "TIP", MessageBoxButton.OK, MessageBoxImage.Information); } };
                            stackPanel.Children.Add(checkBox1);
                            //stackPanel.Children.Add(checkBox2);
                            var msgBox = new MessageBoxScrollable()
                            {
                                Value = aggregatedErrors
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + Environment.NewLine
                                    + TipForDebugging,
                                Title = "JavaScript Error(s)",
                                AdditionalContentToDisplay = stackPanel,
                            };
                            if (msgBox.Visibility == Visibility.Hidden) // Note: we cannot call the "ShowDialog" method if the window is already visible.
                            {
                                msgBox.ShowDialog();
                            }
                            else
                            {
                                msgBox.Show();
                            }
                            if (checkBox1.IsChecked == true)
                                _doNotDisplayJavaScriptErrors = true;
                            //if (checkBox2.IsChecked == true)
                            //    _raiseExceptionWhenJSErrorOccurs = true;
                        }
                    }
                }));
            }
        }

        public static string GenerateFriendlyErrorMessage(JavaScriptExecutionHandler javaScriptExecutionHandler, DotNetBrowser.Events.ConsoleEventArgs e)
        {
            // Display the error description:
            string friendlyErrorMessage = "---- JAVASCRIPT ERROR ----"
                + Environment.NewLine + Environment.NewLine + "DESCRIPTION: " + (e.Message ?? "n/a");

            // Display the source file and the line where the error occurred:
            if (!string.IsNullOrEmpty(e.Source) && !e.Source.Contains(SimulatorRootFileNameWithoutExtension))
            {
                friendlyErrorMessage += Environment.NewLine + Environment.NewLine + "SOURCE: " + e.Source
                                        + Environment.NewLine + Environment.NewLine + "LINE: " + e.LineNumber.ToString();
            }

            /*
            // Attempt to display the code that has the issue, if any:
            if (!string.IsNullOrEmpty(e.Source) && e.Source.Contains(SimulatorRootFileNameWithoutExtension))
            {
                string lastExecutedJavaScriptCode = javaScriptExecutionHandler.GetLastExecutedJavaScriptCode();
                if (!string.IsNullOrWhiteSpace(lastExecutedJavaScriptCode))
                {
                    string[] lines = lastExecutedJavaScriptCode.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    int lineNumber = e.LineNumber;
                    if (lineNumber > 0 && lineNumber <= lines.Length)
                    {
                        lastExecutedJavaScriptCode = lines[lineNumber - 1];
                    }
                    friendlyErrorMessage += Environment.NewLine + Environment.NewLine + "LAST EXECUTED CODE:"
                        + Environment.NewLine + lastExecutedJavaScriptCode;
                }
            }
            */

            return friendlyErrorMessage;
        }

        public string ConsoleLog
        {
            get
            {
                return _consoleLog.ToString();
            }
        }
    }
}
