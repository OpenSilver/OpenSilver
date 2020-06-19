using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // We set the global culture to "Invariant" so that things like "double.TryParse" in "TypeFromStringConverters.ConvertDoubleFromString" works fine (because in XAML numbers use dots, like "0.3").
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            SimulatorProxy.ShowExceptionStatic(e.Exception);
            e.Handled = true;
        }
    }
}
