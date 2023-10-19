//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows.Controls;

namespace System.Windows.Navigation
{
    internal sealed class NavigationCache : DependencyObject
    {
#region Fields

        private int _cacheSize;
        private Dictionary<string, Page> _cachePages;
        private List<string> _cachePagesMRU;

#endregion

#region Properties

        //This property is for testing purposes only
        internal int CachePagesSize { get { return this._cachePages.Count; } }

        //This property is for testing purposes only
        internal int CacheMRUPagesSize { get { return this._cachePagesMRU.Count; } }

        internal Page this[string uri]
        {
            get
            {
                if (this._cachePages.ContainsKey(uri))
                {
                    return this._cachePages[uri];
                }

                return null;
            }
        }

#endregion

#region NavigationCacheMode Attached Property

        internal static readonly DependencyProperty NavigationCacheModeProperty =
            DependencyProperty.RegisterAttached(
                "NavigationCacheMode",
                typeof(NavigationCacheMode),
                typeof(NavigationCache),
                new PropertyMetadata(NavigationCacheMode.Disabled));

        internal static NavigationCacheMode GetNavigationCacheMode(DependencyObject depObj)
        {
            Guard.ArgumentNotNull(depObj, "depObj");
            return (NavigationCacheMode)depObj.GetValue(NavigationCacheModeProperty);
        }

        internal static void SetNavigationCacheMode(DependencyObject depObj, NavigationCacheMode navigationCacheMode)
        {
            Guard.ArgumentNotNull(depObj, "depObj");
            depObj.SetValue(NavigationCacheModeProperty, navigationCacheMode);
        }

#endregion

#region Constructor

        internal NavigationCache(int initialCacheSize)
        {
            this._cacheSize = initialCacheSize;
            this._cachePages = new Dictionary<string, Page>(this._cacheSize);
            this._cachePagesMRU = new List<string>(this._cacheSize);
        }

#endregion

#region Methods

        internal void ChangeCacheSize(int newCacheSize)
        {
            while (this._cachePagesMRU.Count > newCacheSize)
            {
                string toRemove = this._cachePagesMRU[this._cachePagesMRU.Count - 1];

                this._cachePagesMRU.RemoveAt(this._cachePagesMRU.Count - 1);
                this._cachePages.Remove(toRemove);
            }

            this._cacheSize = newCacheSize;
        }

        internal bool Contains(string uri)
        {
            return this._cachePages.ContainsKey(uri);
        }

        internal void AddToCache(string uri, Page page)
        {
            if (this._cachePages.ContainsKey(uri))
            {
                // If it's already in the cache, bump it to the top,
                // and don't bother to examine the cache size, as we
                // are only moving stuff around, so size is not affected.
                this._cachePagesMRU.Remove(uri);
                this._cachePagesMRU.Insert(0, uri);
                this._cachePages[uri] = page;
            }
            else if (this._cacheSize > 0)
            {
                // If we're about to go over the size, instead remove the last entry before
                // adding the new one.
                if (this._cachePagesMRU.Count == this._cacheSize)
                {
                    string toRemove = this._cachePagesMRU[this._cachePagesMRU.Count - 1];
                    this._cachePagesMRU.RemoveAt(this._cachePagesMRU.Count - 1);
                    this._cachePages.Remove(toRemove);
                }

                this._cachePagesMRU.Insert(0, uri);
                this._cachePages.Add(uri, page);
            }
        }

        internal void RemoveFromCache(string uri)
        {
            this._cachePagesMRU.Remove(uri);
            this._cachePages.Remove(uri);
        }

#endregion
    }
}
