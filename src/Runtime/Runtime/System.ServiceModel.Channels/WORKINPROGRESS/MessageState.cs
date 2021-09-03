namespace System.ServiceModel.Channels
{
    //
    // Summary:
    //     Specifies the status of a message.
    public enum MessageState
    {
        //
        // Summary:
        //     The message has been created.
        Created = 0,
        //
        // Summary:
        //     The message is being read.
        Read = 1,
        //
        // Summary:
        //     The message has been written.
        Written = 2,
        //
        // Summary:
        //     The message has been copied.
        Copied = 3,
        //
        // Summary:
        //     The message has been closed and can no longer be accessed.
        Closed = 4
    }
}
