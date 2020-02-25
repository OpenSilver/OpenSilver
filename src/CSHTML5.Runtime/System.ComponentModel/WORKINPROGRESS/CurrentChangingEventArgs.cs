#if WORKINPROGRESS

namespace System.ComponentModel
{
    //
    // Summary:
    //     Provides data for the System.ComponentModel.ICollectionView.CurrentChanging event.
    public class CurrentChangingEventArgs : EventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.CurrentChangingEventArgs
        //     class and sets the System.ComponentModel.CurrentChangingEventArgs.IsCancelable
        //     property to true.
        public CurrentChangingEventArgs()
        {

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
        public bool Cancel { get; set; }
        //
        // Summary:
        //     Gets a value that indicates whether the System.ComponentModel.ICollectionView.CurrentItem
        //     change can be canceled.
        //
        // Returns:
        //     true if the event can be canceled; false if the event cannot be canceled.
        public bool IsCancelable { get; }
    }
}

#endif