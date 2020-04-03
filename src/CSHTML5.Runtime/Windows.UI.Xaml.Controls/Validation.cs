

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{     
/// <summary>
/// Provides methods and attached properties that support data validation.
/// </summary>
    public static class Validation
    {   
        // Exceptions:
        //   System.ArgumentNullException:
        //     If element is null.
        /// <summary>
        /// Gets the value of the System.Windows.Controls.Validation.Errors attached
        /// property of the specified element.
        /// </summary>
        /// <param name="element">
        /// The System.Windows.UIElement or System.Windows.ContentElement object to read
        /// the value from.
        /// </param>
        /// <returns>
        /// A System.Collections.ObjectModel.ReadOnlyObservableCollection`1 of System.Windows.Controls.ValidationError
        /// objects.
        /// </returns>
        public static ObservableCollection<ValidationError> GetErrors(DependencyObject element) //Note: it returned a ReadOnlyObservableCollection but the point seems limited so we will go back to it only if necessary.
        {
            ObservableCollection<ValidationError> returnValue =  (ObservableCollection<ValidationError>)element.GetValue(ErrorsProperty);
            if (returnValue == null)
            {
                returnValue = new ObservableCollection<ValidationError>();
                element.SetValue(ErrorsProperty, returnValue);
            }
            return returnValue;
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.Validation.Errors attached property.
        /// </summary>
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.RegisterAttached("Errors", typeof(ObservableCollection<ValidationError>), typeof(Validation), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        // Exceptions:
        //   System.ArgumentNullException:
        //     If element is null.
        /// <summary>
        /// Gets the value of the System.Windows.Controls.Validation.HasError attached
        /// property of the specified element.
        /// </summary>
        /// <param name="obj">
        /// The System.Windows.UIElement or System.Windows.ContentElement object to read
        /// the value from.
        /// </param>
        /// <returns>
        /// The value of the System.Windows.Controls.Validation.HasError attached property
        /// of the specified element.
        /// </returns>
        public static bool GetHasError(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasErrorProperty);
        }

        private static void SetHasError(DependencyObject obj, object value)
        {
            obj.SetValue(HasErrorProperty, value);
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.Validation.HasError attached property.
        /// </summary>
        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.RegisterAttached("HasError", typeof(bool), typeof(Validation), new PropertyMetadata(false)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        // Exceptions:
        //   System.ArgumentNullException:
        //     If bindingExpression is null.
        /// <summary>
        /// Removes all System.Windows.Controls.ValidationError objects from the specified
        /// System.Windows.Data.BindingExpressionBase object.
        /// </summary>
        /// <param name="bindingExpression">The object to turn valid.</param>
        public static void ClearInvalid(BindingExpression bindingExpression) //Note: bindingExpression was of type BindingExpressionBase but we merged the BindingExpressionBase and the BindingExpression types.
        {
            if (bindingExpression == null)
            {
                throw new ArgumentNullException("bindingExpression");
            }
            if(bindingExpression.Target is UIElement)
            {
                UIElement targetAsUIElement = (UIElement)bindingExpression.Target;
                if (targetAsUIElement.INTERNAL_ValidationErrorsDictionary != null && targetAsUIElement.INTERNAL_ValidationErrorsDictionary.ContainsKey(bindingExpression))
                {
                    ValidationError error = targetAsUIElement.INTERNAL_ValidationErrorsDictionary[bindingExpression];
                    //remove the error from the Errors attached property
                    ObservableCollection<ValidationError> errors = GetErrors(targetAsUIElement);
                    errors.Remove(error);
                    targetAsUIElement.INTERNAL_ValidationErrorsDictionary.Remove(bindingExpression);

                    if (errors.Count == 0)
                    {
                        //We removed the last error on the Target:
                        SetHasError(targetAsUIElement, false);
                    }

                    //Raise the event saying that we removed a ValidationError
                    if (bindingExpression.ParentBinding.NotifyOnValidationError)
                    {
                        DependencyObject element = targetAsUIElement;
                        while (element is FrameworkElement)
                        {
                            ((FrameworkElement)element).INTERNAL_RaiseBindingValidationErrorEvent(new ValidationErrorEventArgs()
                            {
                                Action = ValidationErrorEventAction.Removed,
                                Error = error,
                                OriginalSource = targetAsUIElement
                            });

                            element = ((FrameworkElement)element).Parent;
                        }
                    }

                    RefreshPopup(targetAsUIElement, errors);
                }
            }
        }

        // Exceptions:
        //   System.ArgumentNullException:
        //     If bindingExpression is null.
        //
        //   System.ArgumentNullException:
        //     If validationError is null.
        /// <summary>
        /// Marks the specified System.Windows.Data.BindingExpression object as invalid
        /// with the specified System.Windows.Controls.ValidationError object.
        /// </summary>
        /// <param name="bindingExpression">The System.Windows.Data.BindingExpression object to mark as invalid.</param>
        /// <param name="validationError">The System.Windows.Controls.ValidationError object to use.</param>
        public static void MarkInvalid(BindingExpression bindingExpression, ValidationError validationError) //Note: bindingExpression was of type BindingExpressionBase but we merged the BindingExpressionBase and the BindingExpression types.
        {
            if (bindingExpression == null)
            {
                throw new ArgumentNullException("bindingExpression");
            }
            if (validationError == null)
            {
                throw new ArgumentNullException("validationError");
            }
            DependencyObject target = bindingExpression.Target;
            if (target is UIElement)
            {
                UIElement targetAsUIElement = (UIElement)target;

                ClearInvalid(bindingExpression); //We remove any previous error because I don't see how we could have multiple ones on a single Binding + it wouldn't fit in the Dictionary as it is.

                ObservableCollection<ValidationError> errors = Validation.GetErrors(targetAsUIElement);
                bool setHasError = errors.Count == 0;

                if (targetAsUIElement.INTERNAL_ValidationErrorsDictionary == null)
                {
                    targetAsUIElement.INTERNAL_ValidationErrorsDictionary = new Dictionary<BindingExpression, ValidationError>();
                }
                //Remember the ValidationError and the BindingExpression it came from:
                targetAsUIElement.INTERNAL_ValidationErrorsDictionary.Add(bindingExpression, validationError);
                //add the error to the Errors attached property:
                errors.Add(validationError);


                if (setHasError)
                {
                    SetHasError(targetAsUIElement, true);
                }

                //Raise the event saying that we added a ValidationError
                if (bindingExpression.ParentBinding.NotifyOnValidationError)
                {
                    DependencyObject element = target;
                    while (element is FrameworkElement)
                    {
                        ((FrameworkElement)element).INTERNAL_RaiseBindingValidationErrorEvent(
                            new ValidationErrorEventArgs()
                            {
                                Action = ValidationErrorEventAction.Added,
                                Error = validationError,
                                OriginalSource = targetAsUIElement
                            });
                        element = ((FrameworkElement)element).Parent;
                    }
                }
                

                RefreshPopup(targetAsUIElement, errors);
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
                            Margin = new Thickness(5,0,0,0),
                        };
                        TextBlock textBlock = new TextBlock()
                        {
                            Foreground = new SolidColorBrush(Colors.White),
                            FontSize = 11.0,
                            Margin = new Thickness(5,3,5,3),
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

        static void Popup_PopupMoved(object sender, EventArgs e)
        {
            Popup popup = (Popup)sender;
            PopupRoot popupRoot = popup.PopupRoot;

            // Hide the popup if the parent element is not visible (for example, if the user scrolls and the TextBox becomes hidden under another control, cf. ZenDesk #628):
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
}
