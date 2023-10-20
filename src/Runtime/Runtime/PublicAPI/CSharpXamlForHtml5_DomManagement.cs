
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
using OpenSilver.Internal;
using System.Windows;

public static partial class CSharpXamlForHtml5
{
    /// <summary>
    /// This class allows the management of the Dom tree without using VerbatimExpressions
    /// </summary>
    public static partial class DomManagement
    {
        /// <summary>
        /// Checks if the UIElement is in the visual tree.
        /// </summary>
        /// <param name="control">The UIElement</param>
        /// <returns>True if the UIElement is currently in the visual tree, false otherwise.</returns>
        public static bool IsControlInVisualTree(UIElement control)
        {
            return INTERNAL_VisualTreeManager.IsElementInVisualTree(control);
        }

        //see if the comment that tells that it is the outermost element is comprehensible enough.
        /// <summary>
        /// Returns the DOM element corresponding to the UIElement.
        /// </summary>
        /// <param name="control">The UIElement</param>
        /// <returns>The DOM element corresponding to the UIElement. It is the DOM Element that contains all of the UIElement's DOM elements.</returns>
        [Obsolete]
        public static dynamic GetDomElementFromControl(UIElement control)
        {
            if (control.INTERNAL_OuterDomElement == null)
                throw new InvalidOperationException("Cannot get the DOM element because the control is not (yet?) in the visual tree. Consider waiting until the Loaded event before calling this piece of code. You can also call the 'IsControlInVisualTree' method to check if the control is in the visual tree.");

            if (CSharpXamlForHtml5.Environment.IsRunningInJavaScript)
                return control.INTERNAL_OuterDomElement;
            else
                return new CSharpXamlForHtml5.DomManagement.Types.DynamicDomElement((INTERNAL_HtmlDomElementReference)control.INTERNAL_OuterDomElement);
        }

        /// <summary>
        /// Sets the Html representation of the UIElement.
        /// </summary>
        /// <param name="control">The UIElement</param>
        /// <param name="htmlRepresentation">The string that defines the html representation of the UIElement.</param>
        [Obsolete(Helper.ObsoleteMemberMessage + " Use HtmlPresenter instead.")]
        public static void SetHtmlRepresentation(UIElement control, string htmlRepresentation)
        {
            control.INTERNAL_HtmlRepresentation = htmlRepresentation;
        }
    }
}
