using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class AsyncAwaitTest : Page
    {
        public AsyncAwaitTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        // SLDISABLED
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    TextBlock2.Text = "Before call to Async.";
        //    test();
        //    TextBlock2.Text = "After call to Async.";

        //}

        //private async void Button2_Click(object sender, RoutedEventArgs e)
        //{
        //    TextBlock2.Text = "Before call to Async.";
        //    await test();
        //    TextBlock2.Text = "After call to Async.";

        //}

        //public async Task test()
        //{
        //    TextBlock2.Text = await TestAsync();
        //}

        //public static Task<string> TestAsync()
        //{
        //    var taskCompletionSource = new TaskCompletionSource<string>();
        //    var cameraCaptureTask = new Bidon();
        //    cameraCaptureTask.Completed += (sender, result) => taskCompletionSource.SetResult("Result obtained.");
        //    cameraCaptureTask.Start();
        //    return taskCompletionSource.Task;
        //}

        //class Bidon
        //{
        //    public event EventHandler Completed;
        //    DispatcherTimer t = new DispatcherTimer();
        //    public void Start()
        //    {
        //        t.Interval = new TimeSpan(0, 0, 5);
        //        t.Tick += t_Tick;
        //        t.Start();
        //    }

        //    void t_Tick(object sender, object e)
        //    {
        //        t.Stop();
        //        Completed(sender, new EventArgs());
        //    }
        //}
    }
}
