// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Controls", Justification = "These namespaces are being used for WPF compat.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Automation.Peers", Justification = "Automation peers are shipped in this namespace.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Scope = "type", Target = "System.Windows.Navigation.NavigatedEventHandler", Justification = "This EventHandler type is being used for WPF compat.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Scope = "type", Target = "System.Windows.Navigation.NavigatingCancelEventHandler", Justification = "This EventHandler type is being used for WPF compat.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Scope = "type", Target = "System.Windows.Navigation.NavigationFailedEventHandler", Justification = "This EventHandler type is being used for WPF compat.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Scope = "type", Target = "System.Windows.Navigation.NavigationStoppedEventHandler", Justification = "This EventHandler type is being used for WPF compat.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Scope = "type", Target = "System.Windows.Navigation.FragmentNavigationEventHandler", Justification = "This EventHandler type is being used for WPF compat.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "URIs", Scope = "resource", Target = "System.Windows.Navigation.Resource.resources", Justification = "This is a common acronym used throughout the web.")]
[assembly: SuppressMessage("General", "SWC1001:XmlDocumentationCommentShouldBeSpelledCorrectly", MessageId = "URIs", Justification = "This is a common acronym used throughout the web.")]
