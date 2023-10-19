
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

using System.Threading.Tasks;

namespace System.Windows.Media.Imaging
{
    /// <summary>
    /// Provides a <see cref="BitmapSource"/> that can be written to and updated.
    /// </summary>
    public sealed partial class WriteableBitmap : BitmapSource
    {
        private readonly IWriteableBitmapImpl _impl;

        private int[] _pixels = Array.Empty<int>();
        private int _pixelWidth;
        private int _pixelHeight;

        private WriteableBitmap()
        {
            _impl = OpenSilver.Interop.IsRunningInTheSimulator ?
                new WriteableBitmapSimulator(this) :
                new WriteableBitmapWasm(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteableBitmap" /> class 
        /// using the provided dimensions.
        /// </summary>
        /// <param name="pixelWidth">
        /// The width of the bitmap.
        /// </param>
        /// <param name="pixelHeight">
        /// The height of the bitmap.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="pixelWidth" /> or <paramref name="pixelHeight" /> is zero or less.
        /// </exception>
        public WriteableBitmap(int pixelWidth, int pixelHeight)
            : this()
        {
            if (pixelWidth < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pixelWidth));
            }

            if (pixelHeight < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pixelHeight));
            }

            _pixelWidth = pixelWidth;
            _pixelHeight = pixelHeight;
            _pixels = new int[_pixelWidth * _pixelHeight];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteableBitmap" /> class 
        /// using the provided <see cref="BitmapSource" />.
        /// </summary>
        /// <param name="source">
        /// The <see cref="BitmapSource" /> to use for initialization.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public WriteableBitmap(BitmapSource source)
            : this()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _ = _impl.CreateFromBitmapSourceAsync(source);
        }

        public static async Task<WriteableBitmap> CreateAsync(BitmapSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var bitmap = new WriteableBitmap(source);
            await bitmap.WaitToInitialize();
            return bitmap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteableBitmap" /> class using 
        /// the provided element and transform.
        /// </summary>
        /// <param name="element">
        /// The desired element to be rendered within the bitmap.
        /// </param>
        /// <param name="transform">
        /// The transform the user wants to apply to the element as the last step before 
        /// drawing into the bitmap. This is particularly interesting for you if you want 
        /// the bitmap to respect its transform. This value can be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public WriteableBitmap(UIElement element, Transform transform)
            : this()
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _ = _impl.CreateFromUIElementAsync(element, transform);
        }

        public static async Task<WriteableBitmap> CreateAsync(UIElement element, Transform transform)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var bitmap = new WriteableBitmap(element, transform);
            await bitmap.WaitToInitialize();
            return bitmap;
        }

        /// <summary>
        /// User must call WaitToInitialize after instantiation in order to load the buffer
        /// </summary>
        public Task WaitToInitialize() => _impl.WaitForCompletionAsync();

        /// <summary>
        /// Gets an array representing the 2-D texture of the bitmap.
        /// </summary>
        /// <returns>
        /// An array of integers representing the 2-D texture of the bitmap.
        /// </returns>
        public int[] Pixels => _pixels;

        /// <summary>
        /// Renders an element within the bitmap.
        /// </summary>
        /// <param name="element">
        /// The element to be rendered within the bitmap.
        /// </param>
        /// <param name="transform">
        /// The transform to apply to the element before drawing into the bitmap. 
        /// If an empty transform is supplied, the bits representing the element 
        /// show up at the same offset as if they were placed within their parent.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="element" /> is null.
        /// </exception>
        public void Render(UIElement element, Transform transform)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _ = _impl.RenderUIElementAsync(element, transform, PixelWidth, PixelHeight);
        }

        public Task RenderAsync(UIElement element, Transform transform)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return _impl.RenderUIElementAsync(element, transform, PixelWidth, PixelHeight);
        }

        /// <summary>
        /// Requests a draw or redraw of the entire bitmap.
        /// </summary>
        public void Invalidate()
        {
            if (_pixels != null)
            {
                int rowLenth = _pixelWidth * 4 + 1;

                var bytes = new byte[rowLenth * _pixelHeight];

                for (int y = 0; y < PixelHeight; y++)
                {
                    for (int x = 0; x < PixelWidth; x++)
                    {
                        var rgba = BitConverter.GetBytes(Pixels[PixelWidth * y + x]);
                        int startIdx = rowLenth * y + x * 4 + 1;
                        for (int j = 0; j < rgba.Length; j++)
                        {
                            bytes[startIdx + j] = rgba[j];
                        }
                    }
                }

                SetSource(PngEncoder.Encode(bytes, PixelWidth, PixelHeight));
            }
        }

        internal override int PixelHeightInternal => _pixelHeight;

        internal override int PixelWidthInternal => _pixelWidth;

        internal override async Task<string> GetDataStringAsync(UIElement parent)
        {
            await WaitToInitialize();
            return await base.GetDataStringAsync(parent);
        }

        private interface IWriteableBitmapImpl
        {
            Task CreateFromBitmapSourceAsync(BitmapSource source);

            Task CreateFromUIElementAsync(UIElement element, Transform transform);

            Task RenderUIElementAsync(UIElement element, Transform transform, int width, int height);

            Task WaitForCompletionAsync();
        }
    }
}
