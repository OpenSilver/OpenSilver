
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


#if MIGRATION
namespace System.Windows.Markup
#else
namespace Windows.UI.Xaml.Markup
#endif
{
    /// <summary>
    /// Defines a contract for how names of elements should be accessed within a
    /// particular XAML namescope, and how to enforce uniqueness of names within
    /// that XAML namescope.
    /// </summary>
    public interface INameScope
    {
        /// <summary>
        /// Returns an object that has the provided identifying name.
        /// </summary>
        /// <param name="name">The name identifier for the object being requested.</param>
        /// <returns>The object, if found. Returns null if no object of that name was found.</returns>
        object FindName(string name);

        /// <summary>
        /// Registers the provided name into the current XAML namescope.
        /// </summary>
        /// <param name="name">The name to register.</param>
        /// <param name="scopedElement">The specific element that the provided name refers to.</param>
        void RegisterName(string name, object scopedElement);

        /// <summary>
        /// Unregisters the provided name from the current XAML namescope.
        /// </summary>
        /// <param name="name">The name to unregister.</param>
        void UnregisterName(string name);
    }
}
