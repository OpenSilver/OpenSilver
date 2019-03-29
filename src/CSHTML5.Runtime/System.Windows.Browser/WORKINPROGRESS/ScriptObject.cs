
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
using System.Dynamic;
using System.Linq.Expressions;

namespace System.Windows.Browser
{
#if WORKINPROGRESS
    public class ScriptObject : IDynamicMetaObjectProvider
    {
    #region Not supported yet
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return default(DynamicMetaObject);
        }

        //
        // Summary:
        //     Invokes a method on the current scriptable object, and optionally passes
        //     in one or more method parameters.
        //
        // Parameters:
        //   name:
        //     The method to invoke.
        //
        //   args:
        //     Parameters to be passed to the method.
        //
        // Returns:
        //     An object that represents the return value from the underlying JavaScript
        //     method.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     name is null.
        //
        //   System.ArgumentException:
        //     name is an empty string.-or-name contains an embedded null character (\0).-or-The
        //     method does not exist or is not scriptable.
        //
        //   System.InvalidOperationException:
        //     The underlying method invocation results in an error. The .NET Framework
        //     attempts to return the error text that is associated with the error.
        public virtual object Invoke(string name, params object[] args)
        {
            return null;
        }
    #endregion
    }
#endif
}

