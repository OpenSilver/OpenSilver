
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

using System.IO;
using System.Reflection;
using System.Security;

namespace System.Windows
{
	[OpenSilver.NotImplemented]
    public sealed partial class AssemblyPart : DependencyObject
    {
        //
        // Summary:
        //     Identifies the System.Windows.AssemblyPart.Source dependency property.
        //
        // Returns:
        //     The identifier for the System.Windows.AssemblyPart.Source dependency property.
		[OpenSilver.NotImplemented]
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(AssemblyPart), null);

        //
        // Summary:
        //     Initializes a new instance of the System.Windows.AssemblyPart class.
		[OpenSilver.NotImplemented]
        public AssemblyPart()
        {

        }

        //
        // Summary:
        //     Gets the System.Uri that identifies an assembly as an assembly part.
        //
        // Returns:
        //     A System.String that is the assembly, which is identified as an assembly part.
		[OpenSilver.NotImplemented]
        public string Source
        {
            get { return (string)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        //
        // Summary:
        //     Converts a System.IO.Stream to an System.Reflection.Assembly that is subsequently
        //     loaded into the current application domain.
        //
        // Parameters:
        //   assemblyStream:
        //     The System.IO.Stream to load into the current application domain.
        //
        // Returns:
        //     The System.Reflection.Assembly that is subsequently loaded into the current application
        //     domain.
        [SecuritySafeCritical]
		[OpenSilver.NotImplemented]
        public Assembly Load(Stream assemblyStream)
        {
            return null;
        }
    }
}
