// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// Represents the header for an accordion item.
    /// </summary>
    /// <remarks>By creating a seperate control, there is more flexibility in 
    /// the templating possibilities.</remarks>
    /// <QualityBand>Preview</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = VisualStates.StateExpanded, GroupName = VisualStates.GroupExpansion)]
    [TemplateVisualState(Name = VisualStates.StateCollapsed, GroupName = VisualStates.GroupExpansion)]

    [TemplateVisualState(Name = VisualStates.StateExpandDown, GroupName = VisualStates.GroupExpandDirection)]
    [TemplateVisualState(Name = VisualStates.StateExpandUp, GroupName = VisualStates.GroupExpandDirection)]
    [TemplateVisualState(Name = VisualStates.StateExpandLeft, GroupName = VisualStates.GroupExpandDirection)]
    [TemplateVisualState(Name = VisualStates.StateExpandRight, GroupName = VisualStates.GroupExpandDirection)]
    public class AccordionButton : ToggleButton
    {
        #region Parent AccordionItem
        /// <summary>
        /// Gets or sets a reference to the parent AccordionItem 
        /// of an AccordionButton.
        /// </summary>
        /// <value>The parent accordion item.</value>
        internal AccordionItem ParentAccordionItem { get; set; }
        #endregion Parent AccordionItem

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionButton"/> 
        /// class.
        /// </summary>
        public AccordionButton()
        {
            DefaultStyleKey = typeof(AccordionButton);
        }

        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
        /// <param name="useTransitions">If set to <c>true</c> use transitions.</param>
        /// <remarks>The header will follow the parent accordionitem states.</remarks>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            // the visualstate of the header is completely dependent on the parent state.
            if (ParentAccordionItem == null)
            {
                return;
            }

            if (ParentAccordionItem.IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateExpanded);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateCollapsed);
            }

            switch (ParentAccordionItem.ExpandDirection)
            {
                // no animations on an expanddirection change.
                case ExpandDirection.Down:
                    VisualStates.GoToState(this, false, VisualStates.StateExpandDown);
                    break;

                case ExpandDirection.Up:
                    VisualStates.GoToState(this, false, VisualStates.StateExpandUp);
                    break;

                case ExpandDirection.Left:
                    VisualStates.GoToState(this, false, VisualStates.StateExpandLeft);
                    break;

                default:
                    VisualStates.GoToState(this, false, VisualStates.StateExpandRight);
                    break;
            }
        }
    }
}
