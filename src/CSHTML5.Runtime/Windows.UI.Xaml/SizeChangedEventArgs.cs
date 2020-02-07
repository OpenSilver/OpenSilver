
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



#if MIGRATION
using System.Windows;
#else
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Provides data related to the SizeChanged event.
    /// </summary>
    public sealed class SizeChangedEventArgs : RoutedEventArgs
    {
        Size _newSize;

        public SizeChangedEventArgs(Size newSize)
        {
            _newSize = newSize;
        }

        /// <summary>
        /// Gets the new size of the object reporting the size change.
        /// </summary>
        public Size NewSize
        {
            get
            {
                return _newSize;
            }
        }

#if WORKINPROGRESS
        /// <summary>
        /// Gets the previous size of the object reporting the size change.
        /// </summary>
        public Size PreviousSize { get; private set; }
#endif
    }
}
