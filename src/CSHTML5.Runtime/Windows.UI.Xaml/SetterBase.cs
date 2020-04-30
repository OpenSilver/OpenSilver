

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
    public abstract partial class SetterBase : DependencyObject
    {
        private bool _isSealed;

        internal SetterBase()
        {
            this.CanBeInheritanceContext = false;
        }

        public bool IsSealed
        {
            get
            {
                return this._isSealed;
            }
        }
        ///// <summary>
        ///// Gets a value that indicates whether this object is in an immutable state.
        ///// </summary>
        //public bool IsSealed
        //{
        //    get { return (bool)GetValue(IsSealedProperty); }
        //    internal set { SetValue(IsSealedProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the IsSealed dependency property.
        ///// </summary>
        //public static readonly DependencyProperty IsSealedProperty =
        //    DependencyProperty.Register("IsSealed", typeof(bool), typeof(SetterBase), new PropertyMetadata(null, IsSealed_Changed));


        //static void IsSealed_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //todo: fill this
        //}
    }
}
