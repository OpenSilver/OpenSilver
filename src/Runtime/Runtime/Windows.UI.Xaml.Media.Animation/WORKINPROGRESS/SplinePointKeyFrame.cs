

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
    /// <summary>
    /// This class is used as part of a PointKeyFrameCollection in conjunction
    /// with a PointAnimationUsingKeyFrames to animate a Point property value
    /// along a set of key frames.
    ///
    /// </summary>
    public sealed partial class SplinePointKeyFrame : PointKeyFrame
    {
    }
#endif
}

