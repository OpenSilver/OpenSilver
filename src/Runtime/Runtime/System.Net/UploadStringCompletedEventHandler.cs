
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

namespace OpenSilver.Compatibility
{
    /// <summary>
    /// Represents the method that will handle the System.Net.WebClient.UploadStringCompleted
    /// event of a System.Net.WebClient.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A System.Net.UploadStringCompletedEventArgs containing event data.</param>
    public delegate void UploadStringCompletedEventHandler(object sender, UploadStringCompletedEventArgs e);
}
