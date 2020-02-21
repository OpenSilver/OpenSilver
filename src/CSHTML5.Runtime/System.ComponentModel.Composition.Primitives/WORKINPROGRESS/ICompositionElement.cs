#if WORKINPROGRESS
namespace System.ComponentModel.Composition.Primitives
{
    //
    // Summary:
    //     Represents an element that participates in composition.
    public partial interface ICompositionElement
    {
        //
        // Summary:
        //     Gets the display name of the composition element.
        //
        // Returns:
        //     The human-readable display name of the System.ComponentModel.Composition.Primitives.ICompositionElement.
        string DisplayName { get; }
        //
        // Summary:
        //     Gets the composition element from which the current composition element originated.
        //
        // Returns:
        //     The composition element from which the current System.ComponentModel.Composition.Primitives.ICompositionElement
        //     originated, or null if the System.ComponentModel.Composition.Primitives.ICompositionElement
        //     is the root composition element.
        ICompositionElement Origin { get; }
    }
}
#endif