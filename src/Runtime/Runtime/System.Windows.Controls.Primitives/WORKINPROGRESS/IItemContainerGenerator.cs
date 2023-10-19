
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

namespace System.Windows.Controls.Primitives
{
    /// <summary>
    /// An interface that is implemented by classes which are responsible for generating
    /// UI content on behalf of a host.
    /// </summary>
    public interface IItemContainerGenerator
    {
        /// <summary>
        /// Returns the <see cref="ItemContainerGenerator"/> appropriate for use
        /// by the specified panel.
        /// </summary>
        /// <param name="panel">
        /// The panel for which to return an appropriate <see cref="ItemContainerGenerator"/>.
        /// </param>
        /// <returns>
        /// An <see cref="ItemContainerGenerator"/> appropriate for use by the
        /// specified panel.
        /// </returns>
        ItemContainerGenerator GetItemContainerGeneratorForPanel(Panel panel);

        /// <summary>
        /// Prepares the generator to generate items, starting at the specified <see cref="GeneratorPosition"/>,
        /// and in the specified <see cref="GeneratorDirection"/>, and
        /// controlling whether or not to start at a generated (realized) item.
        /// </summary>
        /// <param name="position">
        /// A <see cref="GeneratorPosition"/>, that specifies the position
        /// of the item to start generating items at.
        /// </param>
        /// <param name="direction">
        /// Specifies the position of the item to start generating items at.
        /// </param>
        /// <param name="allowStartAtRealizedItem">
        /// A <see cref="bool"/> that specifies whether to start at a generated (realized) item.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> object that tracks the lifetime of the generation process.
        /// </returns>
        IDisposable StartAt(GeneratorPosition position, GeneratorDirection direction, bool allowStartAtRealizedItem);

        /// <summary>
        /// Returns the container element used to display the next item, and whether the
        /// container element has been newly generated (realized).
        /// </summary>
        /// <param name="isNewlyRealized">
        /// Is true if the returned <see cref="DependencyObject"/> is newly generated (realized);
        /// otherwise, false.
        /// </param>
        /// <returns>
        /// A <see cref="DependencyObject"/> that is the container element which is used
        /// to display the next item.
        /// </returns>
        DependencyObject GenerateNext(out bool isNewlyRealized);

        /// <summary>
        /// Prepares the specified element as the container for the corresponding item.
        /// </summary>
        /// <param name="container">
        /// The container to prepare. Normally, container is the result of the previous call
        /// to <see cref="GenerateNext(out bool)"/>.
        /// </param>
        void PrepareItemContainer(DependencyObject container);

        /// <summary>
        /// Removes all generated (realized) items.
        /// </summary>
		void RemoveAll();

        /// <summary>
        /// Removes one or more generated (realized) items.
        /// </summary>
        /// <param name="position">
        /// The <see cref="int"/> index of the element to remove. position must refer to a previously
        /// generated (realized) item, which means its offset must be zero.
        /// </param>
        /// <param name="count">
        /// The <see cref="int"/> number of elements to remove, starting at position.
        /// </param>
        void Remove(GeneratorPosition position, int count);

        /// <summary>
        /// Returns the <see cref="GeneratorPosition"/> object that
        /// maps to the item at the specified index.
        /// </summary>
        /// <param name="itemIndex">
        /// The index of desired item.
        /// </param>
        /// <returns>
        /// An <see cref="GeneratorPosition"/> object that maps to the
        /// item at the specified index.
        /// </returns>
        GeneratorPosition GeneratorPositionFromIndex(int itemIndex);

        /// <summary>
        /// Returns the index that maps to the specified <see cref="GeneratorPosition"/>.
        /// </summary>
        /// <param name="position">
        /// The index of desired item. The <see cref="GeneratorPosition"/>
        /// for the desired index.
        /// </param>
        /// <returns>
        /// An <see cref="int"/> that is the index which maps to the specified <see cref="GeneratorPosition"/>.
        /// </returns>
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
