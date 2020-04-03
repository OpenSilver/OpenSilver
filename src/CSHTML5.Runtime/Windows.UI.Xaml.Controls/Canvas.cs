

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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines an area within which you can explicitly position child objects, using
    /// coordinates that are relative to the Canvas area.
    /// </summary>
    /// <example>
    /// You can add a Canvas to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <Canvas Width="100"
    ///         Height="100"
    ///         Background="Blue"
    ///         HorizontalAlignment="Left">
    ///         <!-- Children here. -->
    /// </Canvas>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// Canvas myCanvas = new Canvas();
    /// myCanvas.Width = 100;
    /// myCanvas.Height = 100;
    /// myCanvas.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
    /// myCanvas.HorizontalAlignment=HorizontalAlignment.Left;
    /// </code>
    /// </example>
    public partial class Canvas : Panel
    {
        /// <summary>
        /// Identifies the Canvas.Left XAML attached property.
        /// </summary>
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.RegisterAttached("Left", typeof(double), typeof(UIElement), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    var uielement = instance as UIElement;
                    if (uielement != null && uielement.INTERNAL_VisualParent is Canvas)
                    {
                        return new CSSEquivalent()
                        {
                            Value = (inst, value) =>
                                {
                                    return value.ToString() + "px";
                                },
                            Name = new List<string> { "left" },
                            DomElement = uielement.INTERNAL_AdditionalOutsideDivForMargins,
                            ApplyAlsoWhenThereIsAControlTemplate = true
                        };
                    }
                    else
                    {
                        return null;
                    }
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            }
            );

        /// <summary>
        /// Identifies the Canvas.Top XAML attached property.
        /// </summary>
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.RegisterAttached("Top", typeof(double), typeof(UIElement), new PropertyMetadata(0d)
            {
                GetCSSEquivalent = (instance) =>
                {
                    var uielement = instance as UIElement;
                    if (uielement != null && uielement.INTERNAL_VisualParent is Canvas)
                    {
                        return new CSSEquivalent()
                        {
                            Value = (inst, value) =>
                            {
                                return value.ToString() + "px";
                            },
                            Name = new List<string> { "top" },
                            DomElement = uielement.INTERNAL_AdditionalOutsideDivForMargins,
                            ApplyAlsoWhenThereIsAControlTemplate = true
                        };
                    }
                    else
                    {
                        return null;
                    }
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            }
            );

        /// <summary>
        /// Identifies the Canvas.ZIndex XAML attached property.
        /// </summary>
        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.RegisterAttached("ZIndex", typeof(int), typeof(UIElement), new PropertyMetadata(0)
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Value = (inst, value) =>
                        {
                            return value.ToString();
                        },
                        Name = new List<string> { "zIndex" },
                        DomElement = ((UIElement)instance).INTERNAL_AdditionalOutsideDivForMargins,
                        ApplyAlsoWhenThereIsAControlTemplate = true
                    };
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            }
            );

        /// <summary>
        /// Sets the value of the Canvas.Left XAML attached property for a target element.
        /// </summary>
        /// <param name="element">The object to which the property value is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetLeft(UIElement element, double value)
        {
            element.SetValue(LeftProperty, value);
        }

        /// <summary>
        /// Gets the value of the Canvas.Left XAML attached property for the target element.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The Canvas.Left XAML attached property value of the specified object.</returns>
        public static double GetLeft(UIElement element)
        {
            return (double)element.GetValue(LeftProperty);
        }


        /// <summary>
        /// Sets the value of the Canvas.Top XAML attached property for a target element.
        /// </summary>
        /// <param name="element">The object to which the property value is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetTop(UIElement element, double value)
        {
            element.SetValue(TopProperty, value);
        }
        /// <summary>
        /// Gets the value of the Canvas.Top XAML attached property for the target element.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The Canvas.Top XAML attached property value of the specified object.</returns>
        public static double GetTop(UIElement element)
        {
            return (double)element.GetValue(TopProperty);
        }

        /// <summary>
        /// Sets the value of the Canvas.ZIndex XAML attached property for a target element.
        /// </summary>
        /// <param name="element">The object to which the property value is written.</param>
        /// <param name="value">The value to set.</param>
        public static void SetZIndex(UIElement element, int value)
        {
            element.SetValue(ZIndexProperty, value);
        }
        /// <summary>
        /// Gets the value of the Canvas.ZIndex XAML attached property for the target element.
        /// </summary>
        /// <param name="element">The object from which the property value is read.</param>
        /// <returns>The Canvas.ZIndex XAML attached property value of the specified object.</returns>
        public static int GetZIndex(UIElement element)
        {
            return (int)element.GetValue(ZIndexProperty);
        }


        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            var div = INTERNAL_HtmlDomManager.CreateDomElementAndAppendIt("div", parentRef, this);
            var style = INTERNAL_HtmlDomManager.GetDomElementStyleForModification(div);
            style.overflow = "display";
            style.position = "relative";

            domElementWhereToPlaceChildren = div;
            return div;

            //domElementWhereToPlaceChildren = div;

            //var div1 = INTERNAL_HtmlDomManager.CreateDomElement("div");
            //var div2 = INTERNAL_HtmlDomManager.CreateDomElement("div");
            //div2.style.position = "absolute";
            //INTERNAL_HtmlDomManager.AppendChild(div1, div2);
            //domElementWhereToPlaceChildren = div2;

            //return div;

            /* -------------------------------
             * A canvas should look like this:
             * -------------------------------
             * <div style="width: 50px; height: 50px;">
             *     <div style="position:relative"> width & height are the size of the canvas itself
             *         ... children (with position: absolute), below are two example of children
             *         <div style="background-color: rgb(200,0,200);width:20px;height:20px;  position: absolute"></div>
             *         <div style="background-color: rgb(100,0,200);width:20px;height:20px;margin-left:20px;margin-right:auto;  position: absolute"></div>
             *     </div>
             * </div>
            */

        }
    }
}
