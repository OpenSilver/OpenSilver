﻿
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


        /*
        // Summary:
        //     Gets or sets a value that specifies the height of all items that are contained
        //     within a System.Windows.Controls.WrapPanel.
        //
        // Returns:
        //     The System.Double that represents the uniform height of all items that are
        //     contained within the System.Windows.Controls.WrapPanel. The default value
        //     is System.Double.NaN.
        public double ItemHeight { get; set; }
        // Summary:
        //     Identifies the System.Windows.Controls.WrapPanel.ItemHeight  dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.WrapPanel.ItemHeight  dependency
        //     property.
        public static readonly DependencyProperty ItemHeightProperty;

        //
        // Summary:
        //     Gets or sets a value that specifies the width of all items that are contained
        //     within a System.Windows.Controls.WrapPanel.
        //
        // Returns:
        //     A System.Double that represents the uniform width of all items that are contained
        //     within the System.Windows.Controls.WrapPanel. The default value is System.Double.NaN.
        public double ItemWidth { get; set; }
        //
        // Summary:
        //     Identifies the System.Windows.Controls.WrapPanel.ItemWidth  dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.Controls.WrapPanel.ItemWidth  dependency
        //     property.
        public static readonly DependencyProperty ItemWidthProperty;
        */
       
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
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapPanel), new PropertyMetadata(Orientation.Horizontal, Orientation_Changed));
        static void Orientation_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var wrapPanel = (WrapPanel)d;
            Orientation newValue = (Orientation)e.NewValue;
            if (INTERNAL_VisualTreeManager.IsElementInVisualTree(wrapPanel))
                if (newValue == Orientation.Vertical)
                {
                    throw new NotSupportedException("Vertical orientation is not yet supported for the WrapPanel");
                }
                else
                {
                    //todo: change all the wrappers of the children to set their style.display to inline-block
                    INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(wrapPanel).textAlign = "left";
                }
        }



        /*
        // Summary:
        //     Arranges the content of a System.Windows.Controls.WrapPanel element.
        //
        // Parameters:
        //   finalSize:
        //     The System.Windows.Size that this element should use to arrange its child
        //     elements.
        //
        // Returns:
        //     The System.Windows.Size that represents the arranged size of this System.Windows.Controls.WrapPanel
        //     element and its children.
        protected override Size ArrangeOverride(Size finalSize);
        //
        // Summary:
        //     Measures the child elements of a System.Windows.Controls.WrapPanel in anticipation
        //     of arranging them during the System.Windows.Controls.WrapPanel.ArrangeOverride(System.Windows.Size)
        //     pass.
        //
        // Parameters:
        //   constraint:
        //     An upper limit System.Windows.Size that should not be exceeded.
        //
        // Returns:
        //     The System.Windows.Size that represents the desired size of the element.
        protected override Size MeasureOverride(Size constraint);
         * */


        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
            var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            //divStyle.overflow = "auto";
            divStyle.lineHeight = "0px";
            divStyle.whiteSpace = "normal";
            domElementWhereToPlaceChildren = div;
            return div;
        }

        public override object CreateDomChildWrapper(object parentRef, out object domElementWhereToPlaceChild)
        {
            if (Orientation == Controls.Orientation.Horizontal)
            {
                var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
                var divStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
                divStyle.display = "inline-block";
                divStyle.lineHeight = "initial";
                domElementWhereToPlaceChild = div;
                return div;
            }
            else
            {
                domElementWhereToPlaceChild = null;
                return null;
            }
        }
        //internal override dynamic ShowChild(UIElement child)
        //{
        //    dynamic elementToReturn = base.ShowChild(child); //we need to return this so that a class that inherits from this but doesn't create a wrapper (or a different one) is correctly handled 

        //    dynamic domChildWrapper = INTERNAL_VisualChildrenInformation[child].INTERNAL_OptionalChildWrapper_OuterDomElement;
        //    var domChildWrapperStyle = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(domChildWrapper);
        //    domChildWrapperStyle.display = "block"; //todo: verify that it is not necessary to revert to the previous value instead.
        //    //domChildWrapperStyle.visibility = "visible";
        //    domChildWrapperStyle.width = "";
        //    domChildWrapperStyle.height = "";

        //    return elementToReturn;
        //}
    }

}
