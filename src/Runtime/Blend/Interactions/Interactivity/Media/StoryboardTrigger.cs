// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;
using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Expression.Interactivity.Media
{
    /// <summary>
    /// An abstract class that provides the ability to target a Storyboard.
    /// </summary>
    /// <remarks>
    /// For Trigger authors, this class provides a standard way to target a Storyboard. Design tools may choose to provide a 
    /// special editing experience for classes that inherit from this trigger, thereby improving the designer experience. 
    /// </remarks>
    public abstract class StoryboardTrigger : TriggerBase<DependencyObject>
    {
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(StoryboardTrigger),
#if __WPF__
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnStoryboardChanged)));
#else
            new PropertyMetadata(new PropertyChangedCallback(OnStoryboardChanged)));
#endif

        /// <summary>
        /// The targeted Storyboard. This is a dependency property.
        /// </summary>
        public Storyboard Storyboard
        {
            get { return (Storyboard)this.GetValue(StoryboardProperty); }
            set { this.SetValue(StoryboardProperty, value); }
        }

        private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            StoryboardTrigger storyboardTrigger = sender as StoryboardTrigger;
            if (storyboardTrigger != null)
            {
                storyboardTrigger.OnStoryboardChanged(args);
            }
        }

        /// <summary>
        /// This method is called when the Storyboard property is changed.
        /// </summary>
        protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
        }
    }

    /// <summary>
    /// A trigger that listens for the completion of a Storyboard.
    /// </summary>
    public class StoryboardCompletedTrigger : StoryboardTrigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoryboardCompletedTrigger"/> class.
        /// </summary>
        public StoryboardCompletedTrigger()
        {
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (this.Storyboard != null)
            {
                this.Storyboard.Completed -= this.Storyboard_Completed;
            }
        }

        protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
            Storyboard oldStoryboard = args.OldValue as Storyboard;
            Storyboard newStoryboard = args.NewValue as Storyboard;

            if (oldStoryboard != newStoryboard)
            {
                if (oldStoryboard != null)
                {
                    oldStoryboard.Completed -= this.Storyboard_Completed;
                }
                if (newStoryboard != null)
                {
                    newStoryboard.Completed += this.Storyboard_Completed;
                }
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            this.InvokeActions(e);
        }
    }
}
