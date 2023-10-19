using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Windows.Controls;

namespace System.Windows.Media.Imaging.Tests
{
    [TestClass]
    public class BitmapImageTest
    {
        private static readonly string Base64ImageExample = "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7";

        [TestMethod]
        public void BitmapImage_SetStreamSource_ShouldConsumeAsSoonAsSet()
        {
            BitmapImage bitmapImage = new BitmapImage();
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(Base64ImageExample));
            bitmapImage.SetSource(ms);
            // In Silverlight it is possible to dispose original source before consuming it elsewhere
            ms.Dispose();

            Image image = new Image();
            // BitmapImage should still contain data stream even though original MemoryStream
            // has been disposed of
            image.Source = bitmapImage;

            (image.Source as BitmapImage).INTERNAL_StreamAsBase64String
                .Should()
                .Be(Base64ImageExample);
        }
    }
}
