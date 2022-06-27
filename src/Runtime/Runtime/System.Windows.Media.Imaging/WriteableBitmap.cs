using System.Windows.Media;
using System.Windows;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSHTML5.Internal;

#if MIGRATION
using System.Windows.Controls;
namespace System.Windows.Media.Imaging
#else
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
namespace Windows.UI.Xaml.Media.Imaging
#endif
{
    public sealed class WriteableBitmap : BitmapSource
    {
        private int[] _pixels;
        private int _pixelWidth;
        private int _pixelHeight;
        private int _pixelArrayLength;
        private TaskCompletionSource<bool> _taskCompletion;

        private const string JS_FillBuffer = @"
                    document.WB_Fill32Buffer = function(wasmArray) {
                        const dataPtr = Blazor.platform.getArrayEntryPtr(wasmArray, 0, 4);
                        const length = Blazor.platform.getArrayLength(wasmArray);
                        var shorts = new Uint32Array(Module.HEAP32.buffer, dataPtr, length);
                        shorts.set(new Uint32Array(document.WB_TempPixelData), 0);
                    }";

        private const string JS_CopyBuffer = @"
                    document.WB_Copy32Buffer = function(wasmArray) {
                        const dataPtr = Blazor.platform.getArrayEntryPtr(wasmArray, 0, 4);
                        const length = Blazor.platform.getArrayLength(wasmArray);
                        let tmp = new Uint8Array(Module.HEAP8.buffer, dataPtr, length * 4);
                        document.WB_TempBufferData = new Uint8ClampedArray(tmp);
                        return 0;
                    }";
        private const string JS_CtxSmooth = @"
                     function smoothCanvasContext(ctx) {
                        ctx.imageSmoothingEnabled = true;
                        ctx.webkitImageSmoothingEnabled = true;
                        ctx.mozImageSmoothingEnabled = true;
                        ctx.msImageSmoothingEnabled = true;
                    }";
        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Media.Imaging.WriteableBitmap" /> class using the provided dimensions.</summary>
        /// <param name="pixelWidth">The width of the bitmap.</param>
        /// <param name="pixelHeight">The height of the bitmap.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="pixelWidth" /> or <paramref name="pixelHeight" /> is zero or less.</exception>
        public WriteableBitmap(int pixelWidth, int pixelHeight)
        {
            if (pixelWidth < 0)
                throw new ArgumentOutOfRangeException(nameof(pixelWidth));

            if (pixelHeight < 0)
                throw new ArgumentOutOfRangeException(nameof(pixelHeight));

            _pixelWidth = pixelWidth;
            _pixelHeight = pixelHeight;
            _pixelArrayLength = _pixelWidth * _pixelHeight;
            _pixels = new int[_pixelArrayLength];
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Media.Imaging.WriteableBitmap" /> class using the provided <see cref="T:System.Windows.Media.Imaging.BitmapSource" />.</summary>
        /// <param name="source">The <see cref="T:System.Windows.Media.Imaging.BitmapSource" /> to use for initialization. </param>
        public WriteableBitmap(BitmapSource source)
        {
            if (source == null)
                throw new ArgumentOutOfRangeException(nameof(source));

            _taskCompletion = new TaskCompletionSource<bool>();

            var imageSrc = Image.GetImageTagSrc(source);
            var javascript = JS_CtxSmooth + JS_FillBuffer + @"
                    var imageView = new Image();
                    imageView.src = $0;
                    imageView.onload = function() {
                        try {      
                            let canvas = document.createElement('canvas'); 
                            canvas.height = imageView.height;
                            canvas.width = imageView.width;
                            let ctx = canvas.getContext('2d');
                            smoothCanvasContext(ctx);
                            ctx.drawImage(imageView, 0, 0);
                            let imgData = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
                            document.WB_TempPixelData = new Uint32Array(imgData.data.buffer);
                            $1(imgData.data.length, imgData.width, imgData.height);
                        }
                        catch (err) {
                            console.error(err);
                            $2(err.message);
                        }
                    }";
            Action<int, int, int> successCallback = OnImageDataLoadedCallback;
            Action<string> errorCallback = OnErrorCallabck;
            OpenSilver.Interop.ExecuteJavaScript(javascript, imageSrc, successCallback, errorCallback);
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Media.Imaging.WriteableBitmap" /> class using the provided element and transform.</summary>
        /// <param name="element">The desired element to be rendered within the bitmap. </param>
        /// <param name="transform">The transform the user wants to apply to the element as the last step before drawing into the bitmap. This is particularly interesting for you if you want the bitmap to respect its transform. This value can be null.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element" /> is null.</exception>
        public WriteableBitmap(UIElement element, Transform transform)
        {
            ElementToImageData(element, transform, OnImageDataLoadedCallback, OnErrorCallabck);
        }

        private void ElementToImageData(UIElement element, Transform transform, Action<int, int, int> successCallback, Action<string> errorCallback)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _taskCompletion = new TaskCompletionSource<bool>();

            var outerDiv = (INTERNAL_HtmlDomElementReference)OpenSilver.Interop.GetDiv(element);
            var javascript = JS_CtxSmooth + JS_FillBuffer + @"
                    let element = document.querySelector('#' + $0);
                    html2canvas(element, {scale: window.devicePixelRatio}).then(function (canvas) {
                        try {                      
                            if ($3) {
                                var mainCanvas = document.createElement('canvas'); 
                                let rect = element.getBoundingClientRect();
                                mainCanvas.width = rect.width * window.devicePixelRatio;
                                mainCanvas.height = rect.height * window.devicePixelRatio;
                                let mainCtx = mainCanvas.getContext('2d');
                                smoothCanvasContext(mainCtx);
                                mainCtx.scale(window.devicePixelRatio, window.devicePixelRatio);
                                mainCanvas.style.width = rect.width + 'px';
                                mainCanvas.style.height = rect.height + 'px';
                                mainCtx.clearRect(0, 0, $0.width, $0.height);
                                mainCtx.setTransform($4, $5, $6, $7, $8 * window.devicePixelRatio, $9 * window.devicePixelRatio);
                                mainCtx.drawImage(canvas, 0, 0);
                                canvas = mainCanvas;
                            }

                            let ctx = canvas.getContext('2d');
                            smoothCanvasContext(ctx);
                            let imgData = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
                            document.WB_TempPixelData = new Uint32Array(imgData.data.buffer);
                            $1(imgData.data.length, imgData.width, imgData.height);
                        }
                        catch (err) {
                            console.error(err);
                            $2(err.message);
                        }
                    });";

            if (transform == null || transform.ValueInternal == null)
            {
                OpenSilver.Interop.ExecuteJavaScript(javascript, outerDiv.UniqueIdentifier, successCallback, errorCallback, false);
            }
            else
            {
                var m = transform.ValueInternal;
                OpenSilver.Interop.ExecuteJavaScript(javascript, outerDiv.UniqueIdentifier, successCallback, errorCallback, true, m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
            }
        }

        private Task<bool> WaitForPngDataGeneration()
        {
            if (_taskCompletion == null)
            {
                return Task.FromResult(true);
            }
            else
            {
                return _taskCompletion.Task;
            }
        }
        /// <summary>
        /// User must call WaitToInitialize after instantiation in order to load the buffer
        /// </summary>
        /// <returns></returns>
        public Task<bool> WaitToInitialize()
        {
            return WaitForPngDataGeneration();
        }

        private void OnErrorCallabck(string errMsg)
        {
            _taskCompletion?.SetResult(false);
        }

        private void OnImageDataLoadedCallback(int arrayLength, int width, int height)
        {
            _pixelWidth = width;
            _pixelHeight = height;
            _pixels = new int[arrayLength / 4];
            _pixelArrayLength = _pixels.Length;
            DotNetForHtml5.Core.INTERNAL_Simulator.JavaScriptExecutionHandler.InvokeUnmarshalled<int[], object>("document.WB_Fill32Buffer", _pixels);
            Invalidate();
            _taskCompletion?.SetResult(true);
        }

        /// <summary>Gets an array representing the 2-D texture of the bitmap.</summary>
        /// <returns>An array of integers representing the 2-D texture of the bitmap.</returns>
        public int[] Pixels
        {
            get
            {
                return _pixels;
            }
        }

        internal override int PixelHeightInternal
        {
            get
            {
                return _pixelHeight;
            }
        }

        internal override int PixelWidthInternal
        {
            get
            {
                return _pixelWidth;
            }
        }
        
        public Task<bool> WaitToRender()
        {
            return WaitForPngDataGeneration();
        }

        private void RenderElementToImageData(UIElement element, Transform transform, Action<int, int, int> successCallback, Action<string> errorCallback)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _taskCompletion = new TaskCompletionSource<bool>();
            OpenSilver.Interop.ExecuteJavaScript(JS_FillBuffer + JS_CopyBuffer);
            DotNetForHtml5.Core.INTERNAL_Simulator.JavaScriptExecutionHandler.InvokeUnmarshalled<int[], int>("document.WB_Copy32Buffer", _pixels);

            var outerDiv = (INTERNAL_HtmlDomElementReference)OpenSilver.Interop.GetDiv(element);
            var javascript = JS_CtxSmooth + @"
                    let element = document.querySelector('#' + $0);
                    html2canvas(element, {scale: window.devicePixelRatio}).then(function (canvas) {
                        try {          
                            var cnvs = document.createElement('canvas'); 
                            const pixelWidth = $3, pixelHeight = $4;
                            cnvs.width = pixelWidth;
                            cnvs.height = pixelHeight;
                            let ctx = cnvs.getContext('2d');
                            smoothCanvasContext(ctx);
                            ctx.scale(window.devicePixelRatio, window.devicePixelRatio);
                            cnvs.style.width = (pixelWidth/window.devicePixelRatio) + 'px';
                            cnvs.style.height = (pixelHeight/window.devicePixelRatio) + 'px';
                            let imgData = new ImageData(document.WB_TempBufferData, pixelWidth, pixelHeight);
                            ctx.putImageData(imgData, 0, 0);
                            ctx.setTransform($5, $6, $7, $8, $9 * window.devicePixelRatio, $10 * window.devicePixelRatio);
                            ctx.drawImage(canvas, 0, 0);
                            imgData = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
                            document.WB_TempPixelData = new Uint32Array(imgData.data.buffer);
                            $1(imgData.data.length, imgData.width, imgData.height);
                        }
                        catch (err) {
                            console.error(err);
                            $2(err.message);
                        }
                    });";

            var m = transform.ValueInternal;
            OpenSilver.Interop.ExecuteJavaScript(javascript, outerDiv.UniqueIdentifier, successCallback, 
                errorCallback, PixelWidth, PixelHeight, m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
        }

        private void OnRenderDataLoadedCallback(int arrayLength, int width, int height)
        {
            DotNetForHtml5.Core.INTERNAL_Simulator.JavaScriptExecutionHandler.InvokeUnmarshalled<int[], object>("document.WB_Fill32Buffer", _pixels);
            _taskCompletion.SetResult(true);
        }

        /// <summary>Renders an element within the bitmap.</summary>
        /// <param name="element">The element to be rendered within the bitmap.</param>
        /// <param name="transform">The transform to apply to the element before drawing into the bitmap. If an empty transform is supplied, the bits representing the element show up at the same offset as if they were placed within their parent.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="element" /> is null.</exception>
        public void Render(UIElement element, Transform transform)
        {
            if (transform == null && element.INTERNAL_VisualParent != null)
            {
                try
                {
                    var parentTransform = element.TransformToVisual((UIElement)element.INTERNAL_VisualParent);
                    var coordInParent = parentTransform.Transform(new Point(0, 0));
                    transform = new TranslateTransform() { X = coordInParent.X, Y = coordInParent.Y };
                }
                catch { }
            }

            RenderElementToImageData(element, transform, OnRenderDataLoadedCallback, OnErrorCallabck);
        }

        /// <summary>Requests a draw or redraw of the entire bitmap.</summary>
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
    }
}
