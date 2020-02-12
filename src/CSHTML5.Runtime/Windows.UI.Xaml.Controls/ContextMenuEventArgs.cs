
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

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides data for the context menu event.
    /// </summary>
    public sealed partial class ContextMenuEventArgs : RoutedEventArgs
    {
        double _pointerLeft;
        double _pointerTop;

        public ContextMenuEventArgs(double pointerLeft, double pointerTop)
        {
            _pointerLeft = pointerLeft;
            _pointerTop = pointerTop;
        }

        /// <summary>
        /// Gets the horizontal position of the mouse.
        /// </summary>
#if MIGRATION
        public double CursorLeft
#else
        public double PointerLeft
#endif
        {
            get
            {
                return _pointerLeft;
            }
        }

        /// <summary>
        /// Gets the vertical position of the mouse.
        /// </summary>
#if MIGRATION
        public double CursorTop
#else
        public double PointerTop
#endif
        {
            get
            {
                return _pointerTop;
            }
        }

        //protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget);
    }
}
