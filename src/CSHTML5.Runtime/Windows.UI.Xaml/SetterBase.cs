
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents the base class for value setters.
    /// </summary>
    public class SetterBase : DependencyObject
    {
        /// <summary>
        /// Gets a value that indicates whether this object is in an immutable state.
        /// </summary>
        public bool IsSealed
        {
            get { return (bool)GetValue(IsSealedProperty); }
            internal set { SetValue(IsSealedProperty, value); }
        }
        /// <summary>
        /// Identifies the IsSealed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSealedProperty =
            DependencyProperty.Register("IsSealed", typeof(bool), typeof(SetterBase), new PropertyMetadata(null, IsSealed_Changed));


        static void IsSealed_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: fill this
        }
    }
}
