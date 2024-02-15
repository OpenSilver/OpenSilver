
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

namespace System.Windows.Media.Imaging
{
    /// <summary>
    /// Specifies initialization options for a bitmap image.
    /// </summary>
    public enum BitmapCreateOptions
    {
        /// <summary>
        /// No initialization options are specified. This is the NOT the default value for
        /// the <see cref="BitmapImage.CreateOptions"/> property in Silverlight or Silverlight 
        /// for Windows Phone (<see cref="DelayCreation"/> is the default).
        /// </summary>
        None = 0,
        /// <summary>
        /// Causes a <see cref="BitmapSource"/> object to delay initialization until it 
        /// is necessary. This is useful when dealing with collections of images. This is 
        /// the default value of the <see cref="BitmapImage.CreateOptions"/> property in 
        /// Silverlight and Silverlight for Windows Phone.
        /// </summary>
        DelayCreation = 2,
        /// <summary>
        /// Initializes images without using an existing image cache. Any existing entries
        /// in the image cache are replaced, even if they share the same URI. This option
        /// should only be selected when images in a cache need to be refreshed.
        /// </summary>
        IgnoreImageCache = 8,
        /// <summary>
        /// Causes a <see cref="BitmapSource"/> to be initialized as soon as it is declared. 
        /// This option uses the image cache for previously used URIs. If an image is not in 
        /// the image cache, the image will be downloaded and decoded on a separate background 
        /// thread.
        /// </summary>
        BackgroundCreation = 16
    }
}
