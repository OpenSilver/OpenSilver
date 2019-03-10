
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
