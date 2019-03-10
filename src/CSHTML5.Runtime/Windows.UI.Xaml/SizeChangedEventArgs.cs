
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

        /*
        /// <summary>
        /// Gets the previous size of the object reporting the size change.
        /// </summary>
        public Size PreviousSize { get; }
         */
    }
}
