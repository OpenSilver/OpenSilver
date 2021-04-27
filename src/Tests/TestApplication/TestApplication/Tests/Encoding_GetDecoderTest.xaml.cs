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
    public partial class Encoding_GetDecoderTest : Page
    {
        public Encoding_GetDecoderTest()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void ButtonTestDecoder_Click(object sender, RoutedEventArgs e)
        {
            TestEncoder();
        }

        void TestEncoder()
        {
            throw new NotImplementedException();
            /*
            //For personnal knowledge:
            //First byte    < 192 -->   1 byte =    1 character
            //First byte    < 224 -->   2 bytes =   1 character; Second byte must be >= 128
            //First byte    < 240 -->   3 bytes =   1 character; Following bytes must be >= 128
            //First byte    >= 240 -->   4 bytes =   1 character; Following bytes must be >= 128

            string charsNumbers = TestDecoderTextBox.Text;
            string[] splittedCharsNumbers = charsNumbers.Split(',');
            List<Byte> numbers = new List<Byte>();
            foreach (string str in splittedCharsNumbers)
            {
                int i;
                if (int.TryParse(str, out i))
                {
                    if (i < 255)
                    {
                        numbers.Add((Byte)i);
                    }
                }
            }

            Char[] chars;
            Byte[] bytes = numbers.ToArray();
            Decoder utf8Decoder = Encoding.UTF8.GetDecoder();

            int charCount = utf8Decoder.GetCharCount(bytes, 0, bytes.Length);
            chars = new Char[charCount];
            int charsDecodedCount = utf8Decoder.GetChars(bytes, 0, bytes.Length, chars, 0);
            string s = string.Format("{0} characters used to decode bytes.", charsDecodedCount);
            s += Environment.NewLine;
            s += "Decoded chars: ";
            s += Environment.NewLine;

            foreach (Char c in chars)
            {
                s += string.Format("[{0}]", c);
            }
            int charsCountFromGetCharCount = utf8Decoder.GetCharCount(bytes, 0, bytes.Length);
            s += Environment.NewLine;
            s += string.Format("GetCharCount result: {0}", charsCountFromGetCharCount);
            TestDecoderTextBlock.Text = s;
            */
        }
    }
}
