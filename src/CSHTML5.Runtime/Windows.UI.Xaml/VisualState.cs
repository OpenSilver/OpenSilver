
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        ///// <summary>
        ///// Initializes a new instance of the VisualState class.
        ///// </summary>
        //public VisualState();

        private string _name;
        /// <summary>
        /// Gets the name of the VisualState.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; } //todo: this was originally not public (but we need it in the compiler)
        }

        private Storyboard _storyboard = null;
        /// <summary>
        /// Gets or sets a Storyboard that defines the appearance of the control when
        /// it is the state that is represented by the VisualState.
        /// </summary>
        public Storyboard Storyboard
        {
            get { return _storyboard; }
            set { _storyboard = value; }
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
                Storyboard.Begin(frameworkElement, useTransitions, INTERNAL_Group.Name, isVisualStateChange:true);
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
                Storyboard.Stop(frameworkElement, INTERNAL_Group.Name);
            }
        }

        internal VisualStateGroup INTERNAL_Group = null;
    }
}
