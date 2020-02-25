#if WORKINPROGRESS

namespace System.ComponentModel
{
    //
    // Summary:
    //     Specifies that this object supports a simple, transacted notification for batch
    //     initialization.
    public interface ISupportInitialize
    {
        //
        // Summary:
        //     Signals the object that initialization is starting.
        void BeginInit();
        //
        // Summary:
        //     Signals the object that initialization is complete.
        void EndInit();
    }
}

#endif