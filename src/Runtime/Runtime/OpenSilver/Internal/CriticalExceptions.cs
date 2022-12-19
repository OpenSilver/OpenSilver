
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
using System.Security;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;

namespace OpenSilver.Internal
{
    internal static class CriticalExceptions
    {
        // these are all the exceptions considered critical by PreSharp
        internal static bool IsCriticalException(Exception ex)
        {
            ex = Unwrap(ex);

            return ex is NullReferenceException ||
                   ex is StackOverflowException ||
                   ex is OutOfMemoryException ||
                   ex is ThreadAbortException ||
                   ex is SEHException ||
                   ex is SecurityException;
        }

        // these are exceptions that we should treat as critical when they
        // arise during callbacks into application code
        internal static bool IsCriticalApplicationException(Exception ex)
        {
            ex = Unwrap(ex);

            return ex is StackOverflowException ||
                   ex is OutOfMemoryException ||
                   ex is ThreadAbortException ||
                   ex is SecurityException;
        }

        internal static Exception Unwrap(Exception ex)
        {
            // for certain types of exceptions, we care more about the inner
            // exception
            while (ex.InnerException != null && ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }

            return ex;
        }
    }
}
