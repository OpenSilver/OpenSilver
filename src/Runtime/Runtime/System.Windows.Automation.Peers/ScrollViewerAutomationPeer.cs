
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

using System.Globalization;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace System.Windows.Automation.Peers
{
    /// <summary>
    /// Exposes <see cref="ScrollViewer"/> types to UI automation.
    /// </summary>
    public class ScrollViewerAutomationPeer : FrameworkElementAutomationPeer, IScrollProvider
    {   
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollViewerAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ScrollViewer"/> object that is associated with this <see cref="ScrollViewerAutomationPeer"/> 
        /// instance.
        /// </param>
        public ScrollViewerAutomationPeer(ScrollViewer owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that supports the specified pattern, based on the patterns supported
        /// by this automation peer.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern
        /// interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Scroll)
            {
                return this;
            }
            
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Gets the control type for the <see cref="ScrollViewer"/> object that
        /// is associated with this <see cref="ScrollViewerAutomationPeer"/>
        /// instance. This method is called by <see cref="AutomationPeer.GetAutomationControlType"/>.
        /// </summary>
        /// <returns>
        /// A value of the enumeration.
        /// </returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Pane;

        /// <summary>
        /// Gets the name of the class that is associated with this peer. This method is
        /// called by <see cref="AutomationPeer.GetClassName"/>.
        /// </summary>
        /// <returns>
        /// The name of the associated class.
        /// </returns>
        protected override string GetClassNameCore() => "ScrollViewer";

        /// <summary>
        /// Gets a value that indicates whether the element associated with this peer is
        /// understood by the user as interactive or as contributing to the logical structure
        /// in UI.
        /// </summary>
        /// <returns>
        /// true value to indicate that the owner control is interactive; otherwise, false.
        /// </returns>
        protected override bool IsControlElementCore()
        {
            if (((ScrollViewer)Owner).TemplatedParent == null)
            {
                return base.IsControlElementCore();
            }

            return false;
        }

        /// <summary>
        /// Request to scroll horizontally and vertically by the specified amount.
        /// The ability to call this method and simultaneously scroll horizontally
        /// and vertically provides simple panning support.
        /// </summary>
        /// <see cref="IScrollProvider.Scroll"/>
        void IScrollProvider.Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            bool scrollHorizontally = horizontalAmount != ScrollAmount.NoAmount;
            bool scrollVertically = verticalAmount != ScrollAmount.NoAmount;

            ScrollViewer owner = (ScrollViewer)Owner;

            if (scrollHorizontally && !HorizontallyScrollable || scrollVertically && !VerticallyScrollable)
            {
                throw new InvalidOperationException("Cannot perform operation.");
            }

            switch (horizontalAmount)
            {
                case ScrollAmount.LargeDecrement:
                    owner.PageLeft();
                    break;
                case ScrollAmount.SmallDecrement:
                    owner.LineLeft();
                    break;
                case ScrollAmount.SmallIncrement:
                    owner.LineRight();
                    break;
                case ScrollAmount.LargeIncrement:
                    owner.PageRight();
                    break;
                case ScrollAmount.NoAmount:
                    break;
                default:
                    throw new InvalidOperationException("Cannot perform operation.");
            }

            switch (verticalAmount)
            {
                case ScrollAmount.LargeDecrement:
                    owner.PageUp();
                    break;
                case ScrollAmount.SmallDecrement:
                    owner.LineUp();
                    break;
                case ScrollAmount.SmallIncrement:
                    owner.LineDown();
                    break;
                case ScrollAmount.LargeIncrement:
                    owner.PageDown();
                    break;
                case ScrollAmount.NoAmount:
                    break;
                default:
                    throw new InvalidOperationException("Cannot perform operation.");
            }
        }

        /// <summary>
        /// Request to set the current horizontal and Vertical scroll position by percent (0-100).
        /// Passing in the value of "-1", represented by the constant "NoScroll", will indicate that scrolling
        /// in that direction should be ignored.
        /// The ability to call this method and simultaneously scroll horizontally and vertically provides simple panning support.
        /// </summary>
        /// <see cref="IScrollProvider.SetScrollPercent"/>
        void IScrollProvider.SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            if (!IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            bool scrollHorizontally = horizontalPercent != ScrollPatternIdentifiers.NoScroll;
            bool scrollVertically = verticalPercent != ScrollPatternIdentifiers.NoScroll;

            ScrollViewer owner = (ScrollViewer)Owner;

            if (scrollHorizontally && !HorizontallyScrollable || scrollVertically && !VerticallyScrollable)
            {
                throw new InvalidOperationException("Cannot perform operation.");
            }

            if (scrollHorizontally && (horizontalPercent < 0.0) || (horizontalPercent > 100.0))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(horizontalPercent),
                    string.Format(
                        "'{0}' parameter has value '{1}', which is not in the valid range of '{2}' to '{3}'.",
                        nameof(horizontalPercent),
                        horizontalPercent.ToString(CultureInfo.InvariantCulture),
                        "0",
                        "100"));
            }
            if (scrollVertically && (verticalPercent < 0.0) || (verticalPercent > 100.0))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(verticalPercent),
                    string.Format(
                        "'{0}' parameter has value '{1}', which is not in the valid range of '{2}' to '{3}'.",
                        nameof(verticalPercent),
                        verticalPercent.ToString(CultureInfo.InvariantCulture),
                        "0",
                        "100"));
            }

            if (scrollHorizontally)
            {
                owner.ScrollToHorizontalOffset((owner.ExtentWidth - owner.ViewportWidth) * (double)horizontalPercent * 0.01);
            }
            if (scrollVertically)
            {
                owner.ScrollToVerticalOffset((owner.ExtentHeight - owner.ViewportHeight) * (double)verticalPercent * 0.01);
            }
        }

        /// <summary>
        /// Get the current horizontal scroll position
        /// </summary>
        /// <see cref="IScrollProvider.HorizontalScrollPercent"/>
        double IScrollProvider.HorizontalScrollPercent
        {
            get
            {
                if (!HorizontallyScrollable) { return ScrollPatternIdentifiers.NoScroll; }
                ScrollViewer owner = (ScrollViewer)Owner;
                return (double)(owner.HorizontalOffset * 100.0 / (owner.ExtentWidth - owner.ViewportWidth));
            }
        }

        /// <summary>
        /// Get the current vertical scroll position
        /// </summary>
        /// <see cref="IScrollProvider.VerticalScrollPercent"/>
        double IScrollProvider.VerticalScrollPercent
        {
            get
            {
                if (!VerticallyScrollable) { return ScrollPatternIdentifiers.NoScroll; }
                ScrollViewer owner = (ScrollViewer)Owner;
                return (double)(owner.VerticalOffset * 100.0 / (owner.ExtentHeight - owner.ViewportHeight));
            }
        }

        /// <summary>
        /// Equal to the horizontal percentage of the entire control that is currently viewable.
        /// </summary>
        /// <see cref="IScrollProvider.HorizontalViewSize"/>
        double IScrollProvider.HorizontalViewSize
        {
            get
            {
                ScrollViewer owner = (ScrollViewer)Owner;
                if (owner.ScrollInfo == null || owner.ExtentWidth == 0.0) { return 100.0; }
                return Math.Min(100.0, (double)(owner.ViewportWidth * 100.0 / owner.ExtentWidth));
            }
        }

        /// <summary>
        /// Equal to the vertical percentage of the entire control that is currently viewable.
        /// </summary>
        /// <see cref="IScrollProvider.VerticalViewSize"/>
        double IScrollProvider.VerticalViewSize
        {
            get
            {
                ScrollViewer owner = (ScrollViewer)Owner;
                if (owner.ScrollInfo == null || owner.ExtentHeight == 0.0) { return 100f; }
                return Math.Min(100.0, (double)(owner.ViewportHeight * 100.0 / owner.ExtentHeight));
            }
        }

        /// <summary>
        /// True if control can scroll horizontally
        /// </summary>
        /// <see cref="IScrollProvider.HorizontallyScrollable"/>
        bool IScrollProvider.HorizontallyScrollable => HorizontallyScrollable;

        /// <summary>
        /// True if control can scroll vertically
        /// </summary>
        /// <see cref="IScrollProvider.VerticallyScrollable"/>
        bool IScrollProvider.VerticallyScrollable => VerticallyScrollable;

        // Private *Scrollable properties used to determine scrollability for IScrollProvider implemenation.
        private bool HorizontallyScrollable
        {
            get
            {
                ScrollViewer owner = (ScrollViewer)Owner;
                return owner.ScrollInfo != null && owner.ExtentWidth > owner.ViewportWidth;
            }
        }

        private bool VerticallyScrollable
        {
            get
            {
                ScrollViewer owner = (ScrollViewer)Owner;
                return owner.ScrollInfo != null && owner.ExtentHeight > owner.ViewportHeight;
            }
        }
    }
}
