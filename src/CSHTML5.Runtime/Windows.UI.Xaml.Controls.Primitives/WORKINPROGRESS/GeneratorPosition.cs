#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
	//
	// Summary:
	//     System.Windows.Controls.Primitives.GeneratorPosition is used to describe the
	//     position of an item that is managed by System.Windows.Controls.ItemContainerGenerator.
	public partial struct GeneratorPosition
	{
		//
		// Summary:
		//     Initializes a new instance of System.Windows.Controls.Primitives.GeneratorPosition
		//     with the specified index and offset.
		//
		// Parameters:
		//   index:
		//     An System.Int32 index that is relative to the generated (realized) items. -1
		//     is a special value that refers to a fictitious item at the beginning or the end
		//     of the items list.
		//
		//   offset:
		//     An System.Int32 offset that is relative to the ungenerated (unrealized) items
		//     near the indexed item. An offset of 0 refers to the indexed element itself, an
		//     offset 1 refers to the next ungenerated (unrealized) item, and an offset of -1
		//     refers to the previous item.
		public GeneratorPosition(int index, int offset)
		{
			Index = index;
			Offset = offset;
		}

		//
		// Summary:
		//     Gets or sets the System.Int32 index that is relative to the generated (realized)
		//     items.
		//
		// Returns:
		//     An System.Int32 index that is relative to the generated (realized) items.
		public int Index { get; set; }
		//
		// Summary:
		//     Gets or sets the System.Int32 offset that is relative to the ungenerated (unrealized)
		//     items near the indexed item.
		//
		// Returns:
		//     An System.Int32 offset that is relative to the ungenerated (unrealized) items
		//     near the indexed item.
		public int Offset { get; set; }

		//
		// Summary:
		//     Compares the specified instance and the current instance of System.Windows.Controls.Primitives.GeneratorPosition
		//     for value equality.
		//
		// Parameters:
		//   o:
		//     The System.Windows.Controls.Primitives.GeneratorPosition instance to compare.
		//
		// Returns:
		//     true if o and this instance of System.Windows.Controls.Primitives.GeneratorPosition
		//     have the same values.
		public override bool Equals(object o)
		{
			return false;
		}
		//
		// Summary:
		//     Returns the hash code for this System.Windows.Controls.Primitives.GeneratorPosition.
		//
		// Returns:
		//     The hash code for this System.Windows.Controls.Primitives.GeneratorPosition.
		public override int GetHashCode()
		{
			return 0;
		}
		//
		// Summary:
		//     Returns a string representation of this instance of System.Windows.Controls.Primitives.GeneratorPosition.
		//
		// Returns:
		//     A string representation of this instance of System.Windows.Controls.Primitives.GeneratorPosition.
		public override string ToString()
		{
			return null;
		}

		//
		// Summary:
		//     Compares two System.Windows.Controls.Primitives.GeneratorPosition objects for
		//     value equality.
		//
		// Parameters:
		//   gp1:
		//     The first instance to compare.
		//
		//   gp2:
		//     The second instance to compare.
		//
		// Returns:
		//     true if the two objects are equal; otherwise, false.
		public static bool operator ==(GeneratorPosition gp1, GeneratorPosition gp2)
		{
			return false;
		}
		//
		// Summary:
		//     Compares two System.Windows.Controls.Primitives.GeneratorPosition objects for
		//     value inequality.
		//
		// Parameters:
		//   gp1:
		//     The first instance to compare.
		//
		//   gp2:
		//     The second instance to compare.
		//
		// Returns:
		//     true if the values are not equal; otherwise, false.
		public static bool operator !=(GeneratorPosition gp1, GeneratorPosition gp2)
		{
			return !(gp1 == gp2);
		}
	}
}
#endif