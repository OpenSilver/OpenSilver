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
    public partial class FileInfoTest : Page
    {
        public FileInfoTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_FileInfo_SetFileContent_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            /*
            string inputString = InputTextBox.Text;
            string inputFile = FileNameTextBox.Text;

            if (!string.IsNullOrWhiteSpace(inputString) && !string.IsNullOrWhiteSpace(inputFile))
            {
                FileInfo fileInfo = new FileInfo(inputFile);
                using (FileStream fs = fileInfo.OpenWrite())
                {
                    Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(inputString);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
                ResultTextBlock.Text = "Done.";
            }
            else
            {
                ResultTextBlock.Text = "Error: Both the file name and the input string must be set to set the file's content.";
            }
            */
        }

        private void Button_FileInfo_GetFileContent_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            /*
            string inputFile = FileNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(inputFile))
            {
                FileInfo fileInfo = new FileInfo(inputFile);
                if (fileInfo.Exists)
                {
                    using (FileStream fs = fileInfo.OpenRead())
                    {
                        if (fs != null)
                        {
                            using (StreamReader sr = new StreamReader(fs))
                            {
                                ResultTextBlock.Text = sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            else
            {
                ResultTextBlock.Text = "Error: The file name must be set to read the file's content.";
            }
            */
        }

        private void Button_FileInfo_RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            /*
            string inputFile = FileNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(inputFile))
            {
                FileInfo fileInfo = new FileInfo(inputFile);
                fileInfo.Delete();
                ResultTextBlock.Text = "File deleted.";
            }
            else
            {
                ResultTextBlock.Text = "Error: The file name must be set to delete it.";
            }
            */
        }
    }
}
