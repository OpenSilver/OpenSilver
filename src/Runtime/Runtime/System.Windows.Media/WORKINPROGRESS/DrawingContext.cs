using System.Windows.Threading;
using OpenSilver;

namespace System.Windows.Media;

/// <summary>
/// Describes visual content using draw, push, and pop commands.
/// </summary>
[NotImplemented]
public abstract partial class DrawingContext : DispatcherObject, IDisposable
{
    #region Constructors
    /// <summary>
    /// Default constructor for DrawingContext - this uses the current Dispatcher.
    /// </summary>
    internal DrawingContext()
    {
        // Nothing to do here
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// Draw Text at the location specified.
    /// </summary>
    /// <param name="formattedText"> The FormattedText to draw. </param>
    /// <param name="origin"> The location at which to draw the text. </param>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    [NotImplemented]
    public void DrawText(FormattedText formattedText, Point origin)
    {
    }

    /// <summary>
    /// Closes the DrawingContext and flushes the content.
    /// Afterwards the DrawingContext can not be used anymore.
    /// This call does not require all Push calls to have been Popped.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    public abstract void Close();

    /// <summary>
    /// This is the same as the Close call:
    /// Closes the DrawingContext and flushes the content.
    /// Afterwards the DrawingContext can not be used anymore.
    /// This call does not require all Push calls to have been Popped.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    void IDisposable.Dispose()
    {
        // Call a virtual method for derived Dispose implementations
        //
        // Attempting to override a explicit interface member implementation causes
        // the most-derived implementation to always be called, and the base
        // implementation becomes uncallable. But FxCop requires the base Dispose
        // method is always be called. To avoid this situation, we use the *Core
        // pattern for derived classes, instead of attempting to override
        // IDisposable.Dispose.

        VerifyAccess();

        DisposeCore();
        GC.SuppressFinalize(this);
    }

    #endregion Public Methods

    #region Protected Methods

    /// <summary>
    /// Dispose functionality implemented by subclasses
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// This call is illegal if this object has already been closed or disposed.
    /// </exception>
    protected abstract void DisposeCore();

    /// <summary>
    /// This verifies that the API can be called for read only access.
    /// </summary>
    protected virtual void VerifyApiNonstructuralChange() => VerifyAccess();

    #endregion Protected Methods
}