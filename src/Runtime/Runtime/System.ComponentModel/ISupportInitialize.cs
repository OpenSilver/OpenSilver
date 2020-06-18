namespace System.ComponentModel
{
    /// <summary>
    /// Specifies that this object supports a simple, transacted notification for batch
    /// initialization.
    /// </summary>
    public interface ISupportInitialize
    {
        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        void BeginInit();

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        void EndInit();
    }
}