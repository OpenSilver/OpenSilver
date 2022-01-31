using System;

#if MIGRATION
using System.Windows;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Microsoft.Expression.Interactivity.Media
{
    [OpenSilver.NotImplemented]
    public class StoryboardCompletedTrigger : StoryboardTrigger
    {
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (Storyboard != null)
            {
                Storyboard.Completed -= Storyboard_Completed;
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
                    oldStoryboard.Completed -= Storyboard_Completed;
                }
                if (newStoryboard != null)
                {
                    newStoryboard.Completed += Storyboard_Completed;
                }
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            InvokeActions(e);
        }
    }
}
