using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Serialization;

namespace TestApplication.Tests
{
    public partial class DateTimeTest : Page
    {
        public DateTimeTest()
        {
            InitializeComponent();
        }

        [DataContract]
        public class ClassToTestDateTimeSerialization
        {
            public DateTime DateField { get; set; }
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ButtonTestDateTime1_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(new System.DateTime(2014, 02, 01, 15, 16, 17));
        }

        private void ButtonTestDateTime2_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(new System.DateTime(2014, 02, 01, 15, 16, 17, DateTimeKind.Local));
        }

        private void ButtonTestDateTime3_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(new System.DateTime(2014, 02, 01, 15, 16, 17, DateTimeKind.Utc));
        }

        private void ButtonTestDateTime4_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(System.DateTime.Now);
        }

        private void ButtonTestDateTime5_Click(object sender, RoutedEventArgs e)
        {
            TestDateTime(System.DateTime.Now);
        }

        void TestDateTime(System.DateTime dateTime)
        {
            DisplayDateTimeFullInfo(dateTime, "Input DateTime:");

            //----------------------
            // Test DateTime serialization/deserialization:
            //----------------------
            var _classToSerialize = new ClassToTestDateTimeSerialization()
            {
                DateField = dateTime
            };

            // Serialize:
            var serializer = new XmlSerializer(typeof(ClassToTestDateTimeSerialization));
            var stream = new MemoryStream(); serializer.Serialize(stream, _classToSerialize);
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            var serializedXml = reader.ReadToEnd();

            MessageBox.Show("Serialized: " + serializedXml);

            // Deserialize:
            var deserializer = new XmlSerializer(typeof(ClassToTestDateTimeSerialization));
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serializedXml));
            var xmlReader = XmlReader.Create(memoryStream);
            ClassToTestDateTimeSerialization deserializedObject = (ClassToTestDateTimeSerialization)deserializer.Deserialize(xmlReader);

            DisplayDateTimeFullInfo(deserializedObject.DateField, "Deserialized DateTime:");
        }

        void DisplayDateTimeFullInfo(System.DateTime dateTime, string title)
        {
            MessageBox.Show(title + Environment.NewLine
                + "   ToString()=" + dateTime.ToString() + Environment.NewLine
                + "   ToShortDateString()=" + dateTime.ToShortDateString() + Environment.NewLine
                + "   ToShortTimeString()=" + dateTime.ToShortTimeString() + Environment.NewLine
                //+ "   ToLongTimeString()=" + dateTime.ToLongTimeString() + Environment.NewLine
                + "   ToUniversalTime().ToString()=" + dateTime.ToUniversalTime().ToString() + Environment.NewLine
                + "   ToLocalTime().ToString()=" + dateTime.ToLocalTime().ToString() + Environment.NewLine
                + "   ToUniversalTime().ToLocalTime().ToString()=" + dateTime.ToUniversalTime().ToLocalTime().ToString() + Environment.NewLine
                + "   ToLocalTime().ToUniversalTime().ToString()=" + dateTime.ToLocalTime().ToUniversalTime() + Environment.NewLine
                + "   Hours=" + dateTime.Hour.ToString() + Environment.NewLine
                + "   Minutes=" + dateTime.Minute.ToString() + Environment.NewLine
                + "   Seconds=" + dateTime.Second.ToString() + Environment.NewLine
                );
        }
    }
}
