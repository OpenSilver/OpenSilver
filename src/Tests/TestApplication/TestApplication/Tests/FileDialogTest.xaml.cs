using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
#if OPENSILVER
using OpenFileDialog = OpenSilver.Controls.OpenFileDialog;
using SaveFileDialog = OpenSilver.Controls.SaveFileDialog;
#endif

namespace TestApplication.OpenSilver.Tests
{
    public partial class FileDialogTest : Page
    {
        public FileDialogTest()
        {
            this.InitializeComponent();
        }

#if OPENSILVER
        private async void LoadImageButton_Click(object sender, RoutedEventArgs e)
#else
        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
#endif
        {
            OperationStatus.Text = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.*";
#if OPENSILVER
            bool? isOperationSuccessful = await openFileDialog.ShowDialogAsync();
#else
            bool? isOperationSuccessful = openFileDialog.ShowDialog();
#endif

            if (isOperationSuccessful == true)
            {
                using (Stream strm = openFileDialog.File.OpenRead())
                {
                    byte[] buffer = new byte[strm.Length];
                    strm.Read(buffer, 0, (int)strm.Length);

                    BitmapImage bi = new BitmapImage();
                    MemoryStream ms = new MemoryStream(buffer);
                    bi.SetSource(ms);
                    ms.Dispose();
                    UploadedImage.Source = bi;
                }

                OperationStatus.Text = $"File loaded: {openFileDialog.File.Name}";
            }
            else
            {
                OperationStatus.Text = "File dialog canceled.";
            }
        }

#if OPENSILVER
        private async void SaveTextButton_Click(object sender, RoutedEventArgs e)
#else
        private void SaveTextButton_Click(object sender, RoutedEventArgs e)
#endif
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "*.txt|*.txt"
            };
#if OPENSILVER
            bool? result = await saveFileDialog.ShowDialog();
#else
            bool? result = saveFileDialog.ShowDialog();
#endif
            if (result == true)
            {
#if OPENSILVER
                using (Stream saveFileStream = await saveFileDialog.OpenFile())
#else
                using (Stream saveFileStream = saveFileDialog.OpenFile())
#endif
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(ToSaveTextBox.Text);
#if OPENSILVER
                    await saveFileStream.WriteAsync(bytes, 0, bytes.Length);
                    await saveFileStream.FlushAsync();
#else
                    saveFileStream.Write(bytes, 0, bytes.Length);
                    saveFileStream.Flush();
#endif
                }
            }
            else
            {
                MessageBox.Show("Result from file dialog was false.");
            }
        }
    }
}
