﻿
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.IO;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media.Imaging
#else
namespace Windows.UI.Xaml.Media.Imaging
#endif
{
    /// <summary>
    /// Provides a source object for properties that use a bitmap.
    /// </summary>
    public abstract partial class BitmapSource : ImageSource
    {
        bool _isStreamAsBase64StringValid = false;

        private string _streamAsBase64String;
        public string INTERNAL_StreamAsBase64String
        {
            get
            {
                if (!_isStreamAsBase64StringValid)
                {
                    byte[] bytes = new byte[INTERNAL_StreamSource.Length];//note: if s.Length is longer than int.MaxValue, that's a problem... But that means they have a stream of more than 2 Go...
                    if (INTERNAL_StreamSource.Length > int.MaxValue)
                    {
                        throw new InvalidOperationException("The Stream set as the BitmapSource's Source is too big (more than int.MaxValue (2,147,483,647) bytes).");
                    }

                    int n = INTERNAL_StreamSource.Read(bytes, 0, (int)INTERNAL_StreamSource.Length);

                    //the following (commented) is in case the previous line doesn't work.
                    //int numBytesToRead = (int)s.Length; 
                    //int numBytesRead = 0;
                    //do
                    //{
                    //    // Read may return anything from 0 to 10.
                    //    int n = s.Read(bytes, numBytesRead, 10);
                    //    numBytesRead += n;
                    //    numBytesToRead -= n;
                    //} while (numBytesToRead > 0);
                    INTERNAL_StreamSource.Close();
                    _streamAsBase64String = Convert.ToBase64String(bytes);
                    _isStreamAsBase64StringValid = true;
                }
                return _streamAsBase64String;
            }
        }

        /// <summary>
        /// Provides base class initialization behavior for BitmapSource-derived classes.
        /// </summary>
        protected BitmapSource() : base() { }

        private Stream _streamSource;
        public Stream INTERNAL_StreamSource
        {
            get { return _streamSource; }
            private set { _streamSource = value; }
        }


        private string _dataUrl;
        public string INTERNAL_DataURL
        {
            get { return _dataUrl; }
            private set { _dataUrl = value; }
        }

        /// <summary>
        /// Sets the source image for a BitmapSource by accessing a stream.
        /// </summary>
        /// <param name="streamSource">The stream source that sets the image source value.</param>
        public void SetSource(Stream streamSource) //note: this is supposed to be a IRandomAccessStream
        {
            // Copying the original stream because it could be disposed by the user before it is consumed
            // by the target image
            MemoryStream streamCopy = new MemoryStream();
            streamSource.CopyTo(streamCopy);
            streamCopy.Seek(0, SeekOrigin.Begin);

            INTERNAL_StreamSource = streamCopy;
            _isStreamAsBase64StringValid = false; //in case we set the source after having already set it and used it.
        }


        /// <summary>
        /// Sets the source image for a BitmapSource by passing a "data URL".
        /// </summary>
        /// <param name="dataUrl">The image encoded in "data URL" format.</param>
        public void SetSource(string dataUrl)
        {
            INTERNAL_DataURL = dataUrl;
        }

        internal override Task<string> GetDataStringAsync()
        {
            string data = string.Empty;
            if (INTERNAL_StreamSource != null)
            {
                data = "data:image/png;base64," + INTERNAL_StreamAsBase64String;
            }
            else if (!string.IsNullOrEmpty(INTERNAL_DataURL))
            {
                data = INTERNAL_DataURL;
            }

            return Task.FromResult(data);
        }

        #region Not supported yet
        /// <summary>
        /// Gets the height of the bitmap in pixels.
        /// </summary>
        [OpenSilver.NotImplemented]
        public int PixelHeight => PixelHeightInternal;

        /// <summary>
        /// Identifies the PixelHeight dependency property.
        /// 
        /// Returns the identifier for the PixelHeight dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PixelHeightProperty = DependencyProperty.Register("PixelHeight", typeof(int), typeof(BitmapSource), new PropertyMetadata(0));

        /// <summary>
        /// Gets the width of the bitmap in pixels.
        /// </summary>
        [OpenSilver.NotImplemented]
        public int PixelWidth => PixelWidthInternal;

        /// <summary>
        /// Identifies the PixelWidth dependency property.
        /// 
        /// Returns the identifier for the PixelWidth dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PixelWidthProperty = DependencyProperty.Register("PixelWidth", typeof(int), typeof(BitmapSource), new PropertyMetadata(0));


        ////
        //// Summary:
        ////     Sets the source image for a BitmapSource by accessing a stream and processing
        ////     the result asynchronously.
        ////
        //// Parameters:
        ////   streamSource:
        ////     The stream source that sets the image source value.
        ////
        //// Returns:
        ////     An asynchronous handler called when the operation is complete.
        //public IAsyncAction SetSourceAsync(IRandomAccessStream streamSource);
        #endregion

        internal virtual int PixelHeightInternal => (int)GetValue(PixelHeightProperty);

        internal virtual int PixelWidthInternal => (int)GetValue(PixelWidthProperty);
    }
}
