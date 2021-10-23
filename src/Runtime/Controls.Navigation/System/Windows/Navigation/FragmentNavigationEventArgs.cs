

/*===================================================================================
*
*   Copyright (c) Userware/OpenSilver.net
*
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*
\*====================================================================================*/


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
