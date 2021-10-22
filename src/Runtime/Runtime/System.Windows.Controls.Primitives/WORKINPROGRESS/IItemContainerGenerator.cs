using System.Windows.Controls;
using System;
using System.Windows;
using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
	public partial interface IItemContainerGenerator
    {
		ItemContainerGenerator GetItemContainerGeneratorForPanel(Panel panel);
		IDisposable StartAt(GeneratorPosition position, GeneratorDirection direction, bool allowStartAtRealizedItem);
		DependencyObject GenerateNext(out bool isNewlyRealized);
		void PrepareItemContainer(DependencyObject container);
		void RemoveAll();
		void Remove(GeneratorPosition position, int count);
		GeneratorPosition GeneratorPositionFromIndex(int itemIndex);
		int IndexFromGeneratorPosition(GeneratorPosition position);
	}

    /// <summary>
    /// A user of the ItemContainerGenerator describes positions using this struct.
    /// Some examples:
    /// To start generating forward from the beginning of the item list,
    /// specify position (-1, 0) and direction Forward.
    /// To start generating backward from the end of the list,
    /// specify position (-1, 0) and direction Backward.
    /// To generate the items after the element with index k, specify
    /// position (k, 0) and direction Forward.
    /// </summary>
    public struct GeneratorPosition
    {
        /// <summary>
        /// Index, with respect to realized elements.  The special value -1
        /// refers to a fictitious element at the beginning or end of the
        /// the list.
        /// </summary>
        public int Index { get { return _index; } set { _index = value; } }

        /// <summary>
        /// Offset, with respect to unrealized items near the indexed element.
        /// An offset of 0 refers to the indexed element itself, an offset
        /// of 1 refers to the next (unrealized) item, and an offset of -1
        /// refers to the previous item.
        /// </summary>
        public int Offset { get { return _offset; } set { _offset = value; } }

        /// <summary> Constructor </summary>
        public GeneratorPosition(int index, int offset)
        {
            _index = index;
            _offset = offset;
        }

        /// <summary> Return a hash code </summary>
        // This is required by FxCop.
        public override int GetHashCode()
        {
            return _index.GetHashCode() + _offset.GetHashCode();
        }

        /// <summary>Returns a string representation of the GeneratorPosition</summary>
        public override string ToString()
        {
            //return string.Concat("GeneratorPosition (", _index.ToString(TypeConverterHelper.InvariantEnglishUS), ",", _offset.ToString(TypeConverterHelper.InvariantEnglishUS), ")");
            return string.Concat("GeneratorPosition (", _index.ToString(), ",", _offset.ToString());
        }


        // The remaining methods are present only because they are required by FxCop.

        /// <summary> Equality test </summary>
        // This is required by FxCop.
        public override bool Equals(object o)
        {
            if (o is GeneratorPosition)
            {
                GeneratorPosition that = (GeneratorPosition)o;
                return this._index == that._index &&
                        this._offset == that._offset;
            }
            return false;
        }

        /// <summary> Equality test </summary>
        // This is required by FxCop.
        public static bool operator ==(GeneratorPosition gp1, GeneratorPosition gp2)
        {
            return gp1._index == gp2._index &&
                    gp1._offset == gp2._offset;
        }

        /// <summary> Inequality test </summary>
        // This is required by FxCop.
        public static bool operator !=(GeneratorPosition gp1, GeneratorPosition gp2)
        {
            return !(gp1 == gp2);
        }

        private int _index;
        private int _offset;
    }

    /// <summary>
    /// This enum is used by the ItemContainerGenerator and its client to specify
    /// the direction in which the generator produces UI.
    /// </summary>
    public enum GeneratorDirection
    {
        /// <summary> generate forward through the item collection </summary>
        Forward,

        /// <summary> generate backward through the item collection </summary>
        Backward
    }

    /// <summary>
    /// This enum is used by the ItemContainerGenerator to indicate its status.
    /// </summary>
    internal enum GeneratorStatus
    {
        ///<summary>The generator has not tried to generate content</summary>
        NotStarted,
        ///<summary>The generator is generating containers</summary>
        GeneratingContainers,
        ///<summary>The generator has finished generating containers</summary>
        ContainersGenerated,
        ///<summary>The generator has finished generating containers, but encountered one or more errors</summary>
        Error
    }
}
