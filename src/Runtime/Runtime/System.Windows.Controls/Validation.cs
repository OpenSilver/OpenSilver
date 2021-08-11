

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


using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides methods and attached properties that support data validation and govern
    /// the visual state of the control.
    /// </summary>
    public static class Validation
    {
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
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (ReadOnlyObservableCollection<ValidationError>)element.GetValue(ErrorsProperty);
        }

        /// <summary>
        ///     holds the internally modifiable collection of validation errors.
        /// </summary>
        internal static readonly DependencyProperty ValidationErrorsInternalProperty =
                DependencyProperty.RegisterAttached("ErrorsInternal",
                        typeof(ValidationErrorCollection), typeof(Validation),
                        new PropertyMetadata(
                                (ValidationErrorCollection)null,
                                new PropertyChangedCallback(OnErrorsInternalChanged)));

        // Update HasErrors and Invalidate the public ValidationErrors property whose GetOverride will return
        // the updated value of ValidationErrorsInternal, nicely wrapped into a ReadOnlyCollection<T>
        private static void OnErrorsInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValidationErrorCollection newErrors = e.NewValue as ValidationErrorCollection;

            if (newErrors != null)
            {
                d.SetValue(ErrorsProperty, new ReadOnlyObservableCollection<ValidationError>(newErrors));
            }
            else
            {
                d.ClearValue(ErrorsProperty);
            }
        }

        internal static ValidationErrorCollection GetErrorsInternal(DependencyObject target)
        {
            return (ValidationErrorCollection)target.GetValue(Validation.ValidationErrorsInternalProperty);
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
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool)element.GetValue(HasErrorProperty);
        }

        /// <summary>
        /// Clears the ValidationError that was set through a call
        /// to MarkInvalid or a previously failed validation of that BindingExpression.
        /// </summary>
        /// <param name="bindingExpression">The object to turn valid.</param>
        /// <exception cref="ArgumentNullException">
        /// bindingExpression is null.
        /// </exception>
        public static void ClearInvalid(BindingExpression bindingExpression)
        {
            if (bindingExpression == null)
            {
                throw new ArgumentNullException("bindingExpression");
            }

            UIElement target = bindingExpression.Target as UIElement;
            if (target != null)
            {
                if (target.INTERNAL_ValidationErrorsDictionary != null &&
                    target.INTERNAL_ValidationErrorsDictionary.ContainsKey(bindingExpression))
                {
                    ValidationError error = target.INTERNAL_ValidationErrorsDictionary[bindingExpression];
                    //remove the error from the Errors attached property
                    ValidationErrorCollection errors = GetErrorsInternal(target);
                    errors.Remove(error);
                    target.INTERNAL_ValidationErrorsDictionary.Remove(bindingExpression);

                    if (errors.Count == 0)
                    {
                        //We removed the last error on the Target:
                        target.SetValue(Validation.HasErrorProperty, false);
                    }

                    //Raise the event saying that we removed a ValidationError
                    if (bindingExpression.ParentBinding.NotifyOnValidationError)
                    {
                        for (FrameworkElement elt = target as FrameworkElement; elt != null; elt = (VisualTreeHelper.GetParent(elt) as FrameworkElement))
                        {
                            elt.INTERNAL_RaiseBindingValidationErrorEvent(
                                new ValidationErrorEventArgs()
                                {
                                    Action = ValidationErrorEventAction.Removed,
                                    Error = error,
                                    OriginalSource = target
                                });
                        }
                    }

                    RefreshPopup(target, errors);
                }
            }
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
            if (bindingExpression == null)
            {
                throw new ArgumentNullException("bindingExpression");
            }
            if (validationError == null)
            {
                throw new ArgumentNullException("validationError");
            }

            UIElement target = bindingExpression.Target as UIElement;
            if (target != null)
            {
                // We remove any previous error because I don't see how we could have 
                // multiple ones on a single Binding + it wouldn't fit in the Dictionary 
                // as it is.
                ClearInvalid(bindingExpression);

                if (target.INTERNAL_ValidationErrorsDictionary == null)
                {
                    target.INTERNAL_ValidationErrorsDictionary = new Dictionary<BindingExpression, ValidationError>();
                }
                // Remember the ValidationError and the BindingExpression it came from
                target.INTERNAL_ValidationErrorsDictionary.Add(bindingExpression, validationError);

                bool wasValid;
                ValidationErrorCollection validationErrors = GetErrorsInternal(target);

                if (validationErrors == null)
                {
                    wasValid = true;
                    validationErrors = new ValidationErrorCollection();
                    validationErrors.Add(validationError);
                    target.SetValue(Validation.ValidationErrorsInternalProperty, validationErrors);
                }
                else
                {
                    wasValid = validationErrors.Count == 0;
                    validationErrors.Add(validationError);
                }

                if (wasValid)
                {
                    target.SetValue(Validation.HasErrorProperty, true);
                }

                //Raise the event saying that we added a ValidationError
                if (bindingExpression.ParentBinding.NotifyOnValidationError)
                {
                    for (FrameworkElement elt = target as FrameworkElement; elt != null; elt = (VisualTreeHelper.GetParent(elt) as FrameworkElement))
                    {
                        elt.INTERNAL_RaiseBindingValidationErrorEvent(
                            new ValidationErrorEventArgs()
                            {
                                Action = ValidationErrorEventAction.Added,
                                Error = validationError,
                                OriginalSource = target
                            });
                    }
                }

                RefreshPopup(target, validationErrors);
            }

        }

        private static void RefreshPopup(UIElement target, ObservableCollection<ValidationError> errors)
        {
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(target))
            {
                Popup popup = target.INTERNAL_ValidationErrorPopup;
                if (errors.Count == 0)
                {
                    if (popup != null)
                    {
                        popup.IsOpen = false;
                    }
                }
                else
                {
                    if (popup == null)
                    {
                        popup = new Popup();
                        popup.INTERNAL_PopupMoved += Popup_PopupMoved;
                        target.INTERNAL_ValidationErrorPopup = popup;

                        //we define the content of the popup:
                        Border border = new Border()
                        {
                            Background = new SolidColorBrush(Color.FromArgb(255, 219, 2, 12)),
                            CornerRadius = new CornerRadius(2),
                            Margin = new Thickness(5, 0, 0, 0),
                        };
                        TextBlock textBlock = new TextBlock()
                        {
                            Foreground = new SolidColorBrush(Colors.White),
                            FontSize = 11.0,
                            Margin = new Thickness(5, 3, 5, 3),
                            TextWrapping = TextWrapping.Wrap,
                            MaxWidth = 250,
                        };
                        border.Child = textBlock;
                        popup.Child = border;
                        popup.IsHitTestVisible = false;
                        popup.PlacementTarget = target;
                        popup.Placement = PlacementMode.Right;
                    }

                    //Point PopupAbsolutePosition = INTERNAL_PopupsManager.CalculatePopupAbsolutePositionBasedOnElementPosition(target, 0d, 0d);
                    //popup.HorizontalOffset = PopupAbsolutePosition.X;
                    //popup.VerticalOffset = PopupAbsolutePosition.Y;
                    string errorsInString = "";
                    bool isFirst = true;
                    foreach (ValidationError validationError in errors)
                    {
                        errorsInString += Environment.NewLine;
                        if (isFirst)
                        {
                            errorsInString = "";
                            isFirst = false;
                        }
                        errorsInString += validationError.ErrorContent;
                    }

                    //Note: if the popup is not new, its Child (currently a textBlock) is already set.
                    //Todo: when we will support Templates for the validation error messages, make sure we correctly refresh the template visually.
                    ((TextBlock)((Border)popup.Child).Child).Text = errorsInString;
                    //Note: the line above will need to change when we will support templates.
                    //      At this time, we will probably use a Binding and set the Text as the DataContext of the Popup
                    //      OR we might need to set the whole errors collection as the DataContext depending on how it works in WPF and Silverlight.
                    popup.IsOpen = true;
                }

            }

        }

        private static void Popup_PopupMoved(object sender, EventArgs e)
        {
            Popup popup = (Popup)sender;
            PopupRoot popupRoot = popup.PopupRoot;

            // Hide the popup if the parent element is not visible (for example, if the 
            // user scrolls and the TextBox becomes hidden under another control, cf. ZenDesk #628):
            if (popup.PlacementTarget is FrameworkElement && popupRoot != null)
            {
                bool isParentVisible = INTERNAL_PopupsManager.IsPopupParentVisibleOnScreen(popup);

                popupRoot.Visibility = (isParentVisible ? Visibility.Visible : Visibility.Collapsed);

            }
        }




        #region to be implemented

        ///// <summary>
        ///// Identifies the System.Windows.Controls.Validation.Error attached event.
        ///// </summary>
        //public static readonly RoutedEventHandler ErrorEvent; //originally of type RoutedEvent (which doesn't exist here)

        ///// <summary>
        ///// Adds an event handler for the System.Windows.Controls.Validation.Error attached
        ///// event to the specified object.
        ///// </summary>
        ///// <param name="element">
        ///// The System.Windows.UIElement or System.Windows.ContentElement object to add
        ///// handler to.
        ///// </param>
        ///// <param name="handler">The handler to add.</param>
        //public static void AddErrorHandler(DependencyObject element, EventHandler<ValidationErrorEventArgs> handler)
        //{
        //    //assumption on the way we can make it work?
        //    ValidationErrorEventHandler errorEventHandler = (ValidationErrorEventHandler)element.Getvalue(ErrorEventProperty);
        //    ValidationErrorEventHandler += handler;
        //}

        ///// <summary>
        ///// Adds an event handler for the System.Windows.Controls.Validation.Error attached
        ///// event from the specified object.
        ///// </summary>
        ///// <param name="element">
        ///// The System.Windows.UIElement or System.Windows.ContentElement object to remove
        ///// handler from.
        ///// </param>
        ///// <param name="handler">The handler to remove.</param>
        //public static void RemoveErrorHandler(DependencyObject element, EventHandler<ValidationErrorEventArgs> handler)
        //{
        //    //assumption on the way we can make it work?
        //    ValidationErrorEventHandler errorEventHandler = (ValidationErrorEventHandler)element.Getvalue(ErrorEventProperty);
        //    ValidationErrorEventHandler -= handler;
        //}

        ////[AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     If element is null.
        ///// <summary>
        ///// Gets the value of the System.Windows.Controls.Validation.ErrorTemplate attached
        ///// property of the specified element.
        ///// </summary>
        ///// <param name="element">
        ///// The System.Windows.UIElement or System.Windows.ContentElement object to read
        ///// the value from.
        ///// </param>
        ///// <returns>
        ///// The System.Windows.Controls.ControlTemplate used to generate validation error
        ///// feedback on the adorner layer.
        ///// </returns>
        //public static ControlTemplate GetErrorTemplate(DependencyObject element)
        //{
        //    return (ControlTemplate)element.GetValue(ErrorTemplateProperty);
        //}

        //// Exceptions:
        ////   System.ArgumentNullException:
        ////     If element is null.
        ///// <summary>
        ///// Sets the value of the System.Windows.Controls.Validation.ErrorTemplate attached
        ///// property to the specified element.
        ///// </summary>
        ///// <param name="element">
        ///// The System.Windows.UIElement or System.Windows.ContentElement object to set
        ///// value on.
        ///// </param>
        ///// <param name="value">
        ///// The System.Windows.Controls.ControlTemplate to use to generate validation
        ///// error feedback on the adorner layer.
        ///// </param>
        //public static void SetErrorTemplate(DependencyObject element, ControlTemplate value)
        //{
        //    element.SetValue(ErrorTemplateProperty, value);
        //}

        ///// <summary>
        ///// Identifies the System.Windows.Controls.Validation.ErrorTemplate attached
        ///// property.
        ///// </summary>
        //public static readonly DependencyProperty ErrorTemplateProperty =
        //    DependencyProperty.Register("ErrorTemplate", typeof(ControlTemplate), typeof(UIElement), new PropertyMetadata(null, ErrorTemplate_Changed));


        #endregion


        #region Not implemented stuff

        ///// <summary>
        ///// Identifies the System.Windows.Controls.Validation.ValidationAdornerSiteFor attached
        ///// property.
        ///// </summary>
        //public static readonly DependencyProperty ValidationAdornerSiteForProperty;

        ///// <summary>
        ///// Gets the value of the System.Windows.Controls.Validation.ValidationAdornerSiteFor
        ///// attached property for the specified element.
        ///// </summary>
        ///// <param name="element">The element from which to get the System.Windows.Controls.Validation.ValidationAdornerSiteFor.</param>
        ///// <returns>The value of the System.Windows.Controls.Validation.ValidationAdornerSiteFor.</returns>
        //[AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        //public static DependencyObject GetValidationAdornerSiteFor(DependencyObject element);

        ///// <summary>
        ///// Sets the System.Windows.Controls.Validation.ValidationAdornerSiteFor attached
        ///// property to the specified value on the specified element.
        ///// </summary>
        ///// <param name="element">The element on which to set the System.Windows.Controls.Validation.ValidationAdornerSiteFor.</param>
        ///// <param name="value">
        ///// The System.Windows.Controls.Validation.ValidationAdornerSiteFor of the specified
        ///// element.
        ///// </param>
        //public static void SetValidationAdornerSiteFor(DependencyObject element, DependencyObject value);


        ///// <summary>
        ///// Identifies the System.Windows.Controls.Validation.ValidationAdornerSite attached
        ///// property.
        ///// </summary>
        //public static readonly DependencyProperty ValidationAdornerSiteProperty;

        ///// <summary>
        ///// Gets the value of the System.Windows.Controls.Validation.ValidationAdornerSite
        ///// attached property for the specified element.
        ///// </summary>
        ///// <param name="element">The element from which to get the System.Windows.Controls.Validation.ValidationAdornerSite.</param>
        ///// <returns>The value of the System.Windows.Controls.Validation.ValidationAdornerSite.</returns>
        //[AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        //public static DependencyObject GetValidationAdornerSite(DependencyObject element);

        ///// <summary>
        ///// Sets the System.Windows.Controls.Validation.ValidationAdornerSite attached
        ///// property to the specified value on the specified element.
        ///// </summary>
        ///// <param name="element">The element on which to set the System.Windows.Controls.Validation.ValidationAdornerSite.</param>
        ///// <param name="value">
        ///// The System.Windows.Controls.Validation.ValidationAdornerSite of the specified
        ///// element.
        ///// </param>
        //public static void SetValidationAdornerSite(DependencyObject element, DependencyObject value);



        #endregion
    }

    /// <summary>
    ///      ValidationErrorCollection contains the list of ValidationErrors from
    ///      the various Bindings on an Element.  ValidationErrorCollection
    ///      be set through the Validation.ErrorsProperty.
    /// </summary>
    internal class ValidationErrorCollection : ObservableCollection<ValidationError>
    {

        /// <summary>
        /// Empty collection that serves as a default value for
        /// Validation.ErrorsProperty.
        /// </summary>
        public static readonly ReadOnlyObservableCollection<ValidationError> Empty =
                new ReadOnlyObservableCollection<ValidationError>(new ValidationErrorCollection());
    }
}
