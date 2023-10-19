
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

using System.Collections.Generic;
using System.Windows.Browser;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a button control that displays a hyperlink.
    /// </summary>
    /// <example>
    /// <code lang="XAML" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    /// <StackPanel x:Name="MyStackPanel">
    ///     <HyperlinkButton Content="here" NavigateUri="http://www.myaddress.com" Foreground="Blue"/>
    /// </StackPanel>
    /// </code>
    /// <code lang="C#">
    /// HyperlinkButton hyperlinkButton = new HyperlinkButton() { Content = "here", NavigateUri = new Uri("http://www.myaddress.com"), Foreground = new SolidColorBrush(Windows.UI.Colors.Blue) };
    /// MyStackPanel.Children.Add(hyperlinkButton);
    /// </code>
    /// </example>
    public class HyperlinkButton : ButtonBase
    {
        private static readonly HashSet<string> ExternalTargets = new HashSet<string>
        {
            "_blank",
            "_media",
            "_parent",
            "_search",
            "_self",
            "_top"
        };

        /// <summary>
        /// Initializes a new instance of the HyperlinkButton class.
        /// </summary>
        public HyperlinkButton()
        {
            DefaultStyleKey = typeof(HyperlinkButton);
        }

        protected override void OnClick()
        {
            base.OnClick();

            if (NavigateUri != null)
            {
                Navigate();
            }
        }

        /// <summary>
        /// Returns a <see cref="HyperlinkButtonAutomationPeer"/> for use by the Silverlight automation 
        /// infrastructure.
        /// </summary>
        /// <returns>
        /// A <see cref="HyperlinkButtonAutomationPeer"/> for the hyperlink button object.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            => new HyperlinkButtonAutomationPeer(this);

        /// <summary>
        /// Navigate to the Uri. 
        /// </summary>
        private void Navigate()
        {
            string target = TargetName;
            Uri navigateUri = NavigateUri;

            if (!ExternalTargets.Contains(target))
            {
                if (TryInternalNavigate())
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(target))
            {
                target = "_self";
            }

            HtmlPage.Window.Navigate(navigateUri, target);
        }

        private bool TryInternalNavigate()
        {
            DependencyObject d = this;
            DependencyObject subtree = this;
            do
            {
                d = VisualTreeHelper.GetParent(d) ?? (d as FrameworkElement)?.Parent;

                if (d != null && (d is INavigate || VisualTreeHelper.GetParent(d) == null))
                {
                    INavigate navigator = FindNavigator(d as FrameworkElement, subtree);
                    if (navigator != null)
                    {
                        return navigator.Navigate(NavigateUri);
                    }
                    subtree = d;
                }
            }
            while (d != null);

            return false;
        }

        private INavigate FindNavigator(FrameworkElement fe, DependencyObject subtree)
        {
            if (fe == null)
            {
                return null;
            }
            
            if (fe is INavigate && (fe.Name == TargetName || string.IsNullOrEmpty(TargetName)))
            {
                return (INavigate)fe;
            }

            bool isPopup = fe is Popup;
            int count = (isPopup ? 1 : VisualTreeHelper.GetChildrenCount(fe));
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = (isPopup ? ((Popup)fe).Child : VisualTreeHelper.GetChild(fe, i));
                if (child == subtree)
                {
                    continue;
                }

                INavigate navigate = FindNavigator(child as FrameworkElement, subtree);
                if (navigate != null)
                {
                    return navigate;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) to navigate to when the
        /// HyperlinkButton is clicked.
        /// </summary>
        public Uri NavigateUri
        {
            get { return (Uri)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }
        /// <summary>
        /// Identifies the NavigateUri dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.Register("NavigateUri", typeof(Uri), typeof(HyperlinkButton), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the name of the target window or frame that the Web page should
        /// open in, or the name of the object within the application to navigate to.
        /// </summary>
        public string TargetName
        {
            get { return (string)GetValue(TargetNameProperty); }
            set { SetValue(TargetNameProperty, value); }
        }
        /// <summary>
        /// Identifies the TargetName dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof(string), typeof(HyperlinkButton), new PropertyMetadata(""));


    }
}