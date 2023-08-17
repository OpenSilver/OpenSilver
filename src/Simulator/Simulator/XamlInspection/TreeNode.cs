

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



using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OpenSilver.Simulator.XamlInspection
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
                NotifiyPropertyChanged("Title");
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifiyPropertyChanged("Name");
            }
        }


        private ObservableCollection<TreeNode> _children;
        public ObservableCollection<TreeNode> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                NotifiyPropertyChanged("Children");
            }
        }

        private object _element;
        public object Element
        {
            get { return _element; }
            set
            {
                _element = value;
                NotifiyPropertyChanged("Element");
            }
        }

        private bool _isNodeForXamlSourcePath;
        public bool IsNodeForXamlSourcePath
        {
            get { return _isNodeForXamlSourcePath; }
            set
            {
                _isNodeForXamlSourcePath = value;
                NotifiyPropertyChanged("IsNodeForXamlSourcePath");
            }
        }

        private string _xamlSourcePathOrNull;
        public string XamlSourcePathOrNull
        {
            get { return _xamlSourcePathOrNull; }
            set
            {
                _xamlSourcePathOrNull = value;
                NotifiyPropertyChanged("XamlSourcePathOrNull");
            }
        }


        void NotifiyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TreeNode Parent { get; set; }


        bool _AreChildrenLoaded = true;
        public bool AreChildrenLoaded
        {
            get { return _AreChildrenLoaded; }
            set
            {
                _AreChildrenLoaded = value;
                NotifiyPropertyChanged("AreChildrenLoaded");
            }
        }

        bool _IsSelectedNodeChild = false;
        public bool IsSelectedNodeChild
        {
            get { return _IsSelectedNodeChild; }
            set
            {
                _IsSelectedNodeChild = value;
                NotifiyPropertyChanged("IsSelectedNodeChild");
            }
        }

        bool _IsActiveNodeAncestor = false;
        public bool IsActiveNodeAncestor
        {
            get { return _IsActiveNodeAncestor; }
            set
            {
                _IsActiveNodeAncestor = value;
                NotifiyPropertyChanged("IsActiveNodeAncestor");
            }
        }

        public override string ToString()
        {
            return _title;
        }
    }
}
