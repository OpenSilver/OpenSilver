
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
using System.ComponentModel;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Provides an object source type for Source and ImageSource.
    /// </summary>
    [TypeConverter(typeof(ImageSourceConverter))]
    public abstract class ImageSource : DependencyObject
    {
        internal ImageSource() { }

        internal abstract Task<string> GetDataStringAsync(UIElement parent);
    }
}