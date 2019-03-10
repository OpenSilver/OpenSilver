
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if WORKINPROGRESS
    /// <summary>
    /// This class is used to animate a Color property value along a set
    /// of key frames.
    /// </summary>
    [ContentProperty("KeyFrames")]
    public class ColorAnimationUsingKeyFrames : Timeline
    {
        private ColorKeyFrameCollection _keyFrames;
        // Summary:
        //     Gets the collection of System.Windows.Media.Animation.ColorKeyFrame objects
        //     that define the animation.
        //
        // Returns:
        //     The collection of System.Windows.Media.Animation.ColorKeyFrame objects that
        //     define the animation. The default is an empty collection.
        public ColorKeyFrameCollection KeyFrames
        {
            get
            {
                if (_keyFrames == null)
                {
                    _keyFrames = new ColorKeyFrameCollection();
                }
                return _keyFrames;
            }
        }
    }
#endif
}
