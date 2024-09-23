

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using DotNetForHtml5.EmulatorWithoutJavascript.Console;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    //Do not remove this class: called via reflection.
    public class SimulatorProxy
    {
        private readonly IInputElement _webControl;
        private readonly ConsoleControl _console;
        public SimulatorProxy(IInputElement webControl, ConsoleControl console,
            Dispatcher openSilverRuntimeDispatcher,
            JavaScriptExecutionHandler javaScriptExecutionHandler)
        {
            _webControl = webControl;
            _console = console;

            OpenSilverRuntimeDispatcher = openSilverRuntimeDispatcher;
            JavaScriptExecutionHandler = javaScriptExecutionHandler;
        }

        //Do not remove this method: called via reflection.
        public Point GetMousePosition()
        {
            Point mousePosition = Mouse.GetPosition(_webControl);
            return new Point(-mousePosition.X, -mousePosition.Y);
        }

        //Do not remove this method: called via reflection.
        public void ShowException(Exception exception)
        {
            ShowExceptionStatic(exception);
        }

        internal static void ShowExceptionStatic(Exception exception)
        {
            // john.torjo when debugger attached, message boxes are a huge nuissance, lets write to the debug window
            if (Debugger.IsAttached)
                Debug.WriteLine($"SIMULATOR-caught exception: {exception.ToString()}");
            else
                // Display error message:
                MessageBox.Show("TIP: You can copy the content of this message box by pressing Ctrl+C now."
                                + Environment.NewLine + Environment.NewLine
                                + "The following unhandled exception was raised by the application:"
                                + Environment.NewLine + Environment.NewLine
                                + exception.ToString());
        }

        //Do not remove this method: called dynamically.
        public void ReportJavaScriptError(string error, string where)
        {
            _console.AddMessage(new ConsoleMessage
            {
                Text = error,
                Url = where,
                Source = ConsoleMessage.JavaScriptSource,
                Level = ConsoleMessage.ErrorLevel,
            });
        }

        internal static Dispatcher OpenSilverRuntimeDispatcher { get; private set; }

        internal static JavaScriptExecutionHandler JavaScriptExecutionHandler { get; private set; }
    }
}
