//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    ///<summary>
    ///     FragmentNavigationEventArgs exposes the fragment being navigated to
    ///     in an event fired from NavigationService to notify a listening client
    ///     that a navigation to fragment is about to occur.
    ///</summary> 
    ///<QualityBand>Stable</QualityBand>
    public sealed class FragmentNavigationEventArgs : EventArgs
    {
#region Fields

        private string _fragment;

#endregion
                
#region Constructors

        internal FragmentNavigationEventArgs(string fragment)
        {
            this._fragment = fragment;
        }

#endregion Constructors

#region Public Properties

        /// <summary>
        ///  The fragment part of the URI that was passed to the Navigate() API which initiated this navigation.
        ///  The fragment may be String.Empty.
        /// </summary>
        public string Fragment
        {
            get
            {
                return this._fragment;
            }
        }
        
#endregion Public Properties
    }
}
