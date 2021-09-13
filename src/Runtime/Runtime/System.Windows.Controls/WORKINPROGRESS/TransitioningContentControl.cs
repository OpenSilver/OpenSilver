
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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    [OpenSilver.NotImplemented]
    public class TransitioningContentControl : ContentControl
    {
        [OpenSilver.NotImplemented]
        public const string DefaultTransitionState = "DefaultTransition";

        [OpenSilver.NotImplemented]
        public bool IsTransitioning
        {
            get { return (bool)GetValue(IsTransitioningProperty); }
            private set { SetValue(IsTransitioningProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty IsTransitioningProperty =
                DependencyProperty.Register(
                    "IsTransitioning",
                    typeof(bool),
                    typeof(TransitioningContentControl),
                    null);

        [OpenSilver.NotImplemented]
        public string Transition
        {
            get { return GetValue(TransitionProperty) as string; }
            set { SetValue(TransitionProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TransitionProperty =
                DependencyProperty.Register(
                    "Transition",
                    typeof(string),
                    typeof(TransitioningContentControl),
                    null);

        [OpenSilver.NotImplemented]
        public bool RestartTransitionOnContentChange
        {
            get { return (bool)GetValue(RestartTransitionOnContentChangeProperty); }
            set { SetValue(RestartTransitionOnContentChangeProperty, value); }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RestartTransitionOnContentChangeProperty =
                DependencyProperty.Register(
                    "RestartTransitionOnContentChange",
                    typeof(bool),
                    typeof(TransitioningContentControl),
                    null);

        [OpenSilver.NotImplemented]
        protected virtual void OnRestartTransitionOnContentChangeChanged(bool oldValue, bool newValue)
        {
        }

        [OpenSilver.NotImplemented]
        public event RoutedEventHandler TransitionCompleted;

        [OpenSilver.NotImplemented]
        public TransitioningContentControl()
        {
        }

        [OpenSilver.NotImplemented]
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
        }

        [OpenSilver.NotImplemented]
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
        }

        [OpenSilver.NotImplemented]
        public void AbortTransition()
        {
        }
    }
}
