

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


using System;

#if MIGRATION
using System.Windows.Controls.Primitives;
#else
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class GenerationState : IDisposable
    {
        public bool AllowStartAtRealizedItem
        {
            get; set;
        }

        public GeneratorDirection Direction
        {
            get; set;
        }

        public GeneratorPosition Position
        {
            get; set;
        }

        public int Step
        {
            get { return Direction == GeneratorDirection.Forward ? 1 : -1; }
        }

        public ItemContainerGenerator Generator
        {
            get; set;
        }

        public GenerationState()
        {

        }

        public void Dispose()
        {
            Generator.GenerationState = null;
        }
    }
}
