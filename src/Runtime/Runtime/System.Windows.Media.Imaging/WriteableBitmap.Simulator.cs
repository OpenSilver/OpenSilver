
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

#if MIGRATION
namespace System.Windows.Media.Imaging
#else
namespace Windows.UI.Xaml.Media.Imaging
#endif
{
    public sealed partial class WriteableBitmap
    {
        private sealed class WriteableBitmapSimulator : IWriteableBitmapImpl
        {
            private readonly WriteableBitmap _bitmap;

            public WriteableBitmapSimulator(WriteableBitmap bitmap)
            {
                Debug.WriteLine("WriteableBitmap is not implemented in the simulator.");

                Debug.Assert(bitmap != null);
                _bitmap = bitmap;
            }

            public Task CreateFromBitmapSourceAsync(BitmapSource source) => Task.CompletedTask;

            public Task CreateFromUIElementAsync(UIElement element, Transform transform) => Task.CompletedTask;

            public Task RenderUIElementAsync(UIElement element, Transform transform, int width, int height) => Task.CompletedTask;

            public Task WaitForCompletionAsync() => Task.CompletedTask;
        }
    }
}
