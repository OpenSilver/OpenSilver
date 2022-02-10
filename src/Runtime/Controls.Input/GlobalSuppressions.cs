// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "namescope")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Unwatermarked")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Silverlight")]

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Automation.Peers")]

[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowGotFocus(System.Windows.RoutedEventArgs)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowKeyDown(System.Windows.Input.KeyEventArgs)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowKeyUp(System.Windows.Input.KeyEventArgs)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowLostFocus(System.Windows.RoutedEventArgs)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowMouseEnter(System.Windows.Input.MouseEventArgs)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowMouseLeave(System.Windows.Input.MouseEventArgs)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#ClickCount")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#LastClickPosition")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#LastClickTime")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnGotFocusBase()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnLostFocusBase()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnMouseEnterBase()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnMouseLeaveBase()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnMouseLeftButtonDownBase()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnMouseLeftButtonUpBase()")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.VisualStates.#GetImplementationRoot(System.Windows.DependencyObject)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.VisualStates.#TryGetVisualStateGroup(System.Windows.DependencyObject,System.String)")]

// AutoCompleteBox does not exist on WPF
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.SelectorSelectionAdapter")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.AutoCompleteBox")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.AutoCompleteFilterMode")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.AutoCompleteFilterPredicate`1")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Automation.Peers.AutoCompleteBoxAutomationPeer")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.ISelectionAdapter")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.PopulatedEventArgs")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.PopulatedEventHandler")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.PopulatingEventArgs")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.PopulatingEventHandler")]

// These methods are used in their entirety in the System.Windows.Controls library
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#GetTopAndBottom(System.Windows.FrameworkElement,System.Windows.FrameworkElement,System.Double&,System.Double&)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#LineDown(System.Windows.Controls.ScrollViewer)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#LineLeft(System.Windows.Controls.ScrollViewer)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#LineRight(System.Windows.Controls.ScrollViewer)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#LineUp(System.Windows.Controls.ScrollViewer)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#PageDown(System.Windows.Controls.ScrollViewer)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#PageLeft(System.Windows.Controls.ScrollViewer)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#PageRight(System.Windows.Controls.ScrollViewer)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#PageUp(System.Windows.Controls.ScrollViewer)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#ScrollByHorizontalOffset(System.Windows.Controls.ScrollViewer,System.Double)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#ScrollByVerticalOffset(System.Windows.Controls.ScrollViewer,System.Double)")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.ScrollExtensions.#ScrollToBottom(System.Windows.Controls.ScrollViewer)")]