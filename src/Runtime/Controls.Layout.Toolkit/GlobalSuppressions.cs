// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Automation.Peers", Justification = "This is the official namespace for automation peers.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows")]

[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Headered")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Util")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "namescope")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Multi")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Unwatermarked")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Validator's")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "WPF's")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "Silverlight")]

[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "Multi", Scope = "resource", Target = "System.Windows.Controls.Properties.Resources.resources", Justification = "Follows WPF MultiSelector class naming.")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.Accordion", Justification = "New control for Silverlight")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.AccordionButton", Justification = "Part of Accordion")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.AccordionItem", Justification = "Part of Accordion")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.ExpandableContentControl", Justification = "Part of Accordion")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.LayoutTransformControl")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.SelectionSequence", Justification = "Part of Accordion")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Controls.SelectionMode")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Automation.Peers.AccordionAutomationPeer")]
[assembly: SuppressMessage("Compatibility", "SWC4000:GeneralWPFCompatibilityRule", MessageId = "System.Windows.Automation.Peers.AccordionItemAutomationPeer")]

[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowGotFocus(System.Windows.RoutedEventArgs)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowKeyDown(System.Windows.Input.KeyEventArgs)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowKeyUp(System.Windows.Input.KeyEventArgs)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowLostFocus(System.Windows.RoutedEventArgs)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowMouseEnter(System.Windows.Input.MouseEventArgs)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowMouseLeave(System.Windows.Input.MouseEventArgs)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#AllowMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#ClickCount", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#LastClickPosition", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#LastClickTime", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnGotFocusBase()", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnLostFocusBase()", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnMouseEnterBase()", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnMouseLeaveBase()", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnMouseLeftButtonDownBase()", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.InteractionHelper.#OnMouseLeftButtonUpBase()", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.VisualStates.#GetImplementationRoot(System.Windows.DependencyObject)", Justification = "Implementations used by other assemblies.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "System.Windows.Controls.VisualStates.#TryGetVisualStateGroup(System.Windows.DependencyObject,System.String)", Justification = "Implementations used by other assemblies.")]

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Controls.Primitives", Justification = "Controls are considered primitives.")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "headered", Justification = "Follows naming used in hierarchy.")]
