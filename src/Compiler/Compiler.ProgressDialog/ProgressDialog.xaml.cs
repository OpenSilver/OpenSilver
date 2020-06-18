using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DotNetForHtml5.Compiler.ProgressDialog
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public static readonly string Done = "DONE";

        public ProgressDialog()
        {
            InitializeComponent();

            this.Loaded += ProgressDialog_Loaded;
            this.MouseDown += ProgressDialog_MouseDown;
        }

        void ProgressDialog_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Make the WPF window draggable, no matter what element is clicked:
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        void ProgressDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
        }

        public string Text
        {
            set
            {
                ProgessText.Text = value;
            }
        }

        public static void ShowOnUIThread(string fileName, Dictionary<string, string> typeScriptFileNameToProgress)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                var synchronizationContext = new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher);
                SynchronizationContext.SetSynchronizationContext(synchronizationContext);

                var progressDialog = new ProgressDialog();
                progressDialog.Show();

                Thread t2 = new Thread(new ThreadStart(() =>
                {
                    while (typeScriptFileNameToProgress[fileName] != ProgressDialog.Done)
                    {
                        synchronizationContext.Send(s =>
                        {
                            progressDialog.Text = typeScriptFileNameToProgress[fileName];

                        }, null);
                        Thread.Sleep(200);
                    }
                    synchronizationContext.Send(s =>
                    {
                        progressDialog.Hide();
                    }, null);
                }));

                t2.IsBackground = true; // Prevents the background thread from keeping the application alive.
                t2.Start();

                System.Windows.Threading.Dispatcher.Run();
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true; // Prevents the background thread from keeping the application alive.
            t.Start();
        }
    }
}
