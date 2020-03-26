

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

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if WORKINPROGRESS
    /// <summary>Represents a collection of <see cref="T:System.Windows.Media.Animation.ColorKeyFrame" /> objects that can be individually accessed by index. </summary>
    public sealed partial class ColorKeyFrameCollection : PresentationFrameworkCollection<ColorKeyFrame>
    {
    }
#endif
}
