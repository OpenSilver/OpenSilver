using System;
using System.Windows;
using System.Windows.Controls;

namespace TestApplication
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            foreach (ITreeItem i in TestRegistry.Tests)
            {
                CreateTreeItem(i, MenuContainer.Items);
            }
        }

        private void CreateTreeItem(ITreeItem item, ItemCollection parent)
        {
            if (item is Test)
            {
                Test test = (Test)item;

                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = test.Name;
                treeViewItem.Selected += (object sender, RoutedEventArgs e) => {
                    NavigateToPage(test.FileName);
                };

                parent.Add(treeViewItem);
            }
            else if (item is TestCategory)
            {
                TestCategory testCategory = (TestCategory)item;

                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = testCategory.Name;

                parent.Add(treeViewItem);

                foreach (ITreeItem i in testCategory)
                {
                    CreateTreeItem(i, treeViewItem.Items);
                }
            }
        }

        private void NavigateToPage(string pageName)
        {
            // Navigate to the target page:
            Uri uri = new Uri(string.Format("/{0}Test", pageName), UriKind.Relative);
            ContentContainer.Source = uri;

            // Scroll to top:
            ScrollViewer1.ScrollToVerticalOffset(0d);
        }
    }
}
