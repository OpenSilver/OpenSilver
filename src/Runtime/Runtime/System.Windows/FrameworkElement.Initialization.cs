
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

using OpenSilver.Internal;

namespace System.Windows;

public partial class FrameworkElement
{
    /// <summary>
    /// Gets a value that indicates whether this element has been initialized, either during processing by a XAML 
    /// processor, or by explicitly having its <see cref="EndInit"/> method called.
    /// </summary>
    /// <returns>
    /// true if the element is initialized per the aforementioned XAML processing or method calls; otherwise, false.
    /// </returns>
    public bool IsInitialized => ReadInternalFlag(InternalFlags.IsInitialized);

    /// <summary>
    /// Starts the initialization process for this element.
    /// </summary>
    public virtual void BeginInit()
    {
        // Nested BeginInits on the same instance aren't permitted
        if (ReadInternalFlag(InternalFlags.InitPending))
        {
            throw new InvalidOperationException(Strings.NestedBeginInitNotSupported);
        }

        // Mark the element as pending initialization
        WriteInternalFlag(InternalFlags.InitPending, true);
    }

    /// <summary>
    /// Indicates that the initialization process for the element is complete.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <see cref="EndInit"/> was called without <see cref="BeginInit"/> having previously been called on the element.
    /// </exception>
    public virtual void EndInit()
    {
        // Every EndInit must be preceeded by a BeginInit
        if (!ReadInternalFlag(InternalFlags.InitPending))
        {
            throw new InvalidOperationException(Strings.EndInitWithoutBeginInitNotSupported);
        }

        // Reset the pending flag
        WriteInternalFlag(InternalFlags.InitPending, false);

        // Mark the element initialized and fire Initialized event
        // (eg. tree building via parser)
        TryFireInitialized();
    }

    /// <summary>
    /// Occurs when this <see cref="FrameworkElement"/> is initialized. This event coincides with cases where the value 
    /// of the <see cref="IsInitialized"/> property changes from false (or undefined) to true.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// Raises the <see cref="Initialized"/> event. This method is invoked whenever <see cref="IsInitialized"/> is set 
    /// to true internally.
    /// </summary>
    /// <param name="e">
    /// The <see cref="RoutedEventArgs"/> that contains the event data.
    /// </param>
    protected virtual void OnInitialized(EventArgs e)
    {
        // Need to update the ThemeStyleProperty so that we can pickup
        // the implicit style if it hasn't already been fetched
        if (!HasThemeStyleEverBeenFetched)
        {
            UpdateThemeStyleProperty();
        }

        Initialized?.Invoke(this, e);
    }

    // Helper method that tries to set IsInitialized to true
    // and Fire the Initialized event
    // This method can be invoked from two locations
    //      1> EndInit
    //      2> OnParentChanged
    private void TryFireInitialized()
    {
        if (!ReadInternalFlag(InternalFlags.InitPending) && !ReadInternalFlag(InternalFlags.IsInitialized))
        {
            WriteInternalFlag(InternalFlags.IsInitialized, true);

            // Do instance initialization outside of the OnInitialized virtual
            // to make sure that:
            // 1) We avoid attaching instance handlers to FrameworkElement
            //    (instance handlers are expensive).
            // 2) If a derived class forgets to call base OnInitialized,
            //    this work will still happen.
            PrivateInitialized();

            OnInitialized(EventArgs.Empty);
        }
    }

    // This should be called when the FrameworkElement tree is built up,
    // at this point we can process all the setter-related information
    // because now we'll be able to resolve "Target" references in setters.
    private void PrivateInitialized()
    {
        // Process Trigger information when this object is loaded.
        EventTrigger.ProcessTriggerCollection(this);
    }
}
