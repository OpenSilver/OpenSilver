using System.Windows.Controls;

namespace TestApplication.OpenSilver.Tests
{
    public partial class DataTemplateTest : Page
    {
        public DataTemplateTest()
        {
            this.InitializeComponent();
            DataContext = new MainViewModel();
        }
    }

    public class MainViewModel
    {
        public OuterViewModel OuterViewModel { get; set; } = new OuterViewModel();
    }

    public class OuterViewModel
    {
        public InnerViewModel InnerViewModel { get; set; } = new InnerViewModel();
    }

    public class InnerViewModel { }
}
