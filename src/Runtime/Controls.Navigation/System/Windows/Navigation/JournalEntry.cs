//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Browser;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    /// <summary>
    /// A journal history entry.
    /// </summary>
    /// <seealso cref="Journal"/>
    internal sealed class JournalEntry : DependencyObject
    {
#region Fields

        private Uri _source;

#endregion Fields

#region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">The journal entry name.</param>
        /// <param name="uri">The journal entry URI value.</param>
        public JournalEntry(string name, Uri uri)
        {
            Guard.ArgumentNotNull(uri, "uri");

            this.Name = name;
            this._source = uri;
        }

#endregion Constructors

#region Name Attached Property

        /// <summary>
        /// An attached dependency property used to specify a name for a journal entry, which may be reflected in the browser window
        /// if the journal is integrated with the browser.
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached(
                "Name",
                typeof(string),
                typeof(JournalEntry),
                new PropertyMetadata(new PropertyChangedCallback(NamePropertyChanged)));

        /// <summary>
        /// Gets or sets the journal entry name.
        /// </summary>
        public string Name
        {
            get { return (string)this.GetValue(NameProperty); }
            set { this.SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Gets the value of the <see cref="NameProperty"/> attached property on the <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object to inspect for a <see cref="NameProperty"/></param>
        /// <returns>The value of the <see cref="NameProperty"/> attached property.</returns>
        public static string GetName(DependencyObject obj)
        {
            Guard.ArgumentNotNull(obj, "obj");
            return (string)obj.GetValue(NameProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="NameProperty"/> attached property on the <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object to set the <see cref="NameProperty"/> on.</param>
        /// <param name="name">The name to set</param>
        public static void SetName(DependencyObject obj, string name)
        {
            Guard.ArgumentNotNull(obj, "obj");
            obj.SetValue(NameProperty, name);
        }

        private static void NamePropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            // If the JournalEntry.Name property was changed on a Page, and that Page
            // has a browser-integrated Journal, and we have access to the HTML bridge,
            // then update the title in the browser.
            Page page = depObj as Page;
            if (page != null &&
                page.NavigationService != null &&
                page.NavigationService.Journal != null &&
                page.NavigationService.Journal.UseNavigationState &&
                HtmlPage.IsEnabled)
            {
                HtmlPage.Document.SetProperty("title", e.NewValue);
            }
        }

#endregion

#region NavigationContext Attached Property

        /// <summary>
        /// An attached dependency property used to specify a <see cref="NavigationContext"/> for a piece of content which
        /// has been navigated to.
        /// </summary>
        public static readonly DependencyProperty NavigationContextProperty = DependencyProperty.RegisterAttached("NavigationContext", typeof(NavigationContext), typeof(JournalEntry), null);

        /// <summary>
        /// Gets or sets the <see cref="NavigationContext"/> for the journal entry
        /// </summary>
        public NavigationContext NavigationContext
        {
            get { return (NavigationContext)this.GetValue(NavigationContextProperty); }
            set { this.SetValue(NavigationContextProperty, value); }
        }

        /// <summary>
        /// Gets the value of the <see cref="NavigationContextProperty"/> attached property on the <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object to inspect for a <see cref="NavigationContextProperty"/></param>
        /// <returns>The value of the <see cref="NavigationContextProperty"/> attached property.</returns>
        public static NavigationContext GetNavigationContext(DependencyObject obj)
        {
            Guard.ArgumentNotNull(obj, "obj");
            return (NavigationContext)obj.GetValue(NavigationContextProperty);
        }

        /// <summary>
        /// Sets the value of the <see cref="NavigationContextProperty"/> attached property on the <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object to set the <see cref="NavigationContextProperty"/> on.</param>
        /// <param name="navigationContext">The navigation context to set</param>
        public static void SetNavigationContext(DependencyObject obj, NavigationContext navigationContext)
        {
            Guard.ArgumentNotNull(obj, "obj");
            obj.SetValue(NavigationContextProperty, navigationContext);
        }

#endregion

#region Properties

        /// <summary>
        /// Gets or sets the Uri that for this journal entry
        /// </summary>
        public Uri Source
        {
            get
            {
                return this._source;
            }

            set
            {
                Guard.ArgumentNotNull(value, "value");
                this._source = value;
            }
        }

#endregion Properties
    }
}
