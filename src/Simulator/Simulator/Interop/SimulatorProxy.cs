using DotNetBrowser.WPF;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    //Do not remove this class: called via reflection.
    public class SimulatorProxy
    {
        WPFBrowserView _webControl;

        public SimulatorProxy(WPFBrowserView webControl)
        {
            _webControl = webControl;

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
            // Display error message:
            MessageBox.Show(MainWindow.TipToCopyToClipboard
                + Environment.NewLine + Environment.NewLine
                + "The following unhandled exception was raised by the application:"
                + Environment.NewLine + Environment.NewLine
                + exception.ToString());
        }

        //Do not remove this method: called via reflection.
        public void ThrowExceptionWithoutLosingStackTrace(Exception exception)
        {
            //cf. https://stackoverflow.com/questions/57383/in-c-how-can-i-rethrow-innerexception-without-losing-stack-trace
            global::System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
        }

        //Do not remove this method: called via reflection.
        public void NavigateToUrlInNewBrowserWindow(string url)
        {
            if (url == null)
                url = "";

            if (url.ToLower().StartsWith("http://")
                || url.ToLower().StartsWith("https://"))
            {
                System.Diagnostics.Process.Start(url);
            }
            else
            {
                MessageBox.Show("The application requested to open the following page:" + Environment.NewLine + url + Environment.NewLine + Environment.NewLine + "This feature is not implemented in the Simulator. Please run the application in the browser instead.");
            }
        }

        //Do not remove this method: called via reflection.
        public object DelegateDynamicInvoke(Delegate d, params object[] args)
        {
            return d.DynamicInvoke(args);
        }

        //Do not remove this method: called via reflection.
        public object ConvertChangeType(object value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }

        //Do not remove this method: called via reflection.
        public bool TypeIsPrimitive(Type type)
        {
            return type.IsPrimitive;
        }

        //Do not remove this method: called via reflection.
        public string PathCombine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        //Do not remove this method: called via reflection.
        public int HexToInt(string hexString)
        {
            return int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        }

        //Do not remove this method: called via reflection.
        public void RunClassConstructor(Type type)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }

        public Type GetInterface(object type, string name)
        {
            return ((Type)type).GetInterface(name);
        }

        public object MakeInstanceOfGenericType(Type type, Type[] parameters)
        {
            return Activator.CreateInstance(type.MakeGenericType(parameters));
        }

        public object StartDispatcherTimer(Action action, long intervalInMilliseconds)
        {
            var dispatcherTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, (int)intervalInMilliseconds) //todo: verify that it's ok to cast from long to int here.
            };
            dispatcherTimer.Tick += (s, e) => { if (action != null) action(); };
            dispatcherTimer.Start();
            return dispatcherTimer;
        }

        public void StopDispatcherTimer(object dispatcherTimer)
        {
            ((DispatcherTimer)dispatcherTimer).Stop();
        }
    }
}
