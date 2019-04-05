
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



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    /// <summary>
    /// Represents the base class for Expressions such as BindingExpression.
    /// </summary>
    public class Expression
    {
        internal bool IsUpdating;
        internal bool IsAttached;
        internal virtual object GetValue(DependencyProperty propd, Type typeOfDependencyObjectContainingDependencyProperty) { throw new Exception("Entered a method that should not be called."); }
        internal virtual void OnAttached(DependencyObject target)
        {
            this.IsAttached = true;
            //get the DataContextProperty.
            string dataContextPropertyName = "DataContext";
            Type type = target.GetType();
            DependencyProperty dataContextDependencyProperty = INTERNAL_TypeToStringsToDependencyProperties.GetPropertyInTypeOrItsBaseTypes(type, dataContextPropertyName);
            if (dataContextDependencyProperty != null)
            {
                object dataContext = target.GetValue(dataContextDependencyProperty);
                this.OnDataContextChanged(dataContext);
            }
        }
        internal virtual void OnDetached(DependencyObject target) {
            this.IsAttached = false;
            this.OnDataContextChanged(null);
        }
        internal virtual void OnDataContextChanged(object newDataContext) { }
    }
}
