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
    public partial class DeSerializationTest : Page
    {
        public DeSerializationTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void ButtonSerialize_Click(object sender, RoutedEventArgs e)
        {
            //TestClass testObj = new TestClass();

            //testObj.SomeString = "foo";
            //testObj.Settings.Add("A");
            //testObj.Settings.Add("B");
            //testObj.Settings.Add("C");

            //var xmlSerializer = new XmlSerializer(testObj.GetType());
            //StringWriter textWriter = new StringWriter();
            //xmlSerializer.Serialize(textWriter, testObj);
            //SerializationResult.Text = textWriter.ToString();
        }

        void ButtonDeserialize_Click(object sender, RoutedEventArgs e)
        {
            ////var serializedObject = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<TestClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <SomeString>foo</SomeString>\r\n  <Settings>\r\n    <string>A</string>\r\n    <string>B</string>\r\n    <string>C</string>\r\n  </Settings>\r\n</TestClass>";
            ////var serializedObject = "<?xml version=\"1.0\" ?>\r\n<TestClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <SomeString>foo</SomeString>\r\n  <Settings>\r\n    <string>A</string>\r\n    <string>B</string>\r\n    <string>C</string>\r\n  </Settings>\r\n</TestClass>";
            //var serializedObject = "<TestClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <SomeString>foo</SomeString>\r\n  <Settings>\r\n    <string>A</string>\r\n    <string>B</string>\r\n    <string>C</string>\r\n  </Settings>\r\n</TestClass>";

            //var xmlSerializer = new XmlSerializer(typeof(TestClass));
            ////StringReader textReader = new StringReader(serializedObject);
            //var stream = GenerateStreamFromString(serializedObject);
            //TestClass testObj = (TestClass)xmlSerializer.Deserialize(stream);
            //SerializationResult.Text = "Deserialization succeeded\r\n" + testObj.SomeString + "\r\n" + testObj.Settings.Count.ToString() + "\r\n" + testObj.Settings[2];
        }
    }
}
