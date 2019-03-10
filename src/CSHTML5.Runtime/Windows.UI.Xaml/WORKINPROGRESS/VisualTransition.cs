
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
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
#if WORKINPROGRESS
    #region Not supported yet

    /// <summary>
    /// Defines a transition between VisualStates.
    /// </summary>
    [ContentProperty("Storyboard")]
    public class VisualTransition : DependencyObject
    {
        /// <summary>Gets or sets the <see cref="T:System.Windows.Media.Animation.Storyboard" /> that occurs when the transition occurs.</summary>
        /// <returns>The <see cref="T:System.Windows.Media.Animation.Storyboard" /> that occurs when the transition occurs.</returns>
        public Storyboard Storyboard
        {
            get { return (Storyboard)this.GetValue(VisualTransition.StoryboardProperty); }
            set { this.SetValue(VisualTransition.StoryboardProperty, (DependencyObject)value); }
        }

        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(VisualTransition), null);

        /// <summary>Gets or sets the amount of time it takes to move from one state to another.</summary>
        /// <returns>The amount of time it takes to move from one state to another.</returns>
        public Duration GeneratedDuration
        {
            get { return (Duration)this.GetValue(VisualTransition.GeneratedDurationProperty); }
            set { this.SetValue(VisualTransition.GeneratedDurationProperty, (object)value); }
        }

        public static readonly DependencyProperty GeneratedDurationProperty = DependencyProperty.Register("GeneratedDuration", typeof(Duration), typeof(VisualTransition), null);

        /// <summary>Gets or sets the name of the <see cref="T:System.Windows.VisualState" /> to transition to.</summary>
        /// <returns>The name of the <see cref="T:System.Windows.VisualState" /> to transition to.</returns>
        public string To
        {
            get { return (string)this.GetValue(VisualTransition.ToProperty); }
            set { this.SetValue(VisualTransition.ToProperty, (object)value);  }
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(string), typeof(VisualTransition), null);

        /// <summary>Gets or sets the name of the <see cref="T:System.Windows.VisualState" /> to transition from.</summary>
        /// <returns>The name of the <see cref="T:System.Windows.VisualState" /> to transition from.</returns>
        public string From
        {
            get { return (string)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(string), typeof(VisualTransition), new PropertyMetadata(0));

        
    }

    #endregion
#endif
}
