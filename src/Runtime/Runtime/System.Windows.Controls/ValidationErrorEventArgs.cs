
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides data for the <see cref="FrameworkElement.BindingValidationError"/> event.
    /// </summary>
    public class ValidationErrorEventArgs : RoutedEventArgs
    {
        public ValidationErrorEventArgs() { }

        internal ValidationErrorEventArgs(ValidationError error, ValidationErrorEventAction action)
        {
            Error = error;
            Action = action;
        }

        /// <summary>
        /// Gets the state of the validation error.
        /// </summary>
        /// <returns>
        /// One of the enumeration values that indicates the state of the validation error.
        /// </returns>
        public ValidationErrorEventAction Action { get; }


        /// <summary>
        /// Gets the validation error that caused the <see cref="FrameworkElement.BindingValidationError"/>
        /// event.
        /// </summary>
        /// <returns>
        /// A <see cref="ValidationError"/> object that contains the exception that caused the validation error.
        /// </returns>
        public ValidationError Error { get; }

        /// <summary>
        /// Gets or sets the value that marks the routed event as handled.
        /// </summary>
        /// <returns>
        /// true if the event is handled; otherwise, false.
        /// </returns>
        public bool Handled { get; set; }
    }
}