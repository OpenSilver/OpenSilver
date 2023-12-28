// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides data for the UpDownBase.Parsing event.
    /// </summary>
    /// <typeparam name="T">Type of Value property.</typeparam>
    /// <QualityBand>Stable</QualityBand>
    public class UpDownParsingEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Gets the original string value that will be parsed.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets or sets the value to be used.
        /// </summary>
        /// <value>The parsed value.</value>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// this <see cref="UpDownParsingEventArgs&lt;T&gt;"/> is handled.
        /// </summary>
        /// <value><c>True</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="UpDownParsingEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="text">The text that will be parsed.</param>
        public UpDownParsingEventArgs(string text)
        {
            Text = text;
        }
    }
}
