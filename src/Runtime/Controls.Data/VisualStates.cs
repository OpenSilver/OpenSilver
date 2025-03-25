// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

namespace System.Windows.Controls
{
    /// <summary>
    /// Names and helpers for visual states in the controls.
    /// </summary>
    internal static class VisualStates
    {
        #region GroupCommon
        /// <summary>
        /// Normal state
        /// </summary>
        public const string StateNormal = "Normal";

        /// <summary>
        /// MouseOver state
        /// </summary>
        public const string StateMouseOver = "MouseOver";

        /// <summary>
        /// Pressed state
        /// </summary>
        public const string StatePressed = "Pressed";

        /// <summary>
        /// Disabled state
        /// </summary>
        public const string StateDisabled = "Disabled";

        /// <summary>
        /// Common state group
        /// </summary>
        public const string GroupCommon = "CommonStates";
        #endregion GroupCommon

        #region GroupExpanded
        /// <summary>
        /// Expanded state
        /// </summary>
        public const string StateExpanded = "Expanded";

        /// <summary>
        /// Collapsed state
        /// </summary>
        public const string StateCollapsed = "Collapsed";

        /// <summary>
        /// Empty state
        /// </summary>
        public const string StateEmpty = "Empty";
        #endregion GroupExpanded

        #region GroupFocus
        /// <summary>
        /// Unfocused state
        /// </summary>
        public const string StateUnfocused = "Unfocused";

        /// <summary>
        /// Focused state
        /// </summary>
        public const string StateFocused = "Focused";

        /// <summary>
        /// Focus state group
        /// </summary>
        public const string GroupFocus = "FocusStates";
        #endregion GroupFocus

        #region GroupSelection
        /// <summary>
        /// Selected state
        /// </summary>
        public const string StateSelected = "Selected";

        /// <summary>
        /// Unselected state
        /// </summary>
        public const string StateUnselected = "Unselected";

        /// <summary>
        /// Selection state group
        /// </summary>
        public const string GroupSelection = "SelectionStates";


        #endregion GroupSelection

        #region GroupActive
        /// <summary>
        /// Active state
        /// </summary>
        public const string StateActive = "Active";

        /// <summary>
        /// Inactive state
        /// </summary>
        public const string StateInactive = "Inactive";

        /// <summary>
        /// Active state group
        /// </summary>
        public const string GroupActive = "ActiveStates";
        #endregion GroupActive

        #region GroupCurrent
        /// <summary>
        /// Regular state
        /// </summary>
        public const string StateRegular = "Regular";

        /// <summary>
        /// Current state
        /// </summary>
        public const string StateCurrent = "Current";

        /// <summary>
        /// Current state group
        /// </summary>
        public const string GroupCurrent = "CurrentStates";
        #endregion GroupCurrent

        #region GroupInteraction
        /// <summary>
        /// Display state
        /// </summary>
        public const string StateDisplay = "Display";

        /// <summary>
        /// Editing state
        /// </summary>
        public const string StateEditing = "Editing";

        /// <summary>
        /// Interaction state group
        /// </summary>
        public const string GroupInteraction = "InteractionStates";
        #endregion GroupInteraction

        #region GroupAlternatingRow
        /// <summary>
        /// Regular Row state
        /// </summary>
        public const string StateRegularRow = "RegularRow";

        /// <summary>
        /// Alternating Row state
        /// </summary>
        public const string StateAlternatingRow = "AlternatingRow";

        /// <summary>
        /// Alternating Row state group
        /// </summary>
        public const string GroupAlternatingRow = "AlternatingRowStates";
        #endregion GroupAlternatingRow

        #region GroupSort
        /// <summary>
        /// Unsorted state
        /// </summary>
        public const string StateUnsorted = "Unsorted";

        /// <summary>
        /// Sort Ascending state
        /// </summary>
        public const string StateSortAscending = "SortAscending";

        /// <summary>
        /// Sort Descending state
        /// </summary>
        public const string StateSortDescending = "SortDescending";

        /// <summary>
        /// Sort state group
        /// </summary>
        public const string GroupSort = "SortStates";
        #endregion GroupSort

        #region GroupValidation
        /// <summary>
        /// Invalid state
        /// </summary>
        public const string StateInvalid = "Invalid";

        /// <summary>
        /// RowInvalid state
        /// </summary>
        public const string StateRowInvalid = "RowInvalid";

        /// <summary>
        /// RowValid state
        /// </summary>
        public const string StateRowValid = "RowValid";

        /// <summary>
        /// Valid state
        /// </summary>
        public const string StateValid = "Valid";

        /// <summary>
        /// Validation state group
        /// </summary>
        public const string GroupValidation = "ValidationStates";
        #endregion GroupValidation

        public static void GoToState(Control control, bool useTransitions, string stateName)
        {
            Debug.Assert(control != null);

            VisualStateManager.GoToState(control, stateName, useTransitions);
        }

        public static void GoToState(Control control, bool useTransitions, string stateName1, string stateName2)
        {
            Debug.Assert(control != null);

            if (VisualStateManager.GoToState(control, stateName1, useTransitions))
            {
                return;
            }

            VisualStateManager.GoToState(control, stateName2, useTransitions);
        }

        public static void GoToState(Control control, bool useTransitions, string stateName1, string stateName2, string stateName3)
        {
            Debug.Assert(control != null);

            if (VisualStateManager.GoToState(control, stateName1, useTransitions))
            {
                return;
            }

            if (VisualStateManager.GoToState(control, stateName2, useTransitions))
            {
                return;
            }

            VisualStateManager.GoToState(control, stateName3, useTransitions);
        }
    }
}
