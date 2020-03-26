

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


#if WORKINPROGRESS || true

namespace System.ComponentModel
{
    //
    // Summary:
    //     Provides data for the System.ComponentModel.ICollectionView.CurrentChanging event.
    public class CurrentChangingEventArgs : EventArgs
    {
        private bool _cancel;
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.CurrentChangingEventArgs
        //     class and sets the System.ComponentModel.CurrentChangingEventArgs.IsCancelable
        //     property to true.
        public CurrentChangingEventArgs()
        {
            IsCancelable = true;
        }
        
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.CurrentChangingEventArgs
        //     class and sets the System.ComponentModel.CurrentChangingEventArgs.IsCancelable
        //     property to the specified value.
        //
        // Parameters:
        //   isCancelable:
        //     true to disable the ability to cancel a System.ComponentModel.ICollectionView.CurrentItem
        //     change; false to enable cancellation.
        public CurrentChangingEventArgs(bool isCancelable)
        {
            IsCancelable = isCancelable;
        }

        //
        // Summary:
        //     Gets or sets a value that indicates whether the System.ComponentModel.ICollectionView.CurrentItem
        //     change should be canceled.
        //
        // Returns:
        //     true if the event should be canceled; otherwise, false. The default is false.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.ComponentModel.CurrentChangingEventArgs.IsCancelable property value
        //     is false.
        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                if (!IsCancelable)
                    throw new InvalidOperationException("The IsCancelable property value is false.")
                _cancel = value;
            }
        }
        //
        // Summary:
        //     Gets a value that indicates whether the System.ComponentModel.ICollectionView.CurrentItem
        //     change can be canceled.
        //
        // Returns:
        //     true if the event can be canceled; false if the event cannot be canceled.
        public bool IsCancelable { get; private set; }
    }
}

#endif