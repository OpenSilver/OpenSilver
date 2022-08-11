using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#if OPENSILVER
using OpenFileDialog = OpenSilver.Controls.OpenFileDialog;
#endif

namespace TestApplication.OpenSilver.Tests
{
    public partial class OpenFileDialogTest : Page
    {
        public OpenFileDialogTest()
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
    }
}
