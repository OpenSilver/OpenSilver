

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


#if WORKINPROGRESS

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif


#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    //
    // Summary:
    //     Represents a control that applies a layout transformation to its Content.
    [TemplatePart(Name = "Presenter", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "TransformRoot", Type = typeof(Grid))]
    public sealed class LayoutTransformer : ContentControl
    {
        //
        // Summary:
        //     Identifies the LayoutTransform DependencyProperty.
        public static readonly DependencyProperty LayoutTransformProperty;

        //
        // Summary:
        //     Initializes a new instance of the LayoutTransformer class.
        public LayoutTransformer()
        {

        }

        //
        // Summary:
        //     Gets or sets the layout transform to apply on the LayoutTransformer control content.
        //
        // Remarks:
        //     Corresponds to UIElement.LayoutTransform.
        public Transform LayoutTransform { get; set; }

        //
        // Summary:
        //     Applies the layout transform on the LayoutTransformer control content.
        //
        // Remarks:
        //     Only used in advanced scenarios (like animating the LayoutTransform). Should
        //     be used to notify the LayoutTransformer control that some aspect of its Transform
        //     property has changed.
        public void ApplyLayoutTransform()
        {

        }
        //
        // Summary:
        //     Builds the visual tree for the LayoutTransformer control when a new template
        //     is applied.
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {

        }
        ////
        //// Summary:
        ////     Provides the behavior for the "Arrange" pass of layout.
        ////
        //// Parameters:
        ////   finalSize:
        ////     The final area within the parent that this element should use to arrange itself
        ////     and its children.
        ////
        //// Returns:
        ////     The actual size used.
        ////
        //// Remarks:
        ////     Using the WPF paramater name finalSize instead of Silverlight's finalSize for
        ////     clarity
        //protected override Size ArrangeOverride(Size finalSize)
        //{

        //}
        ////
        //// Summary:
        ////     Provides the behavior for the "Measure" pass of layout.
        ////
        //// Parameters:
        ////   availableSize:
        ////     The available size that this element can give to child elements.
        ////
        //// Returns:
        ////     The size that this element determines it needs during layout, based on its calculations
        ////     of child element sizes.
        //protected override Size MeasureOverride(Size availableSize)
        //{

        //}
    }
}

#endif