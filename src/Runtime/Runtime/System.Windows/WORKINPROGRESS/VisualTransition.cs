

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
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace System.Windows
{
    #region Not supported yet
    /// <summary>
    /// Defines a transition between VisualStates.
    /// </summary>
    [ContentProperty("Storyboard")]
    [OpenSilver.NotImplemented]
    public partial class VisualTransition : DependencyObject
    {
        /// <summary>Gets or sets the <see cref="T:System.Windows.Media.Animation.Storyboard" /> that occurs when the transition occurs.</summary>
        /// <returns>The <see cref="T:System.Windows.Media.Animation.Storyboard" /> that occurs when the transition occurs.</returns>
        [OpenSilver.NotImplemented]
        public Storyboard Storyboard { get; set; }

        /// <summary>Gets or sets the amount of time it takes to move from one state to another.</summary>
        /// <returns>The amount of time it takes to move from one state to another.</returns>
        [OpenSilver.NotImplemented]
        public Duration GeneratedDuration { get; set; }

        /// <summary>
        /// Easing Function for the transition
        /// </summary>
        [OpenSilver.NotImplemented]
        public IEasingFunction GeneratedEasingFunction { get; set; }

        /// <summary>Gets or sets the name of the <see cref="T:System.Windows.VisualState" /> to transition to.</summary>
        /// <returns>The name of the <see cref="T:System.Windows.VisualState" /> to transition to.</returns>
        [OpenSilver.NotImplemented]
        public string To { get; set; }

        /// <summary>Gets or sets the name of the <see cref="T:System.Windows.VisualState" /> to transition from.</summary>
        /// <returns>The name of the <see cref="T:System.Windows.VisualState" /> to transition from.</returns>
        [OpenSilver.NotImplemented]
        public string From { get; set; }
    }
    #endregion
}
