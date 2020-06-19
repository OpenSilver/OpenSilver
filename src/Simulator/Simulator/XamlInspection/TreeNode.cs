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
    }
}
