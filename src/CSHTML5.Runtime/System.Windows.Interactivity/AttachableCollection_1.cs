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
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml;
#endif

namespace System.Windows.Interactivity
{
    /// <summary>
    /// Represents a collection of IAttachedObject with a shared AssociatedObject
    /// and provides change notifications to its contents when that AssociatedObject
    /// changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class AttachableCollection<T> : List<T>, IAttachedObject where T:DependencyObject, IAttachedObject // : DependencyObjectCollection<T>, IAttachedObject where T : DependencyObject, System.Windows.Interactivity.IAttachedObject
    {
        DependencyObject _associatedObject = null;
        /// <summary>
        /// The object on which the collection is hosted.
        /// </summary>
        public DependencyObject AssociatedObject { get { return _associatedObject; } } //todo: was protected but it has to be public because it comes from an interface si I don't really understand.

       
        // Exceptions:
        //   System.InvalidOperationException:
        //     The IAttachedObject is already attached to a different object.
        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to.</param>
        public void Attach(DependencyObject dependencyObject)
        {
            if(_associatedObject != null)
            {
                throw new InvalidOperationException("The AttachableCollection is already attached to a different object.");
            }
            _associatedObject = dependencyObject;
            foreach(T element in this)
            {
                element.Attach(dependencyObject);
            }
            OnAttached();
        }
        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        public void Detach()
        {
            OnDetaching();
            foreach(T element in this)
            {
                element.Detach();
            }
            _associatedObject = null;
        }

        /// <summary>
        /// Called immediately after the collection is attached to an AssociatedObject.
        /// </summary>
        protected abstract void OnAttached();

        /// <summary>
        /// Called when the collection is being detached from its AssociatedObject, but
        /// before it has actually occurred.
        /// </summary>
        protected abstract void OnDetaching();

        public void Add(T item)
        {
            base.Add(item);
            if(_associatedObject != null)
            {
                item.Attach(_associatedObject);
            }
        }

        public bool Remove(T item)
        {
            if(base.Remove(item))
            {
                item.Detach();
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            T item = this.ElementAt(index);
            base.RemoveAt(index);
            item.Detach();
        }

        
        public void Insert(int index, T item)
        {
            base.Insert(index, item);
            if (_associatedObject != null)
            {
                item.Attach(_associatedObject);
            }
        }
    }
}
