
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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
