
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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
    public partial class VisualTransition : DependencyObject
    {
        /// <summary>Gets or sets the <see cref="T:System.Windows.Media.Animation.Storyboard" /> that occurs when the transition occurs.</summary>
        /// <returns>The <see cref="T:System.Windows.Media.Animation.Storyboard" /> that occurs when the transition occurs.</returns>
        public Storyboard Storyboard { get; set; }

        /// <summary>Gets or sets the amount of time it takes to move from one state to another.</summary>
        /// <returns>The amount of time it takes to move from one state to another.</returns>
        public Duration GeneratedDuration { get; set; }

        /// <summary>
        /// Easing Function for the transition
        /// </summary>
        public IEasingFunction GeneratedEasingFunction { get; set; }

        /// <summary>Gets or sets the name of the <see cref="T:System.Windows.VisualState" /> to transition to.</summary>
        /// <returns>The name of the <see cref="T:System.Windows.VisualState" /> to transition to.</returns>
        public string To { get; set; }

        /// <summary>Gets or sets the name of the <see cref="T:System.Windows.VisualState" /> to transition from.</summary>
        /// <returns>The name of the <see cref="T:System.Windows.VisualState" /> to transition from.</returns>
        public string From { get; set; }
    }
    #endregion
#endif
}
