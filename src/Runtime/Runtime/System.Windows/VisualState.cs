
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
using System.Collections.Generic;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents the visual appearance of the control when it is in a specific
    /// state.
    /// </summary>
    [ContentProperty("Storyboard")]
    public sealed partial class VisualState : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualState"/> class.
        /// </summary>
        public VisualState() { }

        /// <summary>
        /// Gets the name of the VisualState.
        /// </summary>
        public string Name
        {
            get;
            set; //todo: this was originally not public (but we need it in the compiler)
        }

        private static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.Register(
                nameof(Storyboard),
                typeof(Storyboard),
                typeof(VisualState),
                null);

        /// <summary>
        /// Gets or sets a <see cref="Media.Animation.Storyboard"/> that defines the appearance 
        /// of the control when it is the state that is represented by the <see cref="VisualState"/>.
        /// </summary>
        public Storyboard Storyboard
        {
            get => (Storyboard)GetValue(StoryboardProperty);
            set => SetValue(StoryboardProperty, value);
        }

        internal Dictionary<Tuple<string, string>, Timeline> GetPropertiesChangedByStoryboard()
        {
            if (Storyboard != null)
            {
                return Storyboard.GetPropertiesChanged();
            }
            return null;
        }

        internal Dictionary<Tuple<string, string>, Timeline> ApplyStoryboard(FrameworkElement frameworkElement, bool useTransitions)
        {
            Dictionary<Tuple<string, string>, Timeline> propertiesChanged = null;
            if (Storyboard != null)
            {
                Storyboard.Begin(frameworkElement);
                propertiesChanged = Storyboard.GetPropertiesChanged();

                //foreach (Timeline timeLine in Storyboard.Children)
                //{
                //    timeLine.Apply(frameworkElement, useTransitions, INTERNAL_Group.Name); //note: the "true" has no meaning, it's only here because hashsets do not work yet.
                //}
            }
            return propertiesChanged;
        }

        internal void StopStoryBoard(FrameworkElement frameworkElement)
        {
            if (Storyboard != null)
            {
                Storyboard.Stop(frameworkElement);
            }
        }

        internal VisualStateGroup INTERNAL_Group = null;
    }
}
