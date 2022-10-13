using OpenSilver.Simulator;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace OpenSilver.Simulator
{
    internal class OpenSilverRuntime
    {
        private readonly MainWindow _simMainWindow;
        private Dispatcher _SimDispatcher;
        private Dispatcher _OSDispatcher;
        private Action _ClientAppStartup;
        public static Assembly OSRuntimeAssembly { get; private set; }
        public JavaScriptExecutionHandler JavaScriptExecutionHandler { get; set; }

        public Action OnInitialized { get; set; }


        public OpenSilverRuntime(MainWindow simMainWindow, Dispatcher simDispatcher)
        {
            _simMainWindow = simMainWindow;
            _SimDispatcher = simDispatcher;
            Assembly osRunTimeAsm;
            if (!ReflectionInUserAssembliesHelper.TryGetCoreAssembly(out osRunTimeAsm))
                throw new Exception("Can't find OpenSilver runtime assembly");

            OSRuntimeAssembly = osRunTimeAsm;
        }

        public bool Start(Action clientAppStartup)
        {
            _ClientAppStartup = clientAppStartup;
            try
            {
                var osThread = new Thread(new ThreadStart(() =>
                {
                    _OSDispatcher = Dispatcher.CurrentDispatcher;

                    if (!Initialize())
                        return;
                    if (OnInitialized != null)
                        OnInitialized();

                    _ClientAppStartup();

                    Dispatcher.Run();
                }));

                osThread.SetApartmentState(ApartmentState.STA);
                osThread.Priority = ThreadPriority.Highest;
                osThread.Start();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to start the application.\r\n\r\n" + ex.ToString());
                _simMainWindow.HideLoadingMessage();
                return false;
            }

        }

        private bool Initialize()
        {
            // In OpenSilver we already have the user application type passed to the constructor, so we do not need to retrieve it here
            try
            {
                // Create the JavaScriptExecutionHandler that will be called by the "Core" project to interact with the Simulator:
                JavaScriptExecutionHandler = new JavaScriptExecutionHandler();

                InteropHelpers.InjectJavaScriptExecutionHandler(JavaScriptExecutionHandler, OSRuntimeAssembly);
                //InteropHelpers.InjectWpfMediaElementFactory(_OSRuntimeAssembly);
                //InteropHelpers.InjectWebClientFactory(_OSRuntimeAssembly);
                InteropHelpers.InjectClipboardHandler(OSRuntimeAssembly);
                InteropHelpers.InjectSimulatorProxy(new SimulatorProxy(_simMainWindow.Console, _SimDispatcher, _OSDispatcher), OSRuntimeAssembly);

                // In the OpenSilver Version, we use this work-around to know if we're in the simulator
                InteropHelpers.InjectIsRunningInTheSimulator_WorkAround(OSRuntimeAssembly);

                //WpfMediaElementFactory._gridWhereToPlaceMediaElements = _simMainWindow.GridForAudioMediaElements;

                // Inject the code to display the message box in the simulator:
                InteropHelpers.InjectCodeToDisplayTheMessageBox(
                    (message, title, showCancelButton) => { return MessageBox.Show(message, title, showCancelButton ? MessageBoxButton.OKCancel : MessageBoxButton.OK) == MessageBoxResult.OK; },
                    OSRuntimeAssembly);

                // Ensure the static constructor of all common types is called so that the type converters are initialized:
                StaticConstructorsCaller.EnsureStaticConstructorOfCommonTypesIsCalled(OSRuntimeAssembly);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading the application: " + Environment.NewLine + Environment.NewLine + ex.Message);
                _simMainWindow.HideLoadingMessage();
                return false;
            }
        }

        public void Stop()
        {
            //OS::DotNetForHtml5.Core.INTERNAL_Simulator.SimulatorProxy.IsOSRuntimeRunning = false;
        }
    }
}
