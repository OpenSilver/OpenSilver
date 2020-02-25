﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if MIGRATION
using CSHTML5.Internal.System.Windows.Data;
#else
using CSHTML5.Internal.Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    class PropertyPathWalker : IPropertyPathNodeListener
    {
        string Path;
        bool IsDataContextBound;
        object Source;
        internal object ValueInternal;
        internal IPropertyPathNode FirstNode = null; //first node of the chained list of the nodes
        internal IPropertyPathNode FinalNode = null;
        private IPropertyPathWalkerListener _listener;


        internal PropertyPathWalker(string path, bool isDatacontextBound)
        {
            Path = path;
            StandardPropertyPathNode lastNode;
            IsDataContextBound = isDatacontextBound;

            if (path == null || path == ".")
            {
                //bindsDirectlyToSource set to true means that the binding is directly made to the source (--> there is no path)
                lastNode = new StandardPropertyPathNode(); //what to put in there ?
                FirstNode = lastNode;
                FinalNode = lastNode;
            }
            else
            {
                PropertyNodeType type;
                var parser = new PropertyPathParser(path);
                string typeName;
                string propertyName;
                string index;
                //IPropertyPathNode node;
                while ((type = parser.Step(out typeName, out propertyName, out index)) != PropertyNodeType.None)
                {
                    //we make node advance (when it is not the first step, otherwise it stays at null)
                    //node = FinalNode;

                    //var isViewProperty = false;
                    //boolean isViewProperty = CollectionViewProperties.Any (prop => prop.Name == propertyName);
                    //          static readonly PropertyInfo[] CollectionViewProperties = typeof (ICollectionView).GetProperties ();
                    
                    switch (type) {
                        case PropertyNodeType.AttachedProperty:
                        case PropertyNodeType.Property:
                            if (FinalNode == null)
                            {
                                FinalNode = new StandardPropertyPathNode(typeName, propertyName);
                            }
                            else
                            {
                                FinalNode.Next = new StandardPropertyPathNode(typeName, propertyName);
                            }
                            break;
                        case PropertyNodeType.Indexed:
                            //throw new NotImplementedException("Indexed properties are not supported yet.");
                            //todo: when we will handle the indexed properties, uncomment the following
                            FinalNode.Next = new IndexedPropertyPathNode(index);
                            break;
                        default:
                            break;
                    }

                    if(FirstNode == null)
                        FirstNode = FinalNode;

                    if (FinalNode.Next != null)
                    {
                        FinalNode = FinalNode.Next;
                    }
                }
            }

            this.FinalNode.Listen(this);
        }

        internal void Listen(IPropertyPathWalkerListener listener ) { this._listener = listener; }
        internal void Unlisten(IPropertyPathWalkerListener listener)
        {
            if (this._listener == listener)
                this._listener = null;
        }

        internal void Update(object source) {
            this.Source = source;
            this.FirstNode.SetSource(source);
        }

        internal bool IsPathBroken
        {
            get
            {
                var path = this.Path;
                if (this.IsDataContextBound && (path == null || path.Length < 1))
                    return false;

                var node = this.FirstNode;
                while (node != null) {
                    if (node.IsBroken)
                        return true;
                    node = node.Next;
                }
                return false;
            }
        }

        void IPropertyPathNodeListener.IsBrokenChanged(IPropertyPathNode node)
        {
            ValueInternal = node.Value;
            var listener = _listener;
            if (listener != null)
                listener.IsBrokenChanged();
        }

        void IPropertyPathNodeListener.ValueChanged(IPropertyPathNode node)
        {
            ValueInternal = node.Value;
            var listener = _listener;
            if (listener != null)
                listener.ValueChanged();
        }
    }
}
