using EnvDTE80;
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace DotNetForHtml5.MigrationWizard
{
    /// <summary>
    /// Interaction logic for Page_Processing.xaml
    /// </summary>
    public partial class Page_Processing : Page, IWizardPage
    {
        Status _currentStatus = Status.Processing;
        Context _context;

        enum Status
        {
            Processing, Completed
        }

        public Page_Processing(Context context)
        {
            InitializeComponent();

            _context = context;

            this.Loaded += Page_Processing_Loaded;
            this.Unloaded += Page_Processing_Unloaded;

            Processor.StaticAbortProcessing = false;
            Processor.StaticDispatcher = this.Dispatcher;
            Processor.StaticProgressLog = this.ProgressLog;
        }

        void Page_Processing_Unloaded(object sender, RoutedEventArgs e)
        {
            // Cause the background thread to quit gracefully when the window is closed for example:
            Processor.StaticAbortProcessing = true;
        }

        async void Page_Processing_Loaded(object sender, RoutedEventArgs e)
        {
            _context.RaiseMigrationStartedEvent();

            await Processor.StartProcessingAsync(_context, OnCompleted);
        }

        void OnCompleted()
        {
            _currentStatus = Status.Completed;
            Title.Text = "Done!";
            _context.RaiseMigrationCompletedEvent();
        }

        public bool TryCreateNextPage(Context context, out Page nextPage)
        {
            if (_currentStatus == Status.Completed)
            {
                MessageBox.Show("Please use the top-right cross to close this wizard.");
                nextPage = null;
                return false;
            }
            else
            {
                MessageBox.Show("Please wait for the operation to complete.");
                nextPage = null;
                return false;
            }
        }

        public bool CanGoBack
        {
            get
            {
                return false;
            }
        }
    }
}
