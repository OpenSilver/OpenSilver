

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Simulator (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Simulator (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using System.Linq;
using System.Windows;
using System.Collections;
using System.Windows.Shapes;
using DotNetForHtml5.Compiler;

namespace DotNetForHtml5.EmulatorWithoutJavascript.XamlInspection
{
    internal static class XamlInspectionHelper
    {
        public static bool TryInitializeTreeView(TreeView treeView, Assembly entryPointAssembly, out int NbTreeViewElement)
        {
            NbTreeViewElement = 0;
            IEnumerable treeRootElements = GetVisualTreeRootElements();
            if (treeRootElements != null)
            {
                treeView.Items.Clear();

                foreach (object treeRootElement in treeRootElements)
                {
                    treeView.Items.Add(RecursivelyAddElementsToTree(treeRootElement, false, ref NbTreeViewElement));
                }

                return true;
            }
            else
                return false;
        }

        static TreeNode RecursivelyAddElementsToTree(dynamic uiElement, bool alreadyInsertedANodeForXamlSourcePath, ref int NbTreeViewElement)
        {
            // If the element is a XAML root element (that is, if its "XamlSourcePath" property has been filled), we add a node to the tree that tells us in which XAML file the element is defined:
            string xamlSourcePathOrNull = alreadyInsertedANodeForXamlSourcePath ? null : GetXamlSourcePathOrNullFromElement(uiElement); ;
            bool isNodeForXamlSourcePath = !string.IsNullOrEmpty(xamlSourcePathOrNull);

            TreeNode treeNode;
            if (isNodeForXamlSourcePath)
            {
                string fileName = GetFileNameFromPath(xamlSourcePathOrNull);
                // Create the tree node for displaying the XAML source path:
                treeNode = new TreeNode()
                {
                    Title = xamlSourcePathOrNull, //"---- File: " + fileName + " ----",
                    IsNodeForXamlSourcePath = isNodeForXamlSourcePath,
                    //XamlSourcePathOrNull = (xamlSourcePathOrNull != fileName ? "(" + fileName + ")" : null),
                    Children = new ObservableCollection<TreeNode>(),
                };

                // Call itself and set "alreadyInsertedANodeForXamlSourcePath" to true:
                treeNode.Children.Add(RecursivelyAddElementsToTree(uiElement, true, ref NbTreeViewElement));
            }
            else
            {
                // Create the tree node for displaying the element:
                treeNode = new TreeNode()
                {
                    Element = (object)uiElement,
                    Title = GetTitleFromElement(uiElement),
                    Name = GetNameOrNullFromElement(uiElement),
                    Children = new ObservableCollection<TreeNode>()
                };

                // Handle the children recursively:
                IDictionary visualChildrenInformation = uiElement.INTERNAL_VisualChildrenInformation as IDictionary;
                if (visualChildrenInformation != null)
                {
                    foreach (dynamic item in visualChildrenInformation.Values) // This corresponds to elements of type "INTERNAL_VisualChildInformation" in the "Core" assembly.
                    {
                        var childElement = item.INTERNAL_UIElement;
                        if (childElement != null)
                        {
                            treeNode.Children.Add(RecursivelyAddElementsToTree(childElement, isNodeForXamlSourcePath, ref NbTreeViewElement));
                        }
                    }
                }
            }
            NbTreeViewElement++;
            return treeNode;
        }

        static string GetFileNameFromPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                int i = path.LastIndexOf('\\');
                if (i > 0)
                {
                    return path.Substring(i + 1);
                }
                int j = path.LastIndexOf('/');
                if (j > 0)
                {
                    return path.Substring(j + 1);
                }
                return path;
            }
            else
            {
                return "";
            }
        }

        static string GetTitleFromElement(dynamic uiElement)
        {
            if (uiElement != null)
            {
                var type = uiElement.GetType();
                if (type is Type)
                {
                    return ((Type)type).Name;
                }
                else
                    return "[UNKNOWN ELEMENT]";
            }
            else
                return "[UNKNOWN ELEMENT]";
        }

        static string GetNameOrNullFromElement(dynamic uiElement)
        {
            if (uiElement != null)
            {
                var type = uiElement.GetType();
                if (type is Type)
                {
                    var propertyInfo = ((Type)type).GetProperty("Name");
                    if (propertyInfo != null)
                        return propertyInfo.GetValue(uiElement);
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;
        }

        static string GetXamlSourcePathOrNullFromElement(dynamic uiElement)
        {
            const string xamlSourcePathMemberName = "XamlSourcePath";
            if (uiElement != null)
            {
                var type = uiElement.GetType();
                if (type is Type)
                {
                    var propertyInfo = ((Type)type).GetProperty(xamlSourcePathMemberName);
                    if (propertyInfo != null)
                    {
                        return propertyInfo.GetValue(uiElement);
                    }
                    else
                    {
                        var fieldInfo = ((Type)type).GetField(xamlSourcePathMemberName);
                        if (fieldInfo != null)
                        {
                            return fieldInfo.GetValue(uiElement);
                        }
                        else
                            return null;
                    }
                }
                else
                    return null;
            }
            else
                return null;
        }

        static IEnumerable GetVisualTreeRootElements()
        {
            // Find the "Core" assembly among the loaded assemblies:
            Assembly coreAssembly =
                (from a in AppDomain.CurrentDomain.GetAssemblies()
                 where a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BRIDGE
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR
                 select a).FirstOrDefault();
            if (coreAssembly != null)
            {
                // Find the type "INTERNAL_PopupsManager" in Core:
                Type manager = (from type in coreAssembly.GetTypes() where (type.Namespace == "DotNetForHtml5.Core" && type.Name == "INTERNAL_PopupsManager") select type).FirstOrDefault();
                if (manager != null)
                {
                    // Call the "GetAllRootUIElements" method:
                    var methodInfo = manager.GetMethod("GetAllRootUIElements", BindingFlags.Public | BindingFlags.Static);
                    if (methodInfo != null)
                    {
                        var rootElements = methodInfo.Invoke(null, null);
                        if (rootElements is IEnumerable)
                        {
                            return (IEnumerable)rootElements;
                        }
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;
        }

        public static object GetElementAtSpecifiedCoordinates(Point coordinates)
        {
            // Find the "Core" assembly among the loaded assemblies:
            Assembly coreAssembly =
                (from a in AppDomain.CurrentDomain.GetAssemblies()
                 where a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BRIDGE
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR
                 || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR
                 select a).FirstOrDefault();
            if (coreAssembly != null)
            {
                // Find the type "VisualTreeHelper" in Core:
                Type manager = (from type in coreAssembly.GetTypes() where (type.Namespace == "CSHTML5.Internal" && type.Name == "INTERNAL_HtmlDomManager") select type).FirstOrDefault();
                if (manager != null)
                {
                    // Call the "GetAllRootUIElements" method:
                    var methodInfo = manager.GetMethod("FindElementInHostCoordinates_UsedBySimulatorToo", BindingFlags.Public | BindingFlags.Static);
                    if (methodInfo != null)
                    {
                        double dpiAwareX = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(coordinates.X, invert: true);
                        double dpiAwareY = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(coordinates.Y, invert: true);

                        var element = methodInfo.Invoke(null, new object[] { dpiAwareX, dpiAwareY });
                        return element;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
                return null;
        }

        public static void HighlightElement(object userUIElementThatWeWantToHighlight, Rectangle rectangleUsedToHighlight, DotNetBrowser.Browser browser)
        {
            bool wasElementHighlighted = false;

            if (userUIElementThatWeWantToHighlight != null)
            {
                try
                {
                    // Get the coordinates of the element in HTML:
                    string uniqueIdentifier = ((dynamic)((dynamic)userUIElementThatWeWantToHighlight).INTERNAL_OuterDomElement).UniqueIdentifier.ToString();
                    if (uniqueIdentifier != null)
                    {
                        string coordinates = browser.ExecuteJavaScriptAndReturnValue(string.Format(
    @"var div = document.getElementByIdSafe('{0}');
                              var rect = div.getBoundingClientRect();
                              var result = rect.top + ';' + rect.right + ';' + rect.bottom + ';' + rect.left;
                              result;
                              ", uniqueIdentifier)).ToString();

                        string[] coordinatesArray = coordinates.Replace(',', '.').Split(';');
                        double top = double.Parse(coordinatesArray[0]);
                        double right = double.Parse(coordinatesArray[1]);
                        double bottom = double.Parse(coordinatesArray[2]);
                        double left = double.Parse(coordinatesArray[3]);

                        // Take into account the screen DPI:
                        double dpiAwareTop = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(top, invert: false);
                        double dpiAwareRight = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(right, invert: false);
                        double dpiAwareBottom = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(bottom, invert: false);
                        double dpiAwareLeft = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(left, invert: false);

                        Canvas.SetLeft(rectangleUsedToHighlight, dpiAwareLeft);
                        Canvas.SetTop(rectangleUsedToHighlight, dpiAwareTop);
                        rectangleUsedToHighlight.Width = (dpiAwareRight > dpiAwareLeft ? (dpiAwareRight - dpiAwareLeft) : 0);
                        rectangleUsedToHighlight.Height = (dpiAwareBottom > dpiAwareTop ? (dpiAwareBottom - dpiAwareTop) : 0);
                        rectangleUsedToHighlight.Visibility = Visibility.Visible;

                        // Remember the highlighted element reference:
                        rectangleUsedToHighlight.Tag = userUIElementThatWeWantToHighlight;

                        wasElementHighlighted = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            // Remove highlight if error or empty:
            if (!wasElementHighlighted)
            {
                rectangleUsedToHighlight.Width = double.NaN;
                rectangleUsedToHighlight.Height = double.NaN;
                rectangleUsedToHighlight.Visibility = Visibility.Collapsed;
            }
        }

        /*
        static dynamic GetVisualTreeRootElement()
        {
            // Find the "Core" assembly among the loaded assemblies:
            Assembly coreAssembly = (from a in AppDomain.CurrentDomain.GetAssemblies() where a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY select a).FirstOrDefault();
            if (coreAssembly != null)
            {
                // Find the "Window" type:
                Type windowType = (from type in coreAssembly.GetTypes() where (type.Namespace == "Windows.UI.Xaml" && type.Name == "Window") select type).FirstOrDefault();
                if (windowType != null)
                {
                    // Get the current window:
                    var propertyInfo = windowType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
                    if (propertyInfo != null)
                    {
                        var window = propertyInfo.GetValue(null, null);
                        return window;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        */
    }
}
