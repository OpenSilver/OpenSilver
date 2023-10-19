namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the method that will handle the <see cref="MultiScaleImage.SubImageOpenSucceeded"/>
    /// and the <see cref="MultiScaleImage.SubImageOpenFailed"/> event.
    /// </summary>
    /// <param name="sender">
    /// The object where the event handler is attached.
    /// </param>
    /// <param name="e">
    /// The event data.
    /// </param>
    public delegate void SubImageEventHandler(object sender, SubImageRoutedEventArgs e);
}
