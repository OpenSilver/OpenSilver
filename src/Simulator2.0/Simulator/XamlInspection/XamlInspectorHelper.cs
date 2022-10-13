

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
using Microsoft.Web.WebView2.Wpf;
using OpenSilver.Simulator;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OpenSilver.Simulator.XamlInspection
{
    internal static class XamlInspectionHelper
    {
        private static SimOverlayCallback _SimOverlayCallback;
        private static MethodInfo _FindElementInHostInfo;
        private static MethodInfo _GetVisualParent;
        private static Dictionary<object, TreeNode> _XamlSourcePathNodes;

        public static bool TryInitializeTreeView(TreeView treeView)
        {
            _XamlSourcePathNodes = new Dictionary<object, TreeNode>();
            IEnumerable treeRootElements = GetVisualTreeRootElements();
            if (treeRootElements != null)
            {
                treeView.Items.Clear();

                foreach (object treeRootElement in treeRootElements)
                {
                    treeView.Items.Add(RecursivelyAddElementsToTree(treeRootElement, false, null, 6, true));
                }

                return true;
            }
            else
                return false;
        }

        public static TreeNode RecursivelyAddElementsToTree(dynamic uiElement, bool alreadyInsertedANodeForXamlSourcePath, TreeNode parentNode, int maxTreeLevel, bool includeEntryElement)
        {
            // If the element is a XAML root element (that is, if its "XamlSourcePath" property has been filled), we add a node to the tree that tells us in which XAML file the element is defined:
            string xamlSourcePathOrNull = alreadyInsertedANodeForXamlSourcePath ? null : GetXamlSourcePathOrNullFromElement(uiElement); ;
            bool isNodeForXamlSourcePath = !string.IsNullOrEmpty(xamlSourcePathOrNull) && includeEntryElement;
            var currMaxLevel = maxTreeLevel;

            TreeNode treeNode;
            if (isNodeForXamlSourcePath)
            {
                string fileName = GetFileNameFromPath(xamlSourcePathOrNull);
                // Create the tree node for displaying the XAML source path:
                if (includeEntryElement)
                {
                    treeNode = new TreeNode()
                    {
                        Title = xamlSourcePathOrNull, //"---- File: " + fileName + " ----",
                        IsNodeForXamlSourcePath = isNodeForXamlSourcePath,
                        //XamlSourcePathOrNull = (xamlSourcePathOrNull != fileName ? "(" + fileName + ")" : null),
                        Children = new ObservableCollection<TreeNode>(),
                        Parent = parentNode
                    };
                }
                else
                {
                    treeNode = parentNode;
                }
                _XamlSourcePathNodes.Add(uiElement, treeNode);
                // Call itself and set "alreadyInsertedANodeForXamlSourcePath" to true:
                treeNode.AreChildrenLoaded = true;
                treeNode.Children.Add(RecursivelyAddElementsToTree(uiElement, true, treeNode, maxTreeLevel == -1 ? -1 : currMaxLevel - 1, true));
            }
            else
            {
                // Create the tree node for displaying the element:
                if (includeEntryElement)
                {
                    treeNode = new TreeNode()
                    {
                        Element = (object)uiElement,
                        Title = GetTitleFromElement(uiElement),
                        Name = GetNameOrNullFromElement(uiElement),
                        Children = new ObservableCollection<TreeNode>(),
                        Parent = parentNode
                    };
                }
                else
                {
                    treeNode = parentNode;
                }
                treeNode.AreChildrenLoaded = true;
                // Handle the children recursively:
                IDictionary visualChildrenInformation = uiElement.INTERNAL_VisualChildrenInformation as IDictionary;
                if (visualChildrenInformation != null)
                {
                    if (currMaxLevel > 0 || maxTreeLevel == -1)
                    {
                        if (maxTreeLevel != -1) currMaxLevel--;
                        foreach (dynamic item in visualChildrenInformation.Values) // This corresponds to elements of type "INTERNAL_VisualChildInformation" in the "Core" assembly.
                        {
                            var childElement = item.INTERNAL_UIElement;
                            if (childElement != null)
                            {
                                if (treeNode.Title == "Window" && (GetTitleFromElement(childElement) == "TextBlock" || GetTitleFromElement(childElement) == "TextBox"))
                                    return treeNode;

                                TreeNode childNode;

                                if (_XamlSourcePathNodes.ContainsKey(childElement))
                                    childNode = (_XamlSourcePathNodes[childElement] as TreeNode).Children.SingleOrDefault(nd => nd.Element == childElement);
                                else
                                    childNode = treeNode.Children.SingleOrDefault(nd => nd.Element == childElement);

                                if (childNode == null)
                                    treeNode.Children.Add(RecursivelyAddElementsToTree(childElement, isNodeForXamlSourcePath, treeNode, currMaxLevel, true));
                                else
                                    RecursivelyAddElementsToTree(childElement, isNodeForXamlSourcePath, childNode, currMaxLevel, false);

                            }
                        }
                    }
                    else
                        treeNode.AreChildrenLoaded = false;
                }
            }
            return treeNode;
        }

        public static TreeNode AddElementBranchToTree(List<dynamic> elementBranch, TreeNode parentNode)
        {
            //elementBranch arg is a branch starting from lowest leaf and going up

            TreeNode lastLeafNode = null;

            var leafNode = parentNode;

            for (int i = elementBranch.Count - 1; i > 0; i--)
            {
                parentNode.AreChildrenLoaded = true;
                var parentElement = elementBranch[i];

                IDictionary visualChildrenInformation = parentElement.INTERNAL_VisualChildrenInformation as IDictionary;
                if (visualChildrenInformation != null)
                {
                    foreach (dynamic item in visualChildrenInformation.Values) // This corresponds to elements of type "INTERNAL_VisualChildInformation" in the "Core" assembly.
                    {
                        var childElement = item.INTERNAL_UIElement;
                        if (childElement != null)
                        {
                            var treeNode = new TreeNode()
                            {
                                Element = (object)childElement,
                                Title = GetTitleFromElement(childElement),
                                Name = GetNameOrNullFromElement(childElement),
                                Children = new ObservableCollection<TreeNode>(),
                                Parent = parentNode,
                                AreChildrenLoaded = childElement.INTERNAL_VisualChildrenInformation == null || (childElement.INTERNAL_VisualChildrenInformation as IDictionary).Count == 0
                            };
                            parentNode.Children.Add(treeNode);
                            if (childElement.Equals(elementBranch[i - 1]))
                                leafNode = treeNode;
                            if (i == 1 && childElement.Equals(elementBranch[0]))
                                lastLeafNode = treeNode;
                        }
                    }
                    parentNode = leafNode;
                }
            }

            //lastLeafNode.AreChildrenLoaded = elementBranch.First().INTERNAL_VisualChildrenInformation == null || (elementBranch.First().INTERNAL_VisualChildrenInformation as IDictionary).Count == 0;
            return lastLeafNode;
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
            //Assembly coreAssembly =
            //    (from a in AppDomain.CurrentDomain.GetAssemblies()
            //     where a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY
            //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BRIDGE
            //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION
            //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE
            //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR
            //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR
            //     select a).FirstOrDefault();
            Assembly coreAssembly = OpenSilverRuntime.OSRuntimeAssembly;
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

        static object GetVisualParent(object uiElement)
        {
            if (_GetVisualParent == null)
            {
                // Find the "Core" assembly among the loaded assemblies:
                //Assembly coreAssembly =
                //    (from a in AppDomain.CurrentDomain.GetAssemblies()
                //     where a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BRIDGE
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR
                //     select a).FirstOrDefault();

                Assembly coreAssembly = OpenSilverRuntime.OSRuntimeAssembly;
                var visualTreeHelperType = coreAssembly.GetType("System.Windows.Media.VisualTreeHelper");
                _GetVisualParent = visualTreeHelperType.GetMethod("GetParent", BindingFlags.Public | BindingFlags.Static);
            }
            return _GetVisualParent.Invoke(null, new object[] { uiElement });
        }

        public static object GetVisualElementAtPoint(Point coordinates)
        {
            if (_FindElementInHostInfo == null)
            {
                // Find the "Core" assembly among the loaded assemblies:
                //Assembly coreAssembly =
                //    (from a in AppDomain.CurrentDomain.GetAssemblies()
                //     where a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BRIDGE
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BRIDGE
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_USING_BLAZOR
                //     || a.GetName().Name == Constants.NAME_OF_CORE_ASSEMBLY_SLMIGRATION_USING_BLAZOR
                //     select a).FirstOrDefault();
                Assembly coreAssembly = OpenSilverRuntime.OSRuntimeAssembly;
                Type manager = (from type in coreAssembly.GetTypes() where (type.Namespace == "CSHTML5.Internal" && type.Name == "INTERNAL_HtmlDomManager") select type).FirstOrDefault();
                _FindElementInHostInfo = manager.GetMethod("FindElementInHostCoordinates_UsedBySimulatorToo", BindingFlags.Public | BindingFlags.Static);
            }

            if (_FindElementInHostInfo != null)
            {
                //With WebView2 we get a dpi aware values so commenting these:
                //double dpiAwareX = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(coordinates.X, invert: true);
                //double dpiAwareY = ScreenCoordinatesHelper.ConvertWidthOrNaNToDpiAwareWidthOrNaN(coordinates.Y, invert: true);

                var element = _FindElementInHostInfo.Invoke(null, new object[] { coordinates.X, coordinates.Y });
                return element;
            }
            else
                return null;
        }

        public static void HighlightElementUsingJS(object uiElement, int highlightClr)
        {
            if (uiElement != null)
            {
                string uniqueIdentifier = ((dynamic)((dynamic)uiElement).INTERNAL_OuterDomElement).UniqueIdentifier.ToString();
                uniqueIdentifier = uniqueIdentifier != null ? $"'{uniqueIdentifier}'" : "null";
                SimBrowser.Instance.ExecuteScriptAsync($"simHighlightElement({uniqueIdentifier}, {highlightClr})");
            }
        }

        public static void StartInspection()
        {
            if (_SimOverlayCallback == null)
            {
                _SimOverlayCallback = new SimOverlayCallback();
                SimBrowser.Instance.CoreWebView2.AddHostObjectToScript("SimOverlayCallback", _SimOverlayCallback);
            }
            SimBrowser.Instance.ExecuteScriptAsync("startXamlInspection()");
        }

        public static void StopInspection()
        {
            SimBrowser.Instance.ExecuteScriptAsync("stopXamlInspection()");
        }

        private static async Task<object> GetElementAtPoint(int x, int y)
        {
            var element = await SimulatorProxy.OSDispatcher.InvokeAsync(() =>
            {
                return XamlInspectionHelper.GetVisualElementAtPoint(new Point(x, y));
            });

            return element;
        }

        public static async void HighlightElementAtPoint(int x, int y)
        {
            XamlInspectionHelper.HighlightElementUsingJS(await GetElementAtPoint(x, y), 1);
        }

        public static async void SelectElementAtPoint(int x, int y)
        {
            var element = await GetElementAtPoint(x, y);
            XamlInspectionHelper.HighlightElementUsingJS(element, 2);

            if (element != null)
            {
                var rootNode = MainWindow.Instance.XamlInspectionTree.XamlTree.Items.GetItemAt(0) as TreeNode;

                var elementNode = MainWindow.Instance.XamlInspectionTree.FindElementNode(element, rootNode);

                if (elementNode == null)
                {
                    var elementTreeBranch = new List<dynamic>();
                    elementTreeBranch.Add(element);
                    var firstLoadedNode = elementNode;
                    while (firstLoadedNode == null)
                    {
                        var parentElement = GetVisualParent(element);
                        elementTreeBranch.Add(parentElement);
                        firstLoadedNode = MainWindow.Instance.XamlInspectionTree.FindElementNode(parentElement, rootNode);
                        element = parentElement;
                    }
                    elementNode = XamlInspectionHelper.AddElementBranchToTree(elementTreeBranch, firstLoadedNode);
                }

                MainWindow.Instance.XamlInspectionTree.SelectTreeItem(elementNode);
                MainWindow.Instance.XamlInspectionTree.ExpandToNode(null, elementNode);

                // Select the TreeNode in the Visual Tree Inspector that corresponds to the specified element:
                //if (!MainWindow.Instance.XamlInspectionTree.TrySelectTreeNode(element))
                //{
                //    MessageBox.Show("The selected element was not found in the visual tree. Please make sure that the visual tree is up to date by clicking the 'Refresh' button in the top-right corner of the window, and try again.");
                //}
            }
            else
            {
                MessageBox.Show("No item was selected by the XAML Visual Tree inspector.");
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
