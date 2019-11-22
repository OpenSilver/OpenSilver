
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
        private bool _isSealed;

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
