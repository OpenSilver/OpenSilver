
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Contains mutually exclusive VisualState objects and VisualTransition objects
    /// that are used to go from one state to another.
    /// </summary>
    [ContentProperty("States")]
    public sealed partial class VisualStateGroup : DependencyObject
    {
        ///// <summary>
        ///// Initializes a new instance of the VisualStateGroup class.
        ///// </summary>
        //public VisualStateGroup();

        VisualState _currentState;
        /// <summary>
        /// Gets the most recently set VisualState from a successful call to the GoToState
        /// method.
        /// </summary>
        public VisualState CurrentState
        {
            get { return _currentState; }
            internal set { _currentState = value; }
        }

        string _name;
        /// <summary>
        /// Gets the name of the VisualStateGroup.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; } //todo: this was originally not public (but we need it in the compiler)
        }

        private List<VisualState> _states = new List<VisualState>();
        /// <summary>
        /// Gets the collection of mutually exclusive VisualState objects.
        /// </summary>
        public IList States { get { return _states; } } // Note: this returns "IList" instead of "IList<VisualState>" so that the XAML Code Editor does not complain.

#if WORKINPROGRESS

        private IList<VisualTransition> _transitions;
        /// <summary>
        /// Gets the collection of VisualTransition objects.
        /// </summary>
        public IList<VisualTransition> Transitions
        {
            get
            {
                if(_transitions == null)
                {
                    _transitions = new List<VisualTransition>();
                }
                return _transitions;
            }
        }
#endif

        ///// <summary>
        ///// Occurs after a control changes into a different state.
        ///// </summary>
        //public event VisualStateChangedEventHandler CurrentStateChanged;

        ///// <summary>
        ///// Occurs when a control begins changing into a different state.
        ///// </summary>
        //public event VisualStateChangedEventHandler CurrentStateChanging;
    }
}