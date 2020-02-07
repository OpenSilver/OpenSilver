
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
using System.Dynamic;
using System.Linq.Expressions;

namespace System.Windows.Browser
{
#if WORKINPROGRESS
    public class ScriptObject : IDynamicMetaObjectProvider
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

