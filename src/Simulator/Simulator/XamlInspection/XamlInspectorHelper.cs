
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

extern alias opensilver;

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Collections;
using DotNetForHtml5.EmulatorWithoutJavascript;
using System.Globalization;
using UIElement = opensilver::System.Windows.UIElement;

namespace OpenSilver.Simulator.XamlInspection
{
    internal static class XamlInspectionHelper
    {
        private static Dictionary<UIElement, TreeNode> _XamlSourcePathNodes;

        public static bool TryInitializeTreeView(TreeView treeView)
        {
            _XamlSourcePathNodes = new Dictionary<UIElement, TreeNode>();
            treeView.Items.Clear();
            
            foreach (UIElement treeRootElement in GetVisualTreeRootElements().OfType<UIElement>())
            {
                treeView.Items.Add(RecursivelyAddElementsToTree(treeRootElement, false, null, 6, true));
            }

            return true;
        }

        public static TreeNode RecursivelyAddElementsToTree(UIElement uiElement, bool alreadyInsertedANodeForXamlSourcePath, TreeNode parentNode, int maxTreeLevel, bool includeEntryElement)
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
                        Element = uiElement,
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
                if (uiElement.VisualChildrenInformation != null)
                {
                    if (currMaxLevel > 0 || maxTreeLevel == -1)
                    {
                        if (maxTreeLevel != -1) currMaxLevel--;
                        foreach (UIElement child in uiElement.VisualChildrenInformation)
                        {
                            if (treeNode.Title == "Window" && (GetTitleFromElement(child) == "TextBlock" || GetTitleFromElement(child) == "TextBox"))
                                return treeNode;

                            TreeNode childNode;

                            if (_XamlSourcePathNodes.ContainsKey(child))
                                childNode = _XamlSourcePathNodes[child].Children.SingleOrDefault(nd => nd.Element == child);
                            else
                                childNode = treeNode.Children.SingleOrDefault(nd => nd.Element == child);

                            if (childNode == null)
                                treeNode.Children.Add(RecursivelyAddElementsToTree(child, isNodeForXamlSourcePath, treeNode, currMaxLevel, true));
                            else
                                RecursivelyAddElementsToTree(child, isNodeForXamlSourcePath, childNode, currMaxLevel, false);
                        }
                    }
                    else
                        treeNode.AreChildrenLoaded = false;
                }
            }
            return treeNode;
        }

        public static TreeNode AddElementBranchToTree(List<UIElement> elementBranch, TreeNode parentNode)
        {
            //elementBranch arg is a branch starting from lowest leaf and going up

            TreeNode lastLeafNode = null;

            var leafNode = parentNode;

            for (int i = elementBranch.Count - 1; i > 0; i--)
            {
                parentNode.AreChildrenLoaded = true;
                var parentElement = elementBranch[i];

                if (parentElement.VisualChildrenInformation != null)
                {
                    foreach (UIElement child in parentElement.VisualChildrenInformation)
                    {
                        var treeNode = new TreeNode()
                        {
                            Element = child,
                            Title = GetTitleFromElement(child),
                            Name = GetNameOrNullFromElement(child),
                            Children = new ObservableCollection<TreeNode>(),
                            Parent = parentNode,
                            AreChildrenLoaded = child.VisualChildrenInformation == null || child.VisualChildrenInformation.Count == 0,
                        };
                        parentNode.Children.Add(treeNode);
                        if (child == elementBranch[i - 1])
                        {
                            leafNode = treeNode;
                        }
                        if (i == 1 && child == elementBranch[0])
                        {
                            lastLeafNode = treeNode;
                        }
                    }
                    parentNode = leafNode;
                }
            }

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

        private static string GetTitleFromElement(UIElement uiElement)
        {
            if (uiElement != null)
            {
                return uiElement.GetType().Name;
            }
            else
            {
                return "[UNKNOWN ELEMENT]";
            }
        }

        private static string GetNameOrNullFromElement(UIElement uiElement)
        {
            if (uiElement != null)
            {
                return (string)uiElement.GetValue(opensilver::System.Windows.FrameworkElement.NameProperty);
            }
            else
            {
                return null;
            }
        }

        private static string GetXamlSourcePathOrNullFromElement(UIElement uiElement)
        {
            if (uiElement != null)
            {
                return uiElement.XamlSourcePath;
            }
            else
            {
                return null;
            }
        }

        private static IEnumerable GetVisualTreeRootElements()
        {
            return opensilver::DotNetForHtml5.Core.PopupsManager.GetAllRootUIElements();
        }

        private static UIElement GetVisualParent(UIElement uiElement)
        {
            return opensilver::System.Windows.Media.VisualTreeHelper.GetParent(uiElement) as UIElement;
        }

        public static UIElement GetVisualElementAtPoint(Point coordinates)
        {
            string js = $@"
(function(){{
    var domElementsAtCoordinates = document.elementsFromPoint({coordinates.X.ToString(CultureInfo.InvariantCulture)}, {coordinates.Y.ToString(CultureInfo.InvariantCulture)});
    if (!domElementsAtCoordinates || domElementsAtCoordinates.length == 0)
    {{
        return null;
    }}
    var firstItem = domElementsAtCoordinates[0];
    if (firstItem === document.documentElement)
    {{
        return null;
    }}
    if (firstItem.id && firstItem.id === 'XamlInspectorOverlay')
    {{
        return domElementsAtCoordinates.length > 1 ? domElementsAtCoordinates[1] : null;
    }}
    return firstItem;
}}())";
            IDisposable domRef = opensilver::OpenSilver.Interop.ExecuteJavaScript(js);
            UIElement element = opensilver::CSHTML5.Internal.INTERNAL_HtmlDomManager.GetUIElementFromDomElement_UsedBySimulatorToo(domRef);
            domRef?.Dispose();
            return element;
        }

        public static void HighlightElementUsingJS(UIElement uiElement, int highlightClr)
        {
            if (uiElement != null)
            {
                string uniqueIdentifier = uiElement.OuterDiv.UniqueIdentifier;
                uniqueIdentifier = uniqueIdentifier != null ? $"'{uniqueIdentifier}'" : "null";
                SimulatorProxy.OpenSilverRuntimeDispatcher.BeginInvoke(() =>
                {
                    SimulatorProxy.JavaScriptExecutionHandler.ExecuteJavaScript($"XamlInspectorHighlightElement({uniqueIdentifier}, {highlightClr})");
                });
            }
        }

        public static void StartInspection()
        {
            SimulatorProxy.OpenSilverRuntimeDispatcher.BeginInvoke(() =>
            {
                SimulatorProxy.JavaScriptExecutionHandler.ExecuteJavaScript("startXamlInspection()");
            });

        }

        public static void StopInspection()
        {
            SimulatorProxy.OpenSilverRuntimeDispatcher.BeginInvoke(() =>
            {
                SimulatorProxy.JavaScriptExecutionHandler.ExecuteJavaScript("stopXamlInspection()");
            });
        }

        private static async Task<UIElement> GetElementAtPoint(int x, int y)
        {
            var element = await SimulatorProxy.OpenSilverRuntimeDispatcher.InvokeAsync(() =>
            {
                return GetVisualElementAtPoint(new Point(x, y));
            });

            return element;
        }

        public static async void HighlightElementAtPoint(int x, int y)
        {
            HighlightElementUsingJS(await GetElementAtPoint(x, y), 1);
        }

        public static async void SelectElementAtPoint(int x, int y)
        {
            UIElement element = await GetElementAtPoint(x, y);
            HighlightElementUsingJS(element, 2);

            if (element != null)
            {
                var rootNode = MainWindow.Instance.XamlInspectionTreeViewInstance.XamlTree.Items.GetItemAt(0) as TreeNode;

                var elementNode = MainWindow.Instance.XamlInspectionTreeViewInstance.FindElementNode(element, rootNode);

                if (elementNode == null)
                {
                    var elementTreeBranch = new List<UIElement> { element };
                    var firstLoadedNode = elementNode;
                    while (firstLoadedNode == null)
                    {
                        var parentElement = GetVisualParent(element);
                        elementTreeBranch.Add(parentElement);
                        firstLoadedNode = MainWindow.Instance.XamlInspectionTreeViewInstance.FindElementNode(parentElement, rootNode);
                        element = parentElement;
                    }
                    elementNode = AddElementBranchToTree(elementTreeBranch, firstLoadedNode);
                }

                MainWindow.Instance.XamlInspectionTreeViewInstance.SelectTreeItem(elementNode);
                MainWindow.Instance.XamlInspectionTreeViewInstance.ExpandToNode(null, elementNode);
            }
            else
            {
                MessageBox.Show("No item was selected by the XAML Visual Tree inspector.");
            }
        }

    }
}
