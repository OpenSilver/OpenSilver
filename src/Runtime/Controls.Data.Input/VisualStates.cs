// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls.Internal
{
    internal static class VisualStates
    {
#region GroupValidation

        /// <summary>
        /// VSM Group for validation, containing focus details.  These details are not orthogonal and thus were combined.   See TextBox validation for precedence.
        /// </summary>
        public const string GroupValidation = "ValidationStates";

        /// <summary>
        /// VSM State for Valid
        /// </summary>
        public const string StateValid = "Valid";

        /// <summary>
        /// VSM STate for Invalid
        /// </summary>
        public const string StateInvalid = "Invalid";

        /// <summary>
        /// VSM State for Valid and Focused (DescriptionViewer specific)
        /// </summary>
        public const string StateValidFocused = "ValidFocused";

        /// <summary>
        /// VSM State for Valid and Unfocused (DescriptionViewer specific)
        /// </summary>
        public const string StateValidUnfocused = "ValidUnfocused";

        /// <summary>
        /// VSM State for Invalid and Focused (DescriptionViewer specific)
        /// </summary>
        public const string StateInvalidFocused = "InvalidFocused";

        /// <summary>
        /// VSM State for Invalid and Focused (DescriptionViewer specific)
        /// </summary>
        public const string StateInvalidUnfocused = "InvalidUnfocused";

        /// <summary>
        /// VSM State for no errors (ValidationSummary specific)
        /// </summary>
        public const string StateEmpty = "Empty";

        /// <summary>
        /// VSM State for containing errors (ValidationSummary specific)
        /// </summary>
        public const string StateHasErrors = "HasErrors";

#endregion GroupValidation

#region GroupCommon

        /// <summary>
        /// VSM Group for common states, such as Normal or Disabled
        /// </summary>
        public const string GroupCommon = "CommonStates";

        /// <summary>
        /// VSM state for Normal (enabled)
        /// </summary>
        public const string StateNormal = "Normal";

        /// <summary>
        /// VSM state for Disabled
        /// </summary>
        public const string StateDisabled = "Disabled";

#endregion GroupCommon

#region GroupRequired

        /// <summary>
        /// VSM group for required states
        /// </summary>
        public const string GroupRequired = "RequiredStates";

        /// <summary>
        /// VSM state for not required
        /// </summary>
        public const string StateNotRequired = "NotRequired";

        /// <summary>
        /// VSM state for required
        /// </summary>
        public const string StateRequired = "Required";

#endregion GroupRequired

#region GroupDescription

        /// <summary>
        /// VSM group for description states
        /// </summary>
        public const string GroupDescription = "DescriptionStates";

        /// <summary>
        /// VSM state for no description defined
        /// </summary>
        public const string StateNoDescription = "NoDescription";

        /// <summary>
        /// VSM state for having a description defined
        /// </summary>
        public const string StateHasDescription = "HasDescription";

#endregion GroupDescription
    }
}
