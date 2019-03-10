
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
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    /// <summary>
    /// Represents the method that handles the KeyUp and KeyDown events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
#if MIGRATION
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);
#else
    public delegate void KeyEventHandler(object sender, KeyRoutedEventArgs e);
#endif
}
