#if WORKINPROGRESS
using System.Collections.ObjectModel;
using System.ComponentModel;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
	//
	// Summary:
	//     Represents an individual validation error.
	public partial class ValidationSummaryItem : INotifyPropertyChanged
	{
		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.ValidationSummaryItem
		//     class.
		public ValidationSummaryItem()
		{
		}

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.ValidationSummaryItem
		//     class with the specified error message.
		//
		// Parameters:
		//   message:
		//     The text of the error message.
		public ValidationSummaryItem(string message)
		{
		}

		//
		// Summary:
		//     Initializes a new instance of the System.Windows.Controls.ValidationSummaryItem
		//     class with the specified error message, header, item type, source, and context.
		//
		// Parameters:
		//   message:
		//     The text of the error message.
		//
		//   messageHeader:
		//     The header of the item, such as the property name.
		//
		//   itemType:
		//     Specifies whether the error originated from an object or a property.
		//
		//   source:
		//     The source of the error message, including the originating control or property
		//     name.
		//
		//   context:
		//     The context in which the error occurred.
		public ValidationSummaryItem(string message, string messageHeader, ValidationSummaryItemType itemType, ValidationSummaryItemSource source, object context)
		{
		}

		//
		// Summary:
		//     Gets or sets the object that is the context in which the error occurred.
		//
		// Returns:
		//     The object that is the context in which the error occurred, or null if no value
		//     is set.
		public object Context
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets a value that specifies whether the error originated from an object or a
		//     property.
		//
		// Returns:
		//     One of the enumeration values that specifies whether the error originated from
		//     an object or a property.
		public ValidationSummaryItemType ItemType
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets or sets the text of the error message.
		//
		// Returns:
		//     The text of the error message.
		public string Message
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets or sets the text of the error message header.
		//
		// Returns:
		//     The text of the error message header.
		public string MessageHeader
		{
			get;
			set;
		}

		//
		// Summary:
		//     Gets the sources of the validation errors.
		//
		// Returns:
		//     A collection of System.Windows.Controls.ValidationSummaryItemSource objects that
		//     represent the sources of validation errors.
		public ObservableCollection<ValidationSummaryItemSource> Sources
		{
			get;
			private set;
		}

		//
		// Summary:
		//     Occurs when a property value for this System.Windows.Controls.ValidationSummaryItem
		//     changes.
		public event PropertyChangedEventHandler PropertyChanged;
		//
		// Summary:
		//     The string representation of this System.Windows.Controls.ValidationSummaryItem.
		//
		// Returns:
		//     The string representation of this System.Windows.Controls.ValidationSummaryItem.
		public override string ToString()
		{
			return "eizfhgzeruiohgpiseo<fjqrhoeuipgnj<rpiosgb^qeruoijgqirbthtg";
		}
	}
}
#endif