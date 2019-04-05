
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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a validation error that is created either by the binding engine
    /// when a System.Windows.Controls.ValidationRule reports a validation error,
    /// or through the System.Windows.Controls.Validation.MarkInvalid(System.Windows.Data.BindingExpressionBase,System.Windows.Controls.ValidationError)
    /// method explicitly.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.ValidationError
        /// class with the specified parameters.
        /// </summary>
        /// <param name="bindingInError">
        /// The System.Windows.Data.BindingExpression or System.Windows.Data.MultiBindingExpression
        /// object with the validation error.
        /// </param>
        public ValidationError(object bindingInError) //Note: we added this constructor to allow the users to create their own ValidationErrors (all the original constructors used ValidationRule which is not implemented yet).
        {
            BindingInError = bindingInError;
        }

        /// <summary>
        /// Gets the System.Windows.Data.BindingExpression or System.Windows.Data.MultiBindingExpression
        /// object of this System.Windows.Controls.ValidationError. The object is either
        /// marked invalid explicitly or has a failed validation rule.
        /// </summary>
        public object BindingInError { get; internal set; }
       
        /// <summary>
        /// Gets or sets an object that provides additional context for this System.Windows.Controls.ValidationError,
        /// such as a string describing the error.
        /// </summary>
        public object ErrorContent { get; set; }
        
        /// <summary>
        /// Gets or sets the System.Exception object that was the cause of this System.Windows.Controls.ValidationError,
        /// if the error is the result of an exception.
        /// </summary>
        public Exception Exception { get; set; }



        #region to implement (needs to handle the ValidationRule class and its subclasses)

        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Controls.ValidationError
        ///// class with the specified parameters.
        ///// </summary>
        ///// <param name="ruleInError">The rule that detected validation error.</param>
        ///// <param name="bindingInError">
        ///// The System.Windows.Data.BindingExpression or System.Windows.Data.MultiBindingExpression
        ///// object with the validation error.
        ///// </param>
        //public ValidationError(ValidationRule ruleInError, object bindingInError);

        ///// <summary>
        ///// Initializes a new instance of the System.Windows.Controls.ValidationError
        ///// class with the specified parameters.
        ///// </summary>
        ///// <param name="ruleInError">The rule that detected validation error.</param>
        ///// <param name="bindingInError">
        ///// The System.Windows.Data.BindingExpression or System.Windows.Data.MultiBindingExpression
        ///// object with the validation error.
        ///// </param>
        ///// <param name="errorContent">Information about the error.</param>
        ///// <param name="exception">
        ///// The exception that caused the validation failure. This parameter is optional
        ///// and can be set to null.
        ///// </param>
        //public ValidationError(ValidationRule ruleInError, object bindingInError, object errorContent, Exception exception);

        ///// <summary>
        ///// Gets or sets the System.Windows.Controls.ValidationRule object that was the
        ///// cause of this System.Windows.Controls.ValidationError, if the error is the
        ///// result of a validation rule.
        ///// </summary>
        //public ValidationRule RuleInError { get; set; }

        #endregion

    }
}
