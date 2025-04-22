
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

using System.Globalization;

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
        /// Prepares the generator to generate items, starting at the specified <see cref="GeneratorPosition"/>, and in 
        /// the specified <see cref="GeneratorDirection"/>.
        /// </summary>
        /// <param name="position">
        /// A <see cref="GeneratorPosition"/>, that specifies the position of the item to start generating items at.
        /// </param>
        /// <param name="direction">
        /// A <see cref="GeneratorDirection"/> that specifies the direction which to generate items.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> object that tracks the lifetime of the generation process.
        /// </returns>
        IDisposable StartAt(GeneratorPosition position, GeneratorDirection direction);

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
        /// Returns the container element used to display the next item.
        /// </summary>
        /// <returns>
        /// A <see cref="DependencyObject"/> that is the container element which is used to display the next item.
        /// </returns>
        DependencyObject GenerateNext();

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
    /// <see cref="GeneratorPosition"/> is used to describe the position of an item that is managed by 
    /// <see cref="ItemContainerGenerator"/>.
    /// </summary>
    public struct GeneratorPosition
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GeneratorPosition"/> with the specified index and offset.
        /// </summary>
        /// <param name="index">
        /// An <see cref="int"/> index that is relative to the generated (realized) items. -1 is a special value that 
        /// refers to a fictitious item at the beginning or the end of the items list.
        /// </param>
        /// <param name="offset">
        /// An <see cref="int"/> offset that is relative to the ungenerated (unrealized) items near the indexed item. 
        /// An offset of 0 refers to the indexed element itself, an offset 1 refers to the next ungenerated (unrealized) 
        /// item, and an offset of -1 refers to the previous item.
        /// </param>
        public GeneratorPosition(int index, int offset)
        {
            Index = index;
            Offset = offset;
        }

        /// <summary>
        /// Gets or sets the <see cref="int"/> index that is relative to the generated (realized) items.
        /// </summary>
        /// <returns>
        /// An <see cref="int"/> index that is relative to the generated (realized) items.
        /// </returns>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="int"/> offset that is relative to the ungenerated (unrealized) items near the indexed item.
        /// </summary>
        /// <returns>
        /// An <see cref="int"/> offset that is relative to the ungenerated (unrealized) items near the indexed item.
        /// </returns>
        public int Offset { get; set; }

        /// <summary>
        /// Returns the hash code for this <see cref="GeneratorPosition"/>.
        /// </summary>
        /// <returns>
        /// The hash code for this <see cref="GeneratorPosition"/>.
        /// </returns>
        public override int GetHashCode() => Index.GetHashCode() + Offset.GetHashCode();

        /// <summary>
        /// Returns a string representation of this instance of <see cref="GeneratorPosition"/>.
        /// </summary>
        /// <returns>
        /// A string representation of this instance of <see cref="GeneratorPosition"/>.
        /// </returns>
        public override string ToString() =>
            string.Concat("GeneratorPosition (", Index.ToString(CultureInfo.InvariantCulture), ",", Offset.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Compares the specified instance and the current instance of <see cref="GeneratorPosition"/> for value equality.
        /// </summary>
        /// <param name="o">
        /// The <see cref="GeneratorPosition"/> instance to compare.
        /// </param>
        /// <returns>
        /// true if o and this instance of <see cref="GeneratorPosition"/> have the same values.
        /// </returns>
        public override bool Equals(object o) => o is GeneratorPosition that && this == that;

        /// <summary>
        /// Compares two <see cref="GeneratorPosition"/> objects for value equality.
        /// </summary>
        /// <param name="gp1">
        /// The first instance to compare.
        /// </param>
        /// <param name="gp2">
        /// The second instance to compare.
        /// </param>
        /// <returns>
        /// true if the two objects are equal; otherwise, false.
        /// </returns>
        public static bool operator ==(GeneratorPosition gp1, GeneratorPosition gp2) => gp1.Index == gp2.Index && gp1.Offset == gp2.Offset;

        /// <summary>
        /// Compares two <see cref="GeneratorPosition"/> objects for value inequality.
        /// </summary>
        /// <param name="gp1">
        /// The first instance to compare.
        /// </param>
        /// <param name="gp2">
        /// The second instance to compare.
        /// </param>
        /// <returns>
        /// true if the values are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(GeneratorPosition gp1, GeneratorPosition gp2) => !(gp1 == gp2);
    }

    /// <summary>
    /// Specifies the direction in which item generation will occur. <see cref="GeneratorDirection"/>
    /// is used by <see cref="IItemContainerGenerator.StartAt(GeneratorPosition, GeneratorDirection)"/> and
    /// <see cref="IItemContainerGenerator.StartAt(GeneratorPosition, GeneratorDirection, bool)"/>.
    /// </summary>
    public enum GeneratorDirection
    {
        /// <summary>
        /// Specifies to generate items in a forward direction.
        /// </summary>
        Forward,

        /// <summary>
        /// Specifies to generate items in a backward direction.
        /// </summary>
        Backward,
    }

    /// <summary>
    /// Used by <see cref="ItemContainerGenerator"/> to indicate the status of its item generation.
    /// </summary>
    public enum GeneratorStatus
    {
        /// <summary>
        /// The generator has not tried to generate content.
        /// </summary>
        NotStarted,

        /// <summary>
        ///  The generator is generating containers.
        /// </summary>
        GeneratingContainers,

        /// <summary>
        /// The generator has finished generating containers.
        /// </summary>
        ContainersGenerated,

        /// <summary>
        /// The generator has finished generating containers, but encountered one or more errors.
        /// </summary>
        Error,
    }
}
