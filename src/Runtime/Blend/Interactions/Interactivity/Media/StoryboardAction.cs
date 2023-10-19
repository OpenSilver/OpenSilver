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
    /// For action authors, this class provides a standard way to target a Storyboard. Design tools may choose to provide a 
    /// special editing experience for classes that inherit from this action, thereby improving the designer experience.
    /// </remarks>
    public abstract class StoryboardAction : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard", typeof(Storyboard), typeof(StoryboardAction),
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
            StoryboardAction storyboardAction = sender as StoryboardAction;
            if (storyboardAction != null)
            {
                storyboardAction.OnStoryboardChanged(args);
            }
        }

        /// <summary>
        /// This method is called when the Storyboard property is changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
        }
    }

    public enum ControlStoryboardOption
    {
        Play,
        Stop,
        TogglePlayPause,
        Pause,
        Resume,
        SkipToFill
    }

    /// <summary>
    /// An action that will change the state of a targeted storyboard when invoked.
    /// </summary>
    [CLSCompliant(false)]
    public class ControlStoryboardAction : StoryboardAction
    {
        private bool isPaused;

#if __WPF__
        public static readonly DependencyProperty ControlStoryboardProperty = DependencyProperty.Register("ControlStoryboardOption", typeof(ControlStoryboardOption), typeof(ControlStoryboardAction));
#else
        public static readonly DependencyProperty ControlStoryboardProperty = DependencyProperty.Register("ControlStoryboardOption", typeof(ControlStoryboardOption), typeof(ControlStoryboardAction), null);
#endif

        public ControlStoryboardAction()
        {
        }

        public ControlStoryboardOption ControlStoryboardOption
        {
            get { return (ControlStoryboardOption)this.GetValue(ControlStoryboardProperty); }
            set { this.SetValue(ControlStoryboardProperty, value); }
        }

        /// <summary>
        /// This method is called when some criteria is met and the action should be invoked. This method will attempt to 
        /// change the targeted storyboard in a way defined by the ControlStoryboardOption.
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject != null && this.Storyboard != null)
            {
                switch (this.ControlStoryboardOption)
                {
                    case ControlStoryboardOption.Play:
                        this.Storyboard.Begin();
                        break;
                    case ControlStoryboardOption.Stop:
                        this.Storyboard.Stop();
                        break;
                    case ControlStoryboardOption.TogglePlayPause:
                        ClockState clockState = ClockState.Stopped;
#if __WPF__
                        bool isPaused = false;
#endif
                        try
                        {
                            clockState = this.Storyboard.GetCurrentState();
#if __WPF__
                            isPaused = this.Storyboard.GetIsPaused();
#endif
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        if (clockState == ClockState.Stopped)
                        {
#if !__WPF__
                            this.isPaused = false;
#endif
                            this.Storyboard.Begin();
                        }
                        else if (isPaused)
                        {
#if !__WPF__
                            this.isPaused = false;
#endif
                            this.Storyboard.Resume();
                        }
                        else
                        {
#if !__WPF__
                            this.isPaused = true;
#endif
                            this.Storyboard.Pause();
                        }
                        break;
                    case ControlStoryboardOption.Pause:
                        this.Storyboard.Pause();
                        break;
                    case ControlStoryboardOption.Resume:
                        this.Storyboard.Resume();
                        break;
                    case ControlStoryboardOption.SkipToFill:
                        this.Storyboard.SkipToFill();
                        break;
                }
            }
        }

#if !__WPF__
        protected override void OnStoryboardChanged(DependencyPropertyChangedEventArgs args)
        {
            base.OnStoryboardChanged(args);
            this.isPaused = false;
        }
#endif
    }
}
