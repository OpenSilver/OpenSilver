namespace $ext_safeprojectname$.Controls
{
    /// <summary>
    /// Names and helpers for visual states in the controls.
    /// </summary>
    internal static class VisualStates
    {
        #region GroupBusyStatus

        /// <summary>
        /// Busy state for BusyIndicator.
        /// </summary>
        public const string StateBusy = "Busy";

        /// <summary>
        /// Idle state for BusyIndicator.
        /// </summary>
        public const string StateIdle = "Idle";

        /// <summary>
        /// Busy states group name.
        /// </summary>
        public const string GroupBusyStatus = "BusyStatusStates";

        #endregion GroupBusyStatus

        #region GroupVisibility

        /// <summary>
        /// Visible state name for BusyIndicator.
        /// </summary>
        public const string StateVisible = "Visible";

        /// <summary>
        /// Hidden state name for BusyIndicator.
        /// </summary>
        public const string StateHidden = "Hidden";

        /// <summary>
        /// BusyDisplay group.
        /// </summary>
        public const string GroupVisibility = "VisibilityStates";

        #endregion GroupVisibility
    }
}