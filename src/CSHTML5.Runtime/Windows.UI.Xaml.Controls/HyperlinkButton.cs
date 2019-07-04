
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



//TODOBRIDGE: usefull using?
#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Browser;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
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
        /// <summary>
        /// Initializes a new instance of the HyperlinkButton class.
        /// </summary>
        public HyperlinkButton()
        {
            // Set default style:
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(HyperlinkButton);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultHyperlinkButtonStyle.GetDefaultStyle());
#endif

            Click += HyperlinkButton_Click;
        }

        void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigateUri != null)
            {
                string targetName = this.TargetName;
                object targetElement = null;

                // Look for an element within the application (in the Visual Tree) that has the specified TargetName and implements INavigate:
                if (!string.IsNullOrEmpty(targetName)
                    && (targetElement = this.FindName(targetName)) is INavigate) //todo: verify that "FindName" is enough to find the element (it walks up the visual tree and looks in the elements that implement INameScope) or if we should manually traverse all the nodes of the Visual Tree.
                {
                    //-----------------
                    // Navigation within the application
                    //-----------------

                    ((INavigate)targetElement).Navigate(NavigateUri);
                }
                else
                {
                    //-----------------
                    // External navigation (browser navigation)
                    //-----------------

                    if (string.IsNullOrEmpty(targetName))
                    {
#if MIGRATION
                        targetName = "_self";
#else
                        targetName = "_blank";
#endif
                    }
                    HtmlPage.Window.Navigate(NavigateUri, targetName);
                }
            }
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