
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
using System.Threading.Tasks;

namespace System.Windows.Media.Imaging
{
    public sealed partial class WriteableBitmap
    {
        private sealed class WriteableBitmapWasm : IWriteableBitmapImpl
        {
            private readonly WriteableBitmap _bitmap;

            private TaskCompletionSource<object> _taskCompletion;

            public WriteableBitmapWasm(WriteableBitmap bitmap)
            {
                Debug.Assert(bitmap != null);
                _bitmap = bitmap;
            }

            public Task CreateFromBitmapSourceAsync(BitmapSource source)
            {
                _taskCompletion = new TaskCompletionSource<object>();

                source.GetDataStringAsync(source.InheritanceContext as UIElement).AsTask().ContinueWith(t =>
                {
                    OpenSilver.Interop.JavaScriptRuntime.Flush();
                    OpenSilver.Interop.NativeMethods.WriteableBitmap_CreateFromBitmapSource(t.Result, OnImageDataLoadSuccess, OnImageDataLoadError);
                });

                return _taskCompletion.Task;
            }

            public Task CreateFromUIElementAsync(UIElement element, Transform transform)
            {
                double width = element.RenderSize.Width;
                double height = element.RenderSize.Height;

                if (transform is not null)
                {
                    Rect transformedBounds = transform.TransformBounds(new Rect(0, 0, width, height));
                    width = transformedBounds.Width;
                    height = transformedBounds.Height;
                }

                return RenderUIElementAsync(
                    element,
                    transform,
                    (int)Math.Round(width),
                    (int)Math.Round(height),
                    OnImageDataLoadSuccess,
                    OnImageDataLoadError);
            }

            public Task RenderUIElementAsync(UIElement element, Transform transform, int width, int height)
                => RenderUIElementAsync(element, transform, width, height, OnRenderDataSuccess, OnRenderDataError);

            public Task WaitForCompletionAsync() => _taskCompletion?.Task ?? Task.CompletedTask;

            private Task RenderUIElementAsync(
                UIElement element,
                Transform transform,
                int width,
                int height,
                Action<int, int, int> onSucess,
                Action<string> onError)
            {
                if (!element.OuterDiv.IsConnected)
                {
                    return Task.CompletedTask;
                }

                _taskCompletion = new TaskCompletionSource<object>();

                OpenSilver.Interop.JavaScriptRuntime.Flush();
                OpenSilver.Interop.NativeMethods.WriteableBitmap_RenderUIElement(
                    element.OuterDiv.Uid,
                    width,
                    height,
                    transform is null ? string.Empty : MatrixTransform.MatrixToHtmlString(transform.Matrix),
                    onSucess,
                    onError);

                return _taskCompletion.Task;
            }

            private void OnImageDataLoadSuccess(int arrayLength, int width, int height)
            {
                _bitmap._pixels = new int[arrayLength / 4];
                FillBuffer(_bitmap);
                _bitmap.SetNaturalSize(width, height);

                _taskCompletion.SetResult(null);

                // Important: we must complete the task before invalidating the WriteableBitmap
                _bitmap.Invalidate();
            }

            private void OnImageDataLoadError(string errorMessage) => _taskCompletion.SetResult(null);

            private void OnRenderDataSuccess(int arrayLength, int width, int height)
            {
                FillBuffer(_bitmap);
                _taskCompletion.SetResult(null);
            }

            private void OnRenderDataError(string errorMessage) => _taskCompletion.SetResult(null);

            private static void FillBuffer(WriteableBitmap bitmap)
            {
                OpenSilver.Interop.JavaScriptRuntime.Flush();
                OpenSilver.Interop.NativeMethods.WriteableBitmap_FillBufferInt32(bitmap._pixels);

                if (bitmap._isSilverlightCompatibilityMode)
                {
                    for (int i = 0; i < bitmap._pixels.Length; i++)
                    {
                        bitmap._pixels[i] = SwapBytes(bitmap._pixels[i]);
                    }
                }
            }
        }
    }
}
