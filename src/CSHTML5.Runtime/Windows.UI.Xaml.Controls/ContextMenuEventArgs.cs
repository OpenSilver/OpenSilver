
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
    public sealed class ContextMenuEventArgs : RoutedEventArgs
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
