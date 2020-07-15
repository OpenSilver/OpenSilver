using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.VisualStudioExtension.Editor.XamlDesigner
{
    static class ProcessNodeToMakeItCompatibleWithWpf
    {
        static XNamespace DefaultNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        static XNamespace DefaultXNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

        const string WARNING_CENTERX_CENTERY_NOT_SUPPORTED = "CenterX and CenterY are not supported in render transforms. Workaround: set the RenderTransformOrigin property on the element itself. Tip: to achieve the same transform without using CenterX and CenterY, consider experimenting with Microsoft Blend on a separate WPF or UWP project. In fact, Microsoft Blend by default also does not use the CenterX and CenterY properties for render transforms.";
        const string WARNING_TRIGGERS_NOT_SUPPORTED = "Triggers are not supported. Workaround: instead of Triggers, use the VisualStateManager class in conjunction with Storyboards. Tip: consider experimenting with Microsoft Blend on a separate UWP or Silverlight project, which also do not support triggers.";
        const string WARNING_TRANSFORMGROUP_NOT_SUPPORTED = "TransformGroup is not supported. Workaround: instead of a TransformGroup, you may use a CompositeTransform, which lets you apply multiple transforms in the following order: Scale, Skew, Rotate, Translate.";
        const string WARNING_LISTBOX_SCROLLBARS_NOT_SUPPORTED = "ListBox controls do not have built-in scroll bars. Workaround: Place the ListBox inside a ScrollViewer control. Tip: for smooth scrolling on mobile devices, set VerticalScrollBarVisibility to Visible rather than Auto.";
        const string WARNING_START_AND_END_LINECAP_MUST_BE_THE_SAME = "StrokeStartLineCap and StrokeEndLineCap must have the same value for a given element. Workaround: set both properties to the same value, or remove those properties altogether. Note: this is due to the fact that HTML5 does not support a different cap for the Start and the End of a line. The default value of those properties is: Flat.";
        const string WARNING_CHILD_WINDOW_SHOULD_HAVE_A_WIDTH_IN_PIXELS = @"It is recommended that you explicitly set a fixed Width and Height (in pixels) for the ChildWindow, otherwise it may take the size of the whole page. For example, you can set it like this: <ChildWindow Width=""400"" Height=""400""> ...";
        const string WARNING_IMAGEBRUSH_NOT_SUPPORTED = @"ImageBrush is not supported. Workaround: use an <Image> control instead. If you need to place the <Image> control behind another control, you can do so by placing both the <Image> control and the said control inside a surrounding <Grid> control. Here is an example of use of the Image control: <Image Source=""/AssemblyName;component/FolderName/ImageFileName.jpg"" Width=""320"" Height=""240""/>";

        const string WARNING_DOCKPANEL_REQUIRES_NO_PREFIX = @"To be able to use the DockPanel control, please remove its prefix (eg. for example, replace <toolkit:DockPanel> with just <DockPanel>)";
        const string WARNING_GRIDSPLITTER_REQUIRES_NO_PREFIX = @"To be able to use the GridSplitter control, please remove its prefix (eg. for example, replace <controls:GridSplitter> with just <GridSplitter>)";
        const string WARNING_DATAGRID_REQUIRES_NO_PREFIX = @"To be able to use the DataGrid control, please remove its prefix (eg. for example, replace <controls:DataGrid> with just <DataGrid>)";
        const string WARNING_EXPANDER_REQUIRES_NO_PREFIX = @"To be able to use the Expander control, please remove its prefix (eg. for example, replace <toolkit:Expander> with just <Expander>)";
        const string WARNING_NUMERICUPDOWN_REQUIRES_NO_PREFIX = @"To be able to use the NumericUpDown control, please remove its prefix (eg. for example, replace <toolkit:NumericUpDown> with just <NumericUpDown>)";

        const string TIP_FONT_FAMILY_NAMESPACE_WRONG = @"You are using the wrong namespace for the FontFamily element. You should replace it with the default namespace. Here is an example: <FontFamily x:Key=""MyKey1"">Times New Roman</FontFamily>";

        static HashSet<string> AttributesToRemove = new HashSet<string>
        {
            "AutoPlay", // MediaElement
            "Behaviors", // Interaction.Behaviors
            "DialogResult", // ChildWindow
            "HasCloseButton", // ChildWindow
            "Closed", // ChildWindow
            "Closing", // ChildWindow
            "OverlayBrush", // ChildWindow
            "OverlayOpacity", // ChildWindow
            "Title", // ChildWindow
            "IsLooping", // MediaElement
            "PageSize", // DataGrid
            "PointerPressed",
            "PointerReleased",
            "PointerMoved",
            "PointerEntered",
            "PointerExited",
            "Tapped",
            "RightTapped",
            "MouseLeftButtonDown",
            "MouseLeftButtonUp",
            "MouseMove",
            "MouseEnter",
            "MouseLeave",
            "MouseRightButtonDown",
            "MouseRightButtonUp",
            "MouseWheel",
            "UseNativeComboBox", // ComboBox
            "SelectedItemForegroundBrush", // ListBox
            "SelectedItemBackgroundBrush", // ListBox
            "SelectedItemBackground", // DataGrid
            "SelectedItemForeground", // DataGrid
            "ShowControls", // MediaElement
            "UnselectedItemBackground", // DataGrid
            "UnselectedItemForeground", // DataGrid
            "UriMapper", // Frame
            "SelectedBackground", // TabControl
            "SelectedForeground", // TabControl
            "SelectedAccent", // TabControl
            "Storyboard.TargetProperty", // We need to remove this element because it may point to stuff that we have removed, such as <CompositeTransform/> (not supported in WPF), so it may raise errors due to properties not found.
            "BindingValidationError",
            "INTERNAL_EnableProgressiveLoading",
            "VerticalCellPadding" // DataGrid
        };

        public static void Process(XElement element, ref HashSet<string> warningsAndTips)
        {
            // If the element is the "Application" class, replace it with something that does not lead to an error:
            if (element.Name == DefaultNamespace + "Application")
            {
                element.RemoveAll();
                element.RemoveAttributes();
                var border = new XElement(DefaultNamespace + "Border");
                element.ReplaceWith(border);
            }

            // Remove the element if it is an attribute among the list above (eg. <Frame.UriMapper>)
            if (element.Name.LocalName.Contains('.'))
            {
                string propertyName = element.Name.LocalName.Substring(element.Name.LocalName.IndexOf('.') + 1);
                if (AttributesToRemove.Contains(propertyName))
                {
                    element.Remove();
                }
            }

            // By default, WPF does an auto-play on the MediaElement, which is annoying in the editor because it restarts at each keystroke. Regardless of the AutoPlay value set by the user, we prevent playing the video by setting the LoadedBehavior to "Pause":
            if (element.Name == DefaultNamespace + "MediaElement")
            {
                element.Add(new XAttribute("LoadedBehavior", "Pause"));
            }

            // Replace "<HyperlinkButton Content="something"/>" with "<TextBlock><Hyperlink>something</Hyperlink></TextBlock>":
            if (element.Name == DefaultNamespace + "HyperlinkButton")
            {
                var contentAttributeIfAny = element.Attributes("Content").FirstOrDefault();
                var navigateUriAttributeIfAny = element.Attributes("NavigateUri").FirstOrDefault();
                string contentIfAny = contentAttributeIfAny != null ? contentAttributeIfAny.Value : "";
                if (contentAttributeIfAny != null)
                    contentAttributeIfAny.Remove();
                if (navigateUriAttributeIfAny != null)
                    navigateUriAttributeIfAny.Remove();
                element.Name = DefaultNamespace + "TextBlock";
                RemoveNotSupportedAttributes(element, ListOfWpfControls.GetHashSetOfTextBlockAttributes(), null);
                var hyperlink = new XElement(DefaultNamespace + "Hyperlink");
                hyperlink.Add(contentIfAny);
                element.Add(hyperlink);
            }

            // Remove MenuItems:
            if (element.Name == DefaultNamespace + "MenuItem")
            {
                element.Remove();
            }

            // Remove Storyboards: (it caused an endlessly looping error which caused an infinite amount of error windows to pop in front of us)
            if (element.Name == DefaultNamespace + "Storyboard")
            {
                element.Remove();
            }

            // Remove the content of "HtmlPresenter" nodes:
            if (element.Name.LocalName == "HtmlPresenter")
            {
                element.RemoveNodes();
            }

            //--------------------------------------------------
            // Generate warnings and tips about not supported elements:
            //--------------------------------------------------
            if (element.Name == DefaultNamespace + "Trigger")
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_TRIGGERS_NOT_SUPPORTED);
            }
            else if (element.Name.LocalName == "FontFamily" && element.Name.Namespace != DefaultNamespace)
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, TIP_FONT_FAMILY_NAMESPACE_WRONG);
            }
            else if (element.Name == DefaultNamespace + "TransformGroup")
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_TRANSFORMGROUP_NOT_SUPPORTED);
            }
            else if (element.Name == DefaultNamespace + "ImageBrush")
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_IMAGEBRUSH_NOT_SUPPORTED);
            }
            else if (element.Name.LocalName == "DockPanel" && element.Name.Namespace != DefaultNamespace)
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_DOCKPANEL_REQUIRES_NO_PREFIX);
            }
            else if (element.Name.LocalName == "GridSplitter" && element.Name.Namespace != DefaultNamespace)
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_GRIDSPLITTER_REQUIRES_NO_PREFIX);
            }
            else if (element.Name.LocalName == "DataGrid" && element.Name.Namespace != DefaultNamespace)
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_DATAGRID_REQUIRES_NO_PREFIX);
            }
            else if (element.Name.LocalName == "Expander" && element.Name.Namespace != DefaultNamespace)
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_EXPANDER_REQUIRES_NO_PREFIX);
            }
            else if (element.Name.LocalName == "NumericUpDown" && element.Name.Namespace != DefaultNamespace)
            {
                AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_NUMERICUPDOWN_REQUIRES_NO_PREFIX);
            }

            //-----------------------------------------
            // Replace "<CompositeTransform ScaleX="1" ScaleY="2" SkewX="3" SkewY="4" Rotation="5" TranslateX="6" TranslateY="7"/>" with:
            //     <TransformGroup>
            //         <ScaleTransform ScaleX="1" ScaleY="2" />
            //         <SkewTransform AngleX="3" AngleY="4" />
            //         <RotateTransform Angle="5" />
            //         <TranslateTransform X="6" Y="7" />
            //     </TransformGroup>
            //-----------------------------------------
            if (element.Name == DefaultNamespace + "CompositeTransform")
            {
                //todo: support a "CompositeTransform" that is declared inside a "TransformGroup" (in this case, we should not "rename" the "CompositeTransform" into "TransformGroup" but instead remove it from the XML and add the new elements at the same place as where the "CompositeTransform" was.

                var scaleXIfAny = ReadAttributeAndRemoveIt(element, "ScaleX");
                var scaleYIfAny = ReadAttributeAndRemoveIt(element, "ScaleY");
                var skewXIfAny = ReadAttributeAndRemoveIt(element, "SkewX");
                var skewYIfAny = ReadAttributeAndRemoveIt(element, "SkewY");
                var rotationIfAny = ReadAttributeAndRemoveIt(element, "Rotation");
                var translateXIfAny = ReadAttributeAndRemoveIt(element, "TranslateX");
                var translateYIfAny = ReadAttributeAndRemoveIt(element, "TranslateY");

                element.Name = DefaultNamespace + "TransformGroup";

                if (scaleXIfAny != null || scaleYIfAny != null)
                {
                    var scaleTransform = new XElement(DefaultNamespace + "ScaleTransform");
                    scaleTransform.SetAttributeValue("ScaleX", scaleXIfAny); //note: the attribute is not added if the value is null.
                    scaleTransform.SetAttributeValue("ScaleY", scaleYIfAny);
                    element.Add(scaleTransform);
                }
                if (skewXIfAny != null || skewYIfAny != null)
                {
                    var skewTransform = new XElement(DefaultNamespace + "SkewTransform");
                    skewTransform.SetAttributeValue("AngleX", skewXIfAny); //note: the attribute is not added if the value is null.
                    skewTransform.SetAttributeValue("AngleY", skewYIfAny);
                    element.Add(skewTransform);
                }
                if (rotationIfAny != null)
                {
                    var rotateTransform = new XElement(DefaultNamespace + "RotateTransform");
                    rotateTransform.SetAttributeValue("Angle", rotationIfAny);
                    element.Add(rotateTransform);
                }
                if (translateXIfAny != null || translateYIfAny != null)
                {
                    var translateTransform = new XElement(DefaultNamespace + "TranslateTransform");
                    translateTransform.SetAttributeValue("X", translateXIfAny); //note: the attribute is not added if the value is null.
                    translateTransform.SetAttributeValue("Y", translateYIfAny);
                    element.Add(translateTransform);
                }
            }

            // Remove unsupported style '<Setter Property="PropertyName"' properties:
            if (element.Name == DefaultNamespace + "Setter")
            {
                var propertyName = element.Attribute("Property");

                //Manually disable Setter that doesn't exist in WPF
                if (element.Parent.Name.LocalName == "Style")
                {
                    var parentAttributes = element.Parent.Attributes().ToArray();
                    foreach (XAttribute attribute in parentAttributes)
                    {
                        if (attribute.Name == "TargetType")
                        {
                            if (attribute.Value == "ContentPresenter")
                            {
                                if (ListOfWpfControls.GetHashSetOfNonCompatibleAttributeOfContentPresenter().Contains(propertyName.Value))
                                {
                                    element.Remove();
                                    propertyName = null;
                                }

                            }
                        }
                    }
                }

                if (propertyName != null && AttributesToRemove.Contains(propertyName.Value))
                {
                    element.Remove();
                }
            }

            //Add a ContentPresenter.content around the content if it's not the case
            if (element.Name == DefaultNamespace + "ContentPresenter")
            {
                bool hasContentPresenterContent = false;
                List<XElement> contentElement = new List<XElement>();
                foreach (XElement child in element.Elements())
                {
                    // verify if a content is already in the Presenter
                    if (child.Name == DefaultNamespace + "ContentPresenter.Content")
                    {
                        hasContentPresenterContent = true;
                        break;
                    }
                    // List every content elements
                    if (!child.Name.LocalName.ToString().StartsWith("ContentPresenter."))
                    {
                        contentElement.Add(child);
                    }
                }
                if (!hasContentPresenterContent && contentElement.Count > 0)
                {
                    // Remove every content Element from the main element to avoid dupplicate
                    for (int i = 0; i < contentElement.Count; i++)
                    {
                        contentElement[i].Remove();
                    }

                    var content = new XElement(DefaultNamespace + "ContentPresenter.Content");
                    content.Add(contentElement);
                    element.Add(content);
                }
            }

            if (element.Name == DefaultNamespace + "ContentPresenter")
            {
                RemoveNotSupportedAttributes(element, null, ListOfWpfControls.GetHashSetOfNonCompatibleAttributeOfContentPresenter());
            }

            // Copy the element attributes so that we can modify the collection during the ForEach:
            var attributesListCopy = element.Attributes().ToArray();

            // Fix attributes and generate warnings:
            bool isElementMarkedForCleanUp = false;
            foreach (XAttribute attribute in attributesListCopy)
            {
                // Remove attributes that are not supported in WPF:
                if (AttributesToRemove.Contains(attribute.Name.LocalName))
                {
                    attribute.Remove();
                }
                // Remove bindings completly so we don't get errors related to {binding} without any keyword or with converters
                else if (attribute.Value.StartsWith("{Binding"))
                {
                    attribute.Remove();
                }
                // Replace values of FontWeight that are not supported in WPF but are supported in WinRT:
                else if (attribute.Name == "FontWeight")
                {
                    string newFontWeight;
                    if (ReplaceFontWeightToMakeItCompatibleWithWpf(attribute.Value, out newFontWeight))
                        attribute.Value = newFontWeight;
                }

                // Detect if a "TargetType" is unsupported by WPF, in which case we should empty the node, leaving only something like <Style x:Key="MyStyle"/> so that "StaticResource" references are not broken:
                if (attribute.Name == "TargetType")
                {
                    string targetType = attribute.Value;

                    // Support the TargetType="{x:Type TypeName}" syntax in addition to the TargetType="TypeName" syntax:
                    if (targetType.StartsWith("{x:Type ") && targetType.EndsWith("}"))
                        targetType = targetType.Substring(8, targetType.Length - 8 - 1);

                    if (!IsTargetTypeSupportedByWpf(targetType))
                    {
                        isElementMarkedForCleanUp = true;
                    }
                }

                //--------------------------------------------------
                // Generate warnings and tips about not supported attributes:
                //--------------------------------------------------
                if (attribute.Name == "CenterX" || attribute.Name == "CenterY")
                {
                    if (element.Name == DefaultNamespace + "CompositeTransform"
                        || element.Name == DefaultNamespace + "RotateTransform"
                        || element.Name == DefaultNamespace + "ScaleTransform"
                        || element.Name == DefaultNamespace + "SkewTransform")
                    {
                        AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_CENTERX_CENTERY_NOT_SUPPORTED);
                    }
                }
                else if (attribute.Name == "ScrollViewer.HorizontalScrollBarVisibility" || attribute.Name == "ScrollViewer.VerticalScrollBarVisibility")
                {
                    if (element.Name == DefaultNamespace + "ListBox")
                    {
                        AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_LISTBOX_SCROLLBARS_NOT_SUPPORTED);
                    }
                }
                //else if (attribute.Name == "Grid.RowSpan" || attribute.Name == "Grid.ColumnSpan")
                //{
                //    AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_ROWSPAN_COLUMNSPAN_NOT_SUPPORTED);
                //}
                else if (attribute.Name == "StrokeStartLineCap" || attribute.Name == "StrokeEndLineCap")
                {
                    if (IsElementAShape(element))
                    {
                        // Display a warning if the "StrokeStartLineCap" is not the same as the "StrokeEndLineCap" (because in HTML5 there cannot be difference between the Start and the End)
                        if (AreStartAndEndLineCapAreDifferent(element))
                        {
                            AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_START_AND_END_LINECAP_MUST_BE_THE_SAME);
                        }
                    }
                }
            }

            //--------------------------------------------------
            // If the element was marked for cleanup, remove all its attributes (except x:Key and x:Name) and all its children. This is especially useful for cleaning styles where TargetType is not supported by WPF.
            //--------------------------------------------------
            if (isElementMarkedForCleanUp)
            {
                // Copy the element attributes so that we can modify the collection during the ForEach:
                var attributesListCopy2 = element.Attributes().ToArray();

                // Remove all attributes except x:Key and x:Name:
                foreach (var attribute in attributesListCopy2)
                {
                    if (attribute.Name != DefaultXNamespace + "Key"
                        && attribute.Name != DefaultXNamespace + "Name")
                    {
                        attribute.Remove();
                    }
                }

                // Remove all children:
                element.RemoveNodes();
            }

            //--------------------------------------------------
            // Generate a warning if the ChildWindow does not have a specified Width:
            //--------------------------------------------------
            if (element.Name == DefaultNamespace + "ChildWindow")
            {
                double unused;
                var widthAttribute = element.Attribute("Width");
                if (widthAttribute == null
                    || widthAttribute.Value == null
                    || !double.TryParse(widthAttribute.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out unused))
                {
                    AddWarningIfNotAlreadyAdded(warningsAndTips, WARNING_CHILD_WINDOW_SHOULD_HAVE_A_WIDTH_IN_PIXELS);
                }
            }
        }

        /// <summary>
        /// Replace the root control with a UserControl (if it is not already one) and keep only the properties and attributes supported by the UserControl class.
        /// </summary>
        /// <param name="xdoc">The XAML document to process</param>
        public static void ReplaceRootWithUserControl(XDocument xdoc)
        {
            if (xdoc.Root != null)
            {
                var root = xdoc.Root;
                XName formerName = xdoc.Root.Name;
                if (formerName != DefaultNamespace + "UserControl")
                {
                    // Change the root name:
                    root.Name = DefaultNamespace + "UserControl";

                    // Change the name of the child properties (for example, "<ChildWindow.Resources>" must be changed into "<UserControl.Resources>"):
                    var elementsCopy = root.Elements().ToArray(); //Note: creating this copy allows to modify the collection inside the foreach.
                    foreach (var childElement in elementsCopy)
                    {
                        if (childElement.Name.Namespace == formerName.Namespace
                            && childElement.Name.LocalName.StartsWith(formerName.LocalName + "."))
                        {
                            // Either rename the element or remove it if it is a property that is not supported by UserControl:
                            string propertyName = childElement.Name.LocalName.Substring(childElement.Name.LocalName.IndexOf(".") + 1);
                            if (ListOfWpfControls.GetHashSetOfUserControlAttributes().Contains(propertyName))
                            {
                                childElement.Name = DefaultNamespace + "UserControl." + propertyName;
                            }
                            else
                            {
                                childElement.Remove();
                            }
                        }
                    }

                    // Remove the attributes that are not supported by UserControl:
                    RemoveNotSupportedAttributes(root, ListOfWpfControls.GetHashSetOfUserControlAttributes(), null);
                }
            }
        }

        public static void RemoveNotSupportedAttributes(XElement element, HashSet<string> listOfSupportedAttributes, HashSet<string> listOfNotSupportedAttributes)
        {
            var attributesListCopy = element.Attributes().ToArray(); //Note: creating this copy enables to modify the collection during the foreach.
                foreach (var attribute in attributesListCopy)
                {
                    if (((listOfNotSupportedAttributes != null && listOfNotSupportedAttributes.Contains(attribute.Name.LocalName)) || (listOfSupportedAttributes != null && !listOfSupportedAttributes.Contains(attribute.Name.LocalName)))
                        && attribute.Name.Namespace != XNamespace.Xmlns
                        && attribute.Name.LocalName != "xmlns"
                        && attribute.Name.Namespace != (XNamespace)"http://schemas.openxmlformats.org/markup-compatibility/2006" // Note: this is for the "mc:ignorable" attribute for example, which we want to keep.
                        && !attribute.Name.LocalName.Contains(".")) // Note: this is for attached properties, such as "Canvas.Left" and "DockPanel.Dock", which we want to keep.
                    {
                        attribute.Remove();
                    }
                }
        }

        static bool AreStartAndEndLineCapAreDifferent(XElement element)
        {
            var startAttr = element.Attribute("StrokeStartLineCap");
            string startAttrValueUppercase = startAttr != null ? startAttr.Value.ToUpper() : "FLAT";
            var endAttr = element.Attribute("StrokeEndLineCap");
            string endAttrValueUppercase = endAttr != null ? endAttr.Value.ToUpper() : "FLAT";

            return (startAttrValueUppercase != endAttrValueUppercase);
        }

        static bool IsElementAShape(XElement element)
        {
            return element.Name == DefaultNamespace + "Ellipse"
                || element.Name == DefaultNamespace + "Line"
                || element.Name == DefaultNamespace + "Path"
                || element.Name == DefaultNamespace + "Polygon"
                || element.Name == DefaultNamespace + "Polyline"
                || element.Name == DefaultNamespace + "Rectangle"
                || element.Name == DefaultNamespace + "Shape";
        }

        static void AddWarningIfNotAlreadyAdded(HashSet<string> warningsAndTips, string text)
        {
            if (!warningsAndTips.Contains(text))
                warningsAndTips.Add(text);
        }

        static string ReadAttributeAndRemoveIt(XElement element, string attributeName)
        {
            var attribute = element.Attributes(attributeName).FirstOrDefault();
            string value = null;
            if (attribute != null)
            {
                attribute.Remove();
                value = attribute.Value;
            }
            return value;
        }

        static bool ReplaceFontWeightToMakeItCompatibleWithWpf(string fontWeight, out string newFontWeight)
        {
            switch (fontWeight)
            {
                case "SemiLight":
                    newFontWeight = "Light";
                    return true;
                default:
                    break;
            }
            newFontWeight = "";
            return false;
        }

        static bool IsTargetTypeSupportedByWpf(string targetType)
        {
            return ListOfWpfControls.GetHashSetOfWpfControls().Contains(targetType);
        }
    }
}
