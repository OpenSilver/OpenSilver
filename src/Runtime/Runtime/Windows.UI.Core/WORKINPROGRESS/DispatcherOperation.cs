

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


#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Threading
#else
namespace Windows.UI.Core
#endif
{
    public enum DispatcherOperationStatus
    {
        Pending,
        Aborted,
        Completed,
        Executing
    }
    public sealed partial class DispatcherOperation
    {
        public event EventHandler Completed;
        public event EventHandler Aborted;

        public DispatcherPriority Priority { get; private set; }
        public DispatcherOperationStatus Status { get; private set; }
        public object Result { get; private set; }

        private Func<object> action;
        internal DispatcherOperation()
        {

        }
        public DispatcherOperation(Action action, DispatcherPriority priority) :
            this(() => { action(); return null; }, priority)
        {
            //
        }

        public DispatcherOperation(Func<object> action, DispatcherPriority priority)
        {
            this.action = action;
            this.Priority = priority;
        }

        public void Abort()
        {
            if (Status != DispatcherOperationStatus.Pending)
            {
                throw new Exception($"Operation is \"{Status}\" and cannot be aborted");
            }

            Status = DispatcherOperationStatus.Aborted;
            Aborted.Raise(this);
        }

        public void Invoke()
        {
            if (Status != DispatcherOperationStatus.Pending)
            {
                throw new Exception($"Operation is \"{Status}\" and cannot be invoked");
            }

            Status = DispatcherOperationStatus.Executing;
            Result = action();
            Status = DispatcherOperationStatus.Completed;
            Completed.Raise(this);
        }
    }
}
#endif
