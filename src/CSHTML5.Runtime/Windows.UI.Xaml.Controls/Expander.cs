
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
#if MIGRATION
using System.Windows;
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public class Expander : HeaderedContentControl
    {
        bool _isInToggleButtonClickHandler = false;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.Expander" /> class.
        /// </summary>
        public Expander()
        {
            //DefaultStyleKey = typeof(Expander);
            //Interaction = new InteractionHelper(this);

            // Set default style:
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(Expander);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultExpanderStyle.GetDefaultStyle());
#endif
        }


        /// <summary>
        /// The ExpanderButton template part is a templated ToggleButton that's used 
        /// to expand and collapse the ExpandSite, which hosts the content.
        /// </summary>
        private ToggleButton _expanderButton;


        /// <summary>
        /// The name of the ExpanderButton template part.
        /// </summary>
        private const string ElementExpanderButtonName = "ExpanderButton";

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="T:System.Windows.Controls.Expander" /> content window is
        /// visible.
        /// </summary>
        /// <value>
        /// True if the content window is expanded; otherwise, false. The
        /// default is false.
        /// </value>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.Expander.IsExpanded" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.Expander.IsExpanded" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty IsExpandedProperty =
                DependencyProperty.Register(
                        "IsExpanded",
                        typeof(bool),
                        typeof(Expander),
                        new PropertyMetadata(false, OnIsExpandedPropertyChanged));


        /// <summary>
        /// ExpandedProperty PropertyChangedCallback static function.
        /// </summary>
        /// <param name="d">Expander object whose Expanded property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Expander ctrl = (Expander)d;
            bool isExpanded = (bool)e.NewValue;

            //// Notify any automation peers of the expansion change
            //ExpanderAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(ctrl) as ExpanderAutomationPeer;
            //if (peer != null)
            //{
            //    peer.RaiseExpandCollapseAutomationEvent((bool)e.OldValue, isExpanded);
            //}

            if (isExpanded)
            {
                ctrl.OnExpanded();
            }
            else
            {
                ctrl.OnCollapsed();
            }
        }

        /// <summary>
        /// Raises the
        /// <see cref="E:System.Windows.Controls.Expander.Expanded" /> event
        /// when the
        /// <see cref="P:System.Windows.Controls.Expander.IsExpanded" />
        /// property changes from false to true.
        /// </summary>
        protected virtual void OnExpanded()
        {
            ToggleExpanded(Expanded, new RoutedEventArgs());
        }

        /// <summary>
        /// Raises the
        /// <see cref="E:System.Windows.Controls.Expander.Collapsed" /> event
        /// when the
        /// <see cref="P:System.Windows.Controls.Expander.IsExpanded" />
        /// property changes from true to false.
        /// </summary>
        protected virtual void OnCollapsed()
        {
            ToggleExpanded(Collapsed, new RoutedEventArgs());
        }

        /// <summary>
        /// Handle changes to the IsExpanded property.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="args">Event arguments.</param>
        private void ToggleExpanded(RoutedEventHandler handler, RoutedEventArgs args)
        {
            ToggleButton expander = ExpanderButton;
            if (!_isInToggleButtonClickHandler && expander != null) // We check "_isInToggleButtonClickHandler" otherwise the "Click" event of the ToggleButton, which also takes place, will again toggle the IsExpanded property.
            {
                expander.IsChecked = IsExpanded;
            }

            UpdateVisualState(true);
            RaiseEvent(handler, args);
        }

        /// <summary>
        /// Raise a RoutedEvent.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="args">Event arguments.</param>
        private void RaiseEvent(RoutedEventHandler handler, RoutedEventArgs args)
        {
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Handle ExpanderButton's click event.
        /// </summary>
        /// <param name="sender">The ExpanderButton in template.</param>
        /// <param name="e">Routed event arg.</param>
        private void OnExpanderButtonClicked(object sender, RoutedEventArgs e)
        {
            _isInToggleButtonClickHandler = true;
            IsExpanded = !IsExpanded;
            _isInToggleButtonClickHandler = false;
        }

        ///// <summary>
        ///// Update the visual state of the control.
        ///// </summary>
        ///// <param name="useTransitions">
        ///// A value indicating whether to automatically generate transitions to
        ///// the new state, or instantly transition to the new state.
        ///// </param>
        //void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        //{
        //    UpdateVisualState(useTransitions);
        //}

        /// <summary>
        /// Update the current visual state of the button.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            if (IsExpanded)
            {
                VisualStateManager.GoToState(this, "Expanded", false);
                //VisualStates.GoToState(this, useTransitions, VisualStates.StateExpanded);
            }
            else
            {
                VisualStateManager.GoToState(this, "Collapsed", false);
                //VisualStates.GoToState(this, useTransitions, VisualStates.StateCollapsed);
            }

        }

        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.Expander" /> control when a new
        /// template is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            ExpanderButton = GetTemplateChild(ElementExpanderButtonName) as ToggleButton;
            //Interaction.OnApplyTemplateBase();
        }


        /// <summary>
        /// Occurs when the content window of an
        /// <see cref="T:System.Windows.Controls.Expander" /> control opens to
        /// display both its header and content.
        /// </summary>
        public event RoutedEventHandler Expanded;

        /// <summary>
        /// Occurs when the content window of an
        /// <see cref="T:System.Windows.Controls.Expander" /> control closes and
        /// only the
        /// <see cref="P:System.Windows.Controls.HeaderedContentControl.Header" />
        /// is visible.
        /// </summary>
        public event RoutedEventHandler Collapsed;

        /// <summary>
        /// Gets or sets the ExpanderButton template part.
        /// </summary>
        private ToggleButton ExpanderButton
        {
            get { return _expanderButton; }
            set
            {
                // Detach from old ExpanderButton
                if (_expanderButton != null)
                {
                    _expanderButton.Click -= OnExpanderButtonClicked;
                }

                _expanderButton = value;

                if (_expanderButton != null)
                {
                    _expanderButton.IsChecked = IsExpanded;
                    _expanderButton.Click += OnExpanderButtonClicked;
                }
            }
        }
    }
}
