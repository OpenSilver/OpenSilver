
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

namespace CSHTML5.Internal.Attributes
{
    /// <summary>
    /// Specifies whether the project was compiled in the "SLMigration" mode.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class CompilerIsSLMigrationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the CompilerIsSLMigrationAttribute class with a
        /// boolean specifying whether the project was compiled in the "SLMigration" mode.
        /// </summary>
        /// <param name="isSLMigration">A boolean that is true if the project compiled in the "SLMigration" mode, and false otherwise.</param>
        public CompilerIsSLMigrationAttribute(bool isSLMigration)
        {
            IsSLMigration = isSLMigration;
        }

        /// <summary>
        /// Gets a boolean that is true if the project compiled in the "SLMigration" mode, and false otherwise.
        /// </summary>
        public bool IsSLMigration { get; private set; }
    }
}
