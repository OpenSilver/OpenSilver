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
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;

namespace TestApplication.Tests
{
    public partial class IsolatedStorageTest : Page
    {
        public IsolatedStorageTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ButtonSaveToIsolatedStorage_Click(object sender, RoutedEventArgs e)
        {
            string path = TextBoxWithIsolatedStorageFilePath.Text;
            FileSystemHelpers.WriteTextToFile(path, TextBoxWithNewTextForIsolatedStorage.Text);
        }

        private void ButtonLoadFromIsolatedStorage_Click(object sender, RoutedEventArgs e)
        {
            string path = TextBoxWithIsolatedStorageFilePath.Text;
            TextBlockWithLoadedText.Text = FileSystemHelpers.ReadTextFromFile(path);
        }

        private void ButtonDeleteFromIsolatedStorage_Click(object sender, RoutedEventArgs e)
        {
            string path = TextBoxWithIsolatedStorageFilePath.Text;
            FileSystemHelpers.DeleteFile(path);
        }

        private void ButtonSaveToIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            FileSystemHelpers.WriteTextToSettings(key, TextBoxWithIsolatedStorageSettingsValue.Text);
        }
        private void ButtonSaveToIsolatedStorageSettingsWithAdd_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            IsolatedStorageSettings.ApplicationSettings.Add(key, TextBoxWithIsolatedStorageSettingsValue.Text);
        }


        private void ButtonRemoveFromIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            IsolatedStorageSettings.ApplicationSettings.Remove(key);
        }

        private void ButtonLoadFromIsolatedStorageSettings_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            TextBlockWithIsolatedStorageSettingsLoadedText.Text = FileSystemHelpers.ReadTextFromSettings(key);
            TextBlockWithIsolatedStorageSettingsElementsCount.Text = IsolatedStorageSettings.ApplicationSettings.Count.ToString();
            string temp = "";
            foreach (var pair in IsolatedStorageSettings.ApplicationSettings)
            {
                temp += "{" + pair.Key + "," + pair.Value + "},";
            }
            TextBlockWithIsolatedStorageSettingsElements.Text = temp;
        }

        private void ButtonLoadFromIsolatedStorageUsingTryGetValue_Click(object sender, RoutedEventArgs e)
        {
            string key = TextBoxWithIsolatedStorageSettingsKey.Text;
            string value = string.Empty;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out value);
            TextBlockWithIsolatedStorageSettingsLoadedText.Text = value;
            TextBlockWithIsolatedStorageSettingsElementsCount.Text = IsolatedStorageSettings.ApplicationSettings.Count.ToString();
            string temp = "";
            foreach (var pair in IsolatedStorageSettings.ApplicationSettings)
            {
                temp += "{" + pair.Key + "," + pair.Value + "},";
            }
            TextBlockWithIsolatedStorageSettingsElements.Text = temp;
        }

        public static class FileSystemHelpers
        {
            public static void WriteTextToFile(string fileName, string fileContent)
            {
#if OPENSILVER || SILVERLIGHT
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
#else
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
                {
                    IsolatedStorageFileStream fs = null;
                    using (fs = storage.CreateFile(fileName))
                    {
                        if (fs != null)
                        {
                            //using (StreamWriter sw = new StreamWriter(fs))
                            //{
                            //    sw.Write(fileContent);
                            //}
                            Encoding encoding = new UTF8Encoding();
                            byte[] bytes = encoding.GetBytes(fileContent);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                    }
                }
            }

            public static void DeleteFile(string fileName)
            {
#if OPENSILVER || SILVERLIGHT
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
#else
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
                {
                    storage.DeleteFile(fileName);
                }
            }

            public static string ReadTextFromFile(string fileName)
            {
#if OPENSILVER || SILVERLIGHT
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
#else
                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly())
#endif
                {
                    if (storage.FileExists(fileName))
                    {
                        using (IsolatedStorageFileStream fs = storage.OpenFile(fileName, System.IO.FileMode.Open))
                        {
                            if (fs != null)
                            {
                                using (StreamReader sr = new StreamReader(fs))
                                {
                                    return sr.ReadToEnd();
                                }

                                //byte[] saveBytes = new byte[4];
                                //int count = fs.Read(saveBytes, 0, 4);
                                //if (count > 0)
                                //{
                                //    number = System.BitConverter.ToInt32(saveBytes, 0);
                                //}
                            }
                        }
                    }
                }
                return null;
            }

            public static void WriteTextToSettings(string key, string value)
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            }

            public static string ReadTextFromSettings(string key)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                {
                    object value = IsolatedStorageSettings.ApplicationSettings[key];
                    if (value is string)
                    {
                        return (string)value;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }


        }
    }
}
