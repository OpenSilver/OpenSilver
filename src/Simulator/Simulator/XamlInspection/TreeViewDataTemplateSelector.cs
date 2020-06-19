using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DotNetForHtml5.EmulatorWithoutJavascript.XamlInspection
{
    public class TreeNodeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate XamlSourcePathTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            TreeNode treeNode = (TreeNode)item;
            if (treeNode.IsNodeForXamlSourcePath)
            {
                return XamlSourcePathTemplate;
            }
            else
            {
                return NormalTemplate;
            }
        }
    }
}
