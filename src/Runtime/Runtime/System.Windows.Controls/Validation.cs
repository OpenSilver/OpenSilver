
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

using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Xml.Linq;

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides methods and attached properties that support data validation and govern
    /// the visual state of the control.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Identifies the Validation.Error attached event.
        /// </summary>
        public static readonly RoutedEvent ErrorEvent =
            EventManager.RegisterRoutedEvent(
                "ValidationError",
                RoutingStrategy.Bubble,
                typeof(EventHandler<ValidationErrorEventArgs>),
                typeof(Validation));

        /// <summary>
        /// Adds an event handler for the Validation.Error attached event to the specified object.
        /// </summary>
        /// <param name="element">
        /// The <see cref="UIElement"/> object to add <paramref name="handler"/> to.
        /// </param>
        /// <param name="handler">
        /// The handler to add.
        /// </param>
        public static void AddErrorHandler(UIElement element, EventHandler<ValidationErrorEventArgs> handler)
        {
            ArgumentNullException.ThrowIfNull(element);

            element.AddHandler(ErrorEvent, handler);
        }

        /// <summary>
        /// Adds an event handler for the Validation.Error attached event from the specified object.
        /// </summary>
        /// <param name="element">
        /// The <see cref="UIElement"/> object to remove <paramref name="handler"/> from.
        /// </param>
        /// <param name="handler">
        /// The handler to remove.
        /// </param>
        public static void RemoveErrorHandler(UIElement element, EventHandler<ValidationErrorEventArgs> handler)
        {
            ArgumentNullException.ThrowIfNull(element);

            element.RemoveHandler(ErrorEvent, handler);
        }

        /// <summary>
        /// Identifies the <see cref="Validation"/> Errors attached property.
        /// </summary>
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.RegisterAttached(
                "Errors",
                typeof(ReadOnlyObservableCollection<ValidationError>),
                typeof(Validation),
                new PropertyMetadata(ValidationErrorCollection.Empty));

        /// <summary>
        /// Gets the value of the <see cref="Validation"/> Errors attached
        /// property of the specified element.
        /// </summary>
        /// <param name="element">
        /// The <see cref="UIElement"/> object to read the value from.
        /// </param>
        /// <returns>
        /// A <see cref="ReadOnlyObservableCollection{ValidationError}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
        public static ReadOnlyObservableCollection<ValidationError> GetErrors(DependencyObject element)
        {
            ArgumentNullException.ThrowIfNull(element);

            return (ReadOnlyObservableCollection<ValidationError>)element.GetValue(ErrorsProperty);
        }

        /// <summary>
        ///     holds the internally modifiable collection of validation errors.
        /// </summary>
        internal static readonly DependencyProperty ValidationErrorsInternalProperty =
                DependencyProperty.RegisterAttached("ErrorsInternal",
                        typeof(ValidationErrorCollection), typeof(Validation),
                        new PropertyMetadata(
                                null,
                                new PropertyChangedCallback(OnErrorsInternalChanged)));

        // Update HasErrors and Invalidate the public ValidationErrors property whose GetOverride will return
        // the updated value of ValidationErrorsInternal, nicely wrapped into a ReadOnlyCollection<T>
        private static void OnErrorsInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationErrorCollection newErrors = e.NewValue as ValidationErrorCollection;

            if (newErrors != null)
            {
                d.SetValueInternal(ErrorsProperty, new ReadOnlyObservableCollection<ValidationError>(newErrors));
            }
            else
            {
                d.ClearValue(ErrorsProperty);
            }
        }

        internal static ValidationErrorCollection GetErrorsInternal(DependencyObject target)
        {
            return (ValidationErrorCollection)target.GetValue(ValidationErrorsInternalProperty);
        }

        /// <summary>
        /// Identifies the <see cref="Validation"/> HasError attached property.
        /// </summary>
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.RegisterAttached(
                "HasError",
                typeof(bool),
                typeof(Validation),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets the value of the <see cref="Validation"/> HasError attached
        /// property of the specified element.
        /// </summary>
        /// <param name="element">
        /// The <see cref="UIElement"/> object to read the value from.
        /// </param>
        /// <returns>
        /// The value of the <see cref="Validation"/> HasError attached property
        /// of the specified element.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// element is null.
        /// </exception>
        public static bool GetHasError(DependencyObject element)
        {
            ArgumentNullException.ThrowIfNull(element);

            return (bool)element.GetValue(HasErrorProperty);
        }

        /// <summary>
        /// Clears the ValidationError that was set through a call
        /// to MarkInvalid or a previously failed validation of that BindingExpression.
        /// </summary>
        /// <param name="bindingExpression">
        /// The object to turn valid.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// bindingExpression is null.
        /// </exception>
        public static void ClearInvalid(BindingExpression bindingExpression)
        {
            ArgumentNullException.ThrowIfNull(bindingExpression);

            bindingExpression.UpdateValidationError(null);
        }

        /// <summary>
        /// Mark this BindingExpression as invalid.  If the BindingExpression has been
        /// explicitly marked invalid in this way, then it will remain
        /// invalid until ClearInvalid is called or another transfer to the source validates successfully.
        /// </summary>
        /// <param name="bindingExpression">
        /// The <see cref="BindingExpression"/> object to mark as invalid.
        /// </param>
        /// <param name="validationError">
        /// The <see cref="ValidationError"/> object to use.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// bindingExpression is null or validationError is null.
        /// </exception>
        public static void MarkInvalid(BindingExpression bindingExpression, ValidationError validationError)
        {
            ArgumentNullException.ThrowIfNull(bindingExpression);
            ArgumentNullException.ThrowIfNull(validationError);

            bindingExpression.UpdateValidationError(validationError);
        }

        // add a validation error to the given element
        internal static void AddValidationError(ValidationError validationError, DependencyObject targetElement, bool shouldRaiseEvent)
        {
            if (targetElement == null)
                return;

            bool wasValid;
            ValidationErrorCollection validationErrors = GetErrorsInternal(targetElement);

            if (validationErrors == null)
            {
                wasValid = true;
                validationErrors = new ValidationErrorCollection();
                validationErrors.Add(validationError);
                targetElement.SetValueInternal(ValidationErrorsInternalProperty, validationErrors);
            }
            else
            {
                wasValid = validationErrors.Count == 0;
                validationErrors.Add(validationError);
            }

            if (wasValid)
            {
                targetElement.SetValueInternal(HasErrorProperty, true);
            }

            if (shouldRaiseEvent)
            {
                OnValidationError(targetElement, validationError, ValidationErrorEventAction.Added);
            }

            if (wasValid)
            {
                ShowValidationError(targetElement, true);
            }
        }

        // remove a validation error from the given element
        internal static void RemoveValidationError(ValidationError validationError, DependencyObject targetElement, bool shouldRaiseEvent)
        {
            if (targetElement == null)
                return;

            ValidationErrorCollection validationErrors = GetErrorsInternal(targetElement);
            if (validationErrors == null || validationErrors.Count == 0 || !validationErrors.Contains(validationError))
                return;

            bool isValid = validationErrors.Count == 1;   // about to remove the last error

            if (isValid)
            {
                // instead of removing the last error, just discard the error collection.
                // This sends out only one property-change event, instead of two.
                // Any bindings to Errors[x] will appreciate the economy.
                targetElement.ClearValue(HasErrorProperty);

                targetElement.ClearValue(ValidationErrorsInternalProperty);

                if (shouldRaiseEvent)
                {
                    OnValidationError(targetElement, validationError, ValidationErrorEventAction.Removed);
                }

                ShowValidationError(targetElement, false);
            }
            else
            {
                // if it's not the last error, just remove it.
                validationErrors.Remove(validationError);

                if (shouldRaiseEvent)
                {
                    OnValidationError(targetElement, validationError, ValidationErrorEventAction.Removed);
                }
            }
        }

        private static void OnValidationError(DependencyObject source, ValidationError validationError, ValidationErrorEventAction action)
        {
            if (source is UIElement uie)
            {
                uie.RaiseEvent(new ValidationErrorEventArgs(validationError, action));
            }
        }

        private static void ShowValidationError(DependencyObject targetElement, bool show)
        {
            if (targetElement is not Control c)
            {
                return;
            }

            if (show)
            {
                c.ShowValidationError();
            }
            else
            {
                c.HideValidationError();
            }
        }
    }

    /// <summary>
    ///      ValidationErrorCollection contains the list of ValidationErrors from
    ///      the various Bindings on an Element.  ValidationErrorCollection
    ///      be set through the Validation.ErrorsProperty.
    /// </summary>
    internal sealed class ValidationErrorCollection : ObservableCollection<ValidationError>
    {

        /// <summary>
        /// Empty collection that serves as a default value for
        /// Validation.ErrorsProperty.
        /// </summary>
        public static readonly ReadOnlyObservableCollection<ValidationError> Empty =
                new ReadOnlyObservableCollection<ValidationError>(new ValidationErrorCollection());
    }
}
