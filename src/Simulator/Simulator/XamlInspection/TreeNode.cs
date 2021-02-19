

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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.EmulatorWithoutJavascript.XamlInspection
{
    public class TreeNode : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }


        private ObservableCollection<TreeNode> _children;
        public ObservableCollection<TreeNode> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                NotifyPropertyChanged("Children");
            }
        }

        private object _element;
        public object Element
        {
            get { return _element; }
            set
            {
                _element = value;
                NotifyPropertyChanged("Element");
            }
        }

        private bool _isNodeForXamlSourcePath;
        public bool IsNodeForXamlSourcePath
        {
            get { return _isNodeForXamlSourcePath; }
            set
            {
                _isNodeForXamlSourcePath = value;
                NotifyPropertyChanged("IsNodeForXamlSourcePath");
            }
        }

        private string _xamlSourcePathOrNull;
        public string XamlSourcePathOrNull
        {
            get { return _xamlSourcePathOrNull; }
            set
            {
                _xamlSourcePathOrNull = value;
                NotifyPropertyChanged("XamlSourcePathOrNull");
            }
        }


        void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
