
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

namespace System.Windows.Input
{
	/// <summary>
	/// Provides data for the <see cref="UIElement.TextInput"/> routed event.
	/// </summary>
	public sealed class TextCompositionEventArgs : RoutedEventArgs
	{
		internal TextCompositionEventArgs() { }

        internal override void InvokeHandler(Delegate handler, object target)
        {
			((TextCompositionEventHandler)handler)(target, this);
        }

        /// <summary>
        /// Gets or sets a value that marks the routed event as handled, and prevents most
        /// handlers along the event route from handling the same event again.
        /// </summary>
        /// <returns>
        /// true to mark the routed event handled. false to leave the routed event unhandled,
        /// which permits the event to potentially route further and be acted on by other
        /// handlers. The default is false.
        /// </returns>
        public bool Handled
        {
			get => HandledImpl;
			set => HandledImpl = value;
		}

		/// <summary>
		/// Gets or sets the text string that of the text composition.
		/// </summary>
		/// <returns>
		/// The text string of the text composition.
		/// </returns>
		public string Text { get; internal set; }

		/// <summary>
		/// Gets or sets the text in the composition as a <see cref="Input.TextComposition"/>
		/// object.
		/// </summary>
		/// <returns>
		/// The text in the composition, as a <see cref="Input.TextComposition"/> object.
		/// </returns>
		public TextComposition TextComposition { get; internal set; }

        internal bool PreventDefault { get; set; }
    }
}
