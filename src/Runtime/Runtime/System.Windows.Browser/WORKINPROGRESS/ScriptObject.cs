

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
using System.Dynamic;
using System.Linq.Expressions;

namespace System.Windows.Browser
{
#if WORKINPROGRESS
    public partial class ScriptObject : IDynamicMetaObjectProvider
    {
        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return null;
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
        public object GetProperty(int @index)
        {
            return null;
        }
        public void SetProperty(int @index, object @value)
        {

        }
        public virtual object GetProperty(string @name)
        {
            return null;
        }
        public virtual void SetProperty(string @name, object @value)
        {

        }

        public virtual object InvokeSelf(params object[] args)
        {
            return null;
        }
    }
#endif
}

