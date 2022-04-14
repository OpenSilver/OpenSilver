
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

#if BRIDGE

using System.Threading;

namespace System.ComponentModel
{
    public sealed class AsyncOperation
    {
        private bool _alreadyCompleted;

        /// <summary>
        ///     Constructor. Protected to avoid unwitting usage - AsyncOperation objects
        ///     are typically created by AsyncOperationManager calling CreateOperation.
        /// </summary>
        private AsyncOperation(object userSuppliedState)
        {
            UserSuppliedState = userSuppliedState;
            _alreadyCompleted = false;
        }

        public object UserSuppliedState { get; }

        public void PostOperationCompleted(SendOrPostCallback d, object arg)
        {
            VerifyNotCompleted();
            VerifyDelegateNotNull(d);
            _alreadyCompleted = true;
            d(arg);
        }

        public void OperationCompleted()
        {
            VerifyNotCompleted();
            _alreadyCompleted = true;
        }

        private void VerifyNotCompleted()
        {
            if (_alreadyCompleted)
            {
                throw new InvalidOperationException("This operation has already had OperationCompleted called on it and further calls are illegal.");
            }
        }

        private void VerifyDelegateNotNull(SendOrPostCallback d)
        {
            if (d == null)
            {
                throw new ArgumentNullException(nameof(d), "A non-null SendOrPostCallback must be supplied.");
            }
        }

        /// <summary>
        ///     Only for use by AsyncOperationManager to create new AsyncOperation objects
        /// </summary>
        internal static AsyncOperation CreateOperation(object userSuppliedState)
        {
            AsyncOperation newOp = new AsyncOperation(userSuppliedState);
            return newOp;
        }
    }

    public static class AsyncOperationManager
    {
        public static AsyncOperation CreateOperation(object userSuppliedState)
        {
            return AsyncOperation.CreateOperation(userSuppliedState);
        }
    }
}

#endif
