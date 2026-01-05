using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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

        private void SaveTextButton_Click(object sender, RoutedEventArgs e)
        {
            Save("Text files|*.txt", null, null);
        }

#if OPENSILVER
        private async void Save(string filter, string defaultFilename, string defaultExt)
#else
        private void Save(string filter, string defaultFilename, string defaultExt)
#endif
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (!string.IsNullOrEmpty(filter))
            {
                saveFileDialog.Filter = filter;
            }
            if (!string.IsNullOrEmpty(defaultFilename))
            {
                saveFileDialog.DefaultFileName = defaultFilename;
            }
            if (!string.IsNullOrEmpty(defaultExt))
            {
                saveFileDialog.DefaultExt = defaultExt;
            }

#if OPENSILVER
            bool? result = await saveFileDialog.ShowDialogAsync();
#else
            bool? result = saveFileDialog.ShowDialog();
#endif
            if (result == true)
            {
#if OPENSILVER
                using (Stream saveFileStream = await saveFileDialog.OpenFileAsync())
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
                SavingStatus.Text = $"File saved: '{saveFileDialog.SafeFileName}'";
            }
            else
            {
                MessageBox.Show("Result from file dialog was false.");
            }
        }

        private void SaveTextWithoutFilterButton_Click(object sender, RoutedEventArgs e)
        {
            Save(null, null, null);
        }

        private void SaveTextWithDefaultFilenameButton_Click(object sender, RoutedEventArgs e)
        {
            Save(null, "default-filename.txt", null);
        }

        private void SaveTextWithDefaultExtButton_Click(object sender, RoutedEventArgs e)
        {
            Save("All files|*.*", "default-filename-without-extension", ".defExt");
        }
    }
}
