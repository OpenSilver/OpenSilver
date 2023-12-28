// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Media;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="ChildWindow" /> types to UI automation.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class ChildWindowAutomationPeer : FrameworkElementAutomationPeer, IWindowProvider, ITransformProvider
    {
        /// <summary>
        /// Specifies whether the ChildWindow is the top most element.
        /// </summary>
        private bool _isTopMost;

        /// <summary>
        /// Gets or sets a value indicating whether the ChildWindow is the top most element.
        /// </summary>
        private bool IsTopMostPrivate
        {
            get
            {
                return this._isTopMost;
            }
            set
            {
                if (this._isTopMost != value)
                {
                    this._isTopMost = value;
                    this.RaisePropertyChangedEvent(WindowPatternIdentifiers.IsTopmostProperty, !this._isTopMost, this._isTopMost);
                }
            }
        }

        /// <summary>
        /// Gets the owning ChildWindow.
        /// </summary>
        private ChildWindow OwningChildWindow
        {
            get
            {
                return (ChildWindow)Owner;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildWindowAutomationPeer" /> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ChildWindow" /> to associate with this <see cref="ChildWindowAutomationPeer" />.
        /// </param>
        public ChildWindowAutomationPeer(ChildWindow owner)
            : base(owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }
            this.RefreshIsTopMostProperty();
        }

        /// <summary>
        /// Gets the control pattern for this <see cref="ChildWindowAutomationPeer" />.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the
        /// specified pattern interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Transform || patternInterface == PatternInterface.Window)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the <see cref="AutomationControlType" /> for the element associated with
        /// this <see cref="ChildWindowAutomationPeer" />.
        /// Called by <see cref="AutomationPeer.GetAutomationControlType" />.
        /// </summary>
        /// <returns>A value of the enumeration.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }

        /// <summary>
        /// Gets the name of the class for the object associated with this
        /// <see cref="ChildWindowAutomationPeer" />.
        /// Called by <see cref="AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>
        /// A string value that represents the type of the child window.
        /// </returns>
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <summary>
        /// Gets the text label of the <see cref="ChildWindow" /> that is associated with 
        /// this <see cref="ChildWindowAutomationPeer" />.
        /// Called by <see cref="AutomationPeer.GetName" />.
        /// </summary>
        /// <returns>
        /// The text label of the element that is associated with this
        /// automation peer.
        /// </returns>
        protected override string GetNameCore()
        {
            string name = base.GetNameCore();
            if (string.IsNullOrEmpty(name))
            {
                AutomationPeer labeledBy = GetLabeledByCore();
                if (labeledBy != null)
                {
                    name = labeledBy.GetName();
                }

                if (string.IsNullOrEmpty(name) && this.OwningChildWindow.Title != null)
                {
                    name = this.OwningChildWindow.Title.ToString();
                }
            }
            return name;
        }

        /// <summary>
        /// Gets the interaction state of the window.
        /// </summary>
        /// <value>
        /// The interaction state of the control, as a value of the enumeration.
        /// </value>
        WindowInteractionState IWindowProvider.InteractionState
        {
            get
            {
                return this.OwningChildWindow.InteractionState;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window is modal.
        /// </summary>
        /// <value>
        /// True in all cases.
        /// </value>
        bool IWindowProvider.IsModal
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the window is the topmost
        /// element in the z-order of layout.
        /// </summary>
        /// <value>
        /// True if the window is topmost; otherwise, false.
        /// </value>
        bool IWindowProvider.IsTopmost
        {
            get
            {
                return this.IsTopMostPrivate;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window can be maximized.
        /// </summary>
        /// <value>False in all cases.</value>
        bool IWindowProvider.Maximizable
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the window can be minimized.
        /// </summary>
        /// <value>False in all cases.</value>
        bool IWindowProvider.Minimizable
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the visual state of the window.
        /// </summary>
        /// <value>
        /// <see cref="WindowVisualState.Normal" /> in all cases.
        /// </value>
        WindowVisualState IWindowProvider.VisualState
        {
            get
            {
                return WindowVisualState.Normal;
            }
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        void IWindowProvider.Close()
        {
            this.OwningChildWindow.Close();
        }

        /// <summary>
        /// Changes the visual state of the window (such as minimizing or
        /// maximizing it).
        /// </summary>
        /// <param name="state">
        /// The visual state of the window to change to, as a value of the
        /// enumeration.
        /// </param>
        void IWindowProvider.SetVisualState(WindowVisualState state)
        {
        }

        /// <summary>
        /// Blocks the calling code for the specified time or until the
        /// associated process enters an idle state, whichever completes first.
        /// </summary>
        /// <param name="milliseconds">
        /// The amount of time, in milliseconds, to wait for the associated
        /// process to become idle.
        /// </param>
        /// <returns>
        /// True if the window has entered the idle state; false if the timeout
        /// occurred.
        /// </returns>
        bool IWindowProvider.WaitForInputIdle(int milliseconds)
        {
            return false;
        }

        /// <summary>
        /// Moves the control.
        /// </summary>
        /// <param name="x">
        /// The absolute screen coordinates of the left side of the control.
        /// </param>
        /// <param name="y">
        /// The absolute screen coordinates of the top of the control.
        /// </param>
        void ITransformProvider.Move(double x, double y)
        {
            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }

            if (x > this.OwningChildWindow.Width)
            {
                x = this.OwningChildWindow.Width;
            }

            if (y > this.OwningChildWindow.Height)
            {
                y = this.OwningChildWindow.Height;
            }

            FrameworkElement contentRoot = this.OwningChildWindow.ContentRoot;

            if (contentRoot != null)
            {
                GeneralTransform gt = contentRoot.TransformToVisual(null);

                if (gt != null)
                {
                    Point p = gt.Transform(new Point(0, 0));

                    TransformGroup transformGroup = contentRoot.RenderTransform as TransformGroup;

                    if (transformGroup == null)
                    {
                        transformGroup = new TransformGroup();
                        transformGroup.Children.Add(contentRoot.RenderTransform);
                    }

                    TranslateTransform t = new TranslateTransform();
                    t.X = x - p.X;
                    t.Y = y - p.Y;

                    if (transformGroup != null)
                    {
                        transformGroup.Children.Add(t);
                        contentRoot.RenderTransform = transformGroup;
                    }
                }
            }
        }

        /// <summary>
        /// Resizes the control.
        /// </summary>
        /// <param name="width">The new width of the window, in pixels.</param>
        /// <param name="height">
        /// The new height of the window, in pixels.
        /// </param>
        void ITransformProvider.Resize(double width, double height)
        {
        }

        /// <summary>
        /// Rotates the control.
        /// </summary>
        /// <param name="degrees">
        /// The number of degrees to rotate the control.  A positive number
        /// rotates the control clockwise.  A negative number rotates the
        /// control counterclockwise.
        /// </param>
        void ITransformProvider.Rotate(double degrees)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the element can be moved.
        /// </summary>
        /// <value>True in all cases.</value>
        bool ITransformProvider.CanMove
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the element can be resized.
        /// </summary>
        /// <value>False in all cases.</value>
        bool ITransformProvider.CanResize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the element can be rotated.
        /// </summary>
        /// <value>False in all cases.</value>
        bool ITransformProvider.CanRotate
        {
            get { return false; }
        }

        /// <summary>
        /// Returns if the ChildWindow is the top most element.
        /// </summary>
        /// <returns>Bool value.</returns>
        private bool GetIsTopMostCore()
        {
            return !(this.OwningChildWindow.InteractionState == WindowInteractionState.BlockedByModalWindow);
        }

        /// <summary>
        /// Raises PropertyChangedEvent for WindowInteractionStateProperty.
        /// </summary>
        /// <param name="oldValue">Old WindowInteractionStateProperty.</param>
        /// <param name="newValue">New WindowInteractionStateProperty.</param>
        internal void RaiseInteractionStatePropertyChangedEvent(WindowInteractionState oldValue, WindowInteractionState newValue)
        {
            this.RaisePropertyChangedEvent(WindowPatternIdentifiers.WindowInteractionStateProperty, oldValue, newValue);
            this.RefreshIsTopMostProperty();
        }

        /// <summary>
        /// Updates the IsTopMostPrivate property.
        /// </summary>
        private void RefreshIsTopMostProperty()
        {
            this.IsTopMostPrivate = this.GetIsTopMostCore();
        }
    }
}
