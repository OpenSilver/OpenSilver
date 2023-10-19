// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace System.Windows.Controls
{
    /// <summary>
    /// Provides data for the UpDownBase.ParseError event.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    public class UpDownParseErrorEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the original string value that failed to parse.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the exception that was raised during the initial parsing 
        /// attempt.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event is handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Initializes a new instance of the UpDownParseErrorEventArgs class.
        /// </summary>
        /// <param name="text">The text that caused the parsing error.</param>
        /// <param name="error">The exception thrown by ParseValue method.</param>
        public UpDownParseErrorEventArgs(string text, Exception error)
            : base()
        {
            Text = text;
            Error = error;
        }
    }
}