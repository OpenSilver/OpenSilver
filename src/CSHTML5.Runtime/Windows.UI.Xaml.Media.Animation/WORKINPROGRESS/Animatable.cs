
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
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
#if WORKINPROGRESS
    #region Not Supported yet
    public abstract class Animatable : Freezable, IAnimatable
    {
        //public void ApplyAnimationClock(DependencyProperty dp, AnimationClock clock)
        //{
            
        //}

        //public void ApplyAnimationClock(DependencyProperty dp, AnimationClock clock, HandoffBehavior handoffBehavior)
        //{
            
        //}

        //public void BeginAnimation(DependencyProperty dp, AnimationTimeline animation)
        //{
            
        //}

        //public void BeginAnimation(DependencyProperty dp, AnimationTimeline animation, HandoffBehavior handoffBehavior)
        //{
            
        //}

        //public object GetAnimationBaseValue(DependencyProperty dp)
        //{
        //    return null;
        //}
    }
    #endregion
#endif
}
