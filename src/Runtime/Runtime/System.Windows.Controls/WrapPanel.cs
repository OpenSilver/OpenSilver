

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Positions child elements in sequential position from left to right, breaking
    /// content to the next line at the edge of the containing box. Subsequent ordering
    /// happens sequentially from top to bottom or from right to left, depending
    /// on the value of the System.Windows.Controls.WrapPanel.Orientation property.
    /// </summary>
    /// <example>
    /// You can add a WrapPanel to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <WrapPanel Background="Blue">
    ///     <Border Width="80" Height="20" Background="White" Margin="5"/>
    ///     <Border Width="60" Height="20" Background="Green" Margin="5"/>
    ///     <Border Width="90" Height="20" Background="Red" Margin="5"/>
    /// </WrapPanel>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// WrapPanel myWrapPanel = new WrapPanel();
    /// myWrapPanel.Width = 100;
    /// myWrapPanel.Height = 100;
    /// myWrapPanel.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
    /// myWrapPanel.HorizontalAlignment=HorizontalAlignment.Left;
    /// //Then you can add the children element using myWrapPanel.Children.Add(...).
    /// //Do not forget to add the WrapPanel itself to the visual tree.
    /// </code>
    /// </example>
    public partial class WrapPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the System.Windows.Controls.WrapPanel class.
        /// </summary>
        public WrapPanel() : base() { }
        
        /// <summary>
        /// Gets or sets a value that specifies the dimension in which child content
        /// is arranged.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(WrapPanel),
                new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange)
                {
                    MethodToUpdateDom2 = UpdateDomOnOrientationChanged
                });

        private static void UpdateDomOnOrientationChanged(DependencyObject d, object oldValue, object newValue)
        {
            var wrapPanel = (WrapPanel)d;
            var innerDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(wrapPanel.INTERNAL_InnerDomElement);
            switch ((Orientation)newValue)
            {
                case Orientation.Horizontal:
                    innerDivStyle.width = string.Empty;
                    innerDivStyle.height = string.Empty;
                    innerDivStyle.flexDirection = string.Empty;
                    wrapPanel.SetDisplayOnChildWrappers("inline-flex");
                    break;

                case Orientation.Vertical:
                    innerDivStyle.width = "fit-content";
                    innerDivStyle.height = "calc(100%)";
                    innerDivStyle.flexDirection = "column";
                    wrapPanel.SetDisplayOnChildWrappers("block");
                    break;
            }
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var outerDiv = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
            var innerDivStyle = INTERNAL_HtmlDomManager.CreateDomElementAppendItAndGetStyle("div", outerDiv, this, out object innerDiv);
            innerDivStyle.display = "flex";
            innerDivStyle.flexWrap = "wrap";
            if (Orientation == Orientation.Vertical)
            {
                innerDivStyle.width = "fit-content";
                innerDivStyle.height = "calc(100%)";
                innerDivStyle.flexDirection = "column";
            }

            domElementWhereToPlaceChildren = innerDiv;
            return outerDiv;
        }

        public override object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild, int index = -1)
        {
            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this, index);
            var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            divStyle.display = Orientation switch
            {
                Orientation.Horizontal => "inline-flex",
                Orientation.Vertical => "block",
                _ => throw new InvalidOperationException(),
            };
            domElementWhereToPlaceChild = div;
            return div;
        }

        private void SetDisplayOnChildWrappers(string display)
        {
            foreach (UIElement child in Children)
            {
                if (child.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny is not null)
                {
                    var wrapperDivStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(
                        child.INTERNAL_InnerDivOfTheChildWrapperOfTheParentIfAny);
                    wrapperDivStyle.display = display;
                }
            }
        }
    }
}
