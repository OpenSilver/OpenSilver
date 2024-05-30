
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

using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xaml.Markup;

namespace System.Windows
{
    /// <summary>
    /// Represents the visual appearance of the control when it is in a specific state.
    /// </summary>
    [ContentProperty(nameof(Storyboard))]
    [RuntimeNameProperty(nameof(Name))]
    public sealed class VisualState : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualState"/> class.
        /// </summary>
        public VisualState() { }

        /// <summary>
        /// Gets the name of the <see cref="VisualState"/>.
        /// </summary>
        /// <returns>
        /// The name of the <see cref="VisualState"/>.
        /// </returns>
        public string Name
        {
            get => (string)GetValue(FrameworkElement.NameProperty);
            [EditorBrowsable(EditorBrowsableState.Never)]
            set => SetValueInternal(FrameworkElement.NameProperty, value);
        }

        private static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.Register(
                nameof(Storyboard),
                typeof(Storyboard),
                typeof(VisualState),
                null);

        /// <summary>
        /// Gets or sets a <see cref="Storyboard"/> that defines the appearance of the control 
        /// when it is the state that is represented by the <see cref="VisualState"/>.
        /// </summary>
        /// <returns>
        /// A Storyboard that defines the appearance of the control when it is the state that 
        /// is represented by the <see cref="VisualState"/>.
        /// </returns>
        public Storyboard Storyboard
        {
            get => (Storyboard)GetValue(StoryboardProperty);
            set => SetValueInternal(StoryboardProperty, value);
        }
    }
}
