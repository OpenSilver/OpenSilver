
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

using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using CSHTML5.Internal;
using DotNetForHtml5.Core;

namespace System.Windows.Media.Imaging
{
    public sealed partial class WriteableBitmap
    {
        private sealed class WriteableBitmapWasm : IWriteableBitmapImpl
        {
            private readonly WriteableBitmap _bitmap;

            private TaskCompletionSource<object> _taskCompletion;
            private JavaScriptCallback _imageRenderedCallback;

            static WriteableBitmapWasm()
            {
                OpenSilver.Interop.ExecuteJavaScriptVoid(@"
document.WB_Fill32Buffer = function (wasmArray) {
  const dataPtr = Blazor.platform.getArrayEntryPtr(wasmArray, 0, 4);
  const length = Blazor.platform.getArrayLength(wasmArray);
  var shorts = new Uint32Array(Module.HEAP32.buffer, dataPtr, length);
  shorts.set(new Uint32Array(document.WB_TempPixelData), 0);
}

document.WB_Copy32Buffer = function (wasmArray) {
  const dataPtr = Blazor.platform.getArrayEntryPtr(wasmArray, 0, 4);
  const length = Blazor.platform.getArrayLength(wasmArray);
  let tmp = new Uint8Array(Module.HEAP8.buffer, dataPtr, length * 4);
  document.WB_TempBufferData = new Uint8ClampedArray(tmp);
  return 0;
}

document.WB_SmoothCanvasContext = function (ctx) {
  ctx.imageSmoothingEnabled = true;
  ctx.webkitImageSmoothingEnabled = true;
  ctx.mozImageSmoothingEnabled = true;
  ctx.msImageSmoothingEnabled = true;
}");
            }

            public WriteableBitmapWasm(WriteableBitmap bitmap)
            {
                Debug.Assert(bitmap != null);
                _bitmap = bitmap;
            }

            public Task CreateFromBitmapSourceAsync(BitmapSource source)
            {
                _taskCompletion = new TaskCompletionSource<object>();
                _imageRenderedCallback = JavaScriptCallback.Create(OnImageDataLoadedCallback);

                source.GetDataStringAsync(source.InheritanceContext as UIElement).AsTask().ContinueWith(t =>
                {
                    var data = t.Result;
                    var javascript = @"
var imageView = new Image();
imageView.src = $0;
imageView.onload = function() {
  try {      
    let canvas = document.createElement('canvas'); 
    canvas.height = imageView.height;
    canvas.width = imageView.width;
    let ctx = canvas.getContext('2d');
    document.WB_SmoothCanvasContext(ctx);
    ctx.drawImage(imageView, 0, 0);
    let imgData = ctx.getImageData(0, 0, ctx.canvas.width, ctx.canvas.height);
    document.WB_TempPixelData = new Uint32Array(imgData.data.buffer);
    $1(null, imgData.data.length, imgData.width, imgData.height);
  } catch (err) {
    console.error(err);
    $1(err.message, 0, 0, 0);
  }
}";

                    OpenSilver.Interop.ExecuteJavaScriptVoid(
                        javascript, flushQueue:false, 
                        data,
                        _imageRenderedCallback);
                });

                return _taskCompletion.Task;
            }

            public Task CreateFromUIElementAsync(UIElement element, Transform transform)
                => RenderUIElementAsync(element, transform, -1, -1, OnImageDataLoadedCallback);

            public Task RenderUIElementAsync(UIElement element, Transform transform, int width, int height)
                => RenderUIElementAsync(element, transform, width, height, OnRenderDataLoadedCallback);

            public Task WaitForCompletionAsync() => _taskCompletion?.Task ?? Task.CompletedTask;

            private Task RenderUIElementAsync(
                UIElement element,
                Transform transform,
                int width,
                int height,
                Action<string, int, int, int> callback)
            {
                if (element.OuterDiv is null)
                {
                    return Task.CompletedTask;
                }

                _taskCompletion = new TaskCompletionSource<object>();
                _imageRenderedCallback = JavaScriptCallback.Create(callback);

                INTERNAL_Simulator.WebAssemblyExecutionHandler.InvokeUnmarshalled<int[], int>(
                    "document.WB_Copy32Buffer", _bitmap._pixels);

                var javascript = @"(function (data) {
let element = document.getElementById(data.Id);
const currentTransform = element.style.transform;
element.style.transform = data.Transform;
html2canvas(element, {scale: 1}).then(function (canvas) {
  try {
    let ctx = canvas.getContext('2d');
    document.WB_SmoothCanvasContext(ctx);
    const w = data.Size.Width > -1 ? data.Size.Width : ctx.canvas.width;
    const h = data.Size.Height > -1 ? data.Size.Height : ctx.canvas.height;
    let imgData = ctx.getImageData(0, 0, w, h);
    document.WB_TempPixelData = new Uint32Array(imgData.data.buffer);
    $1(null, imgData.data.length, imgData.width, imgData.height);
  } catch (err) {
    console.error(err);
    $1(err.message, 0, 0, 0);
  }
});
element.style.transform = currentTransform;
})(JSON.parse($0));";

                var data = new WB_Data
                {
                    Id = element.OuterDiv.UniqueIdentifier,
                    Size = new WB_Size { Width = width, Height = height },
                };

                if (transform is null)
                {
                    data.Transform = "";
                }
                else
                {
                    Matrix m = transform.Matrix;
                    data.Transform = $"matrix({m.M11}, {m.M12}, {m.M21}, {m.M22}, {m.OffsetX}, {m.OffsetY})";
                }

                OpenSilver.Interop.ExecuteJavaScriptVoid(
                   javascript, flushQueue:false, 
                   JsonSerializer.Serialize(data),
                   _imageRenderedCallback);

                return _taskCompletion.Task;
            }

            private void OnImageDataLoadedCallback(string errorMessage, int arrayLength, int width, int height)
            {
                if (errorMessage is null)
                {
                    _bitmap._pixelWidth = width;
                    _bitmap._pixelHeight = height;
                    _bitmap._pixels = new int[arrayLength / 4];
                    INTERNAL_Simulator.WebAssemblyExecutionHandler.InvokeUnmarshalled<int[], object>(
                        "document.WB_Fill32Buffer", _bitmap._pixels);
                    _bitmap.Invalidate();
                }

                _imageRenderedCallback.Dispose();
                _imageRenderedCallback = null;
                _taskCompletion.SetResult(null);
            }

            private void OnRenderDataLoadedCallback(string errorMessage, int arrayLength, int width, int height)
            {
                if (errorMessage is null)
                {
                    INTERNAL_Simulator.WebAssemblyExecutionHandler.InvokeUnmarshalled<int[], object>(
                        "document.WB_Fill32Buffer", _bitmap._pixels);
                }

                _imageRenderedCallback.Dispose();
                _imageRenderedCallback = null;
                _taskCompletion.SetResult(null);
            }

            private struct WB_Data
            {
                public string Id { get; set; }
                public WB_Size Size { get; set; }
                public string Transform { get; set; }
            }

            private struct WB_Size
            {
                public int Width { get; set; }
                public int Height { get; set; }
            }
        }
    }
}
