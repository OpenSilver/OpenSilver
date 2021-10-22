
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
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenSilver.Tests
{
    public abstract class TypeConverterTestBase
    {
        protected abstract TypeConverter Converter { get; }

        [TestInitialize]
        public void Initialize()
        {
            if (Converter == null)
            {
                throw new InvalidOperationException(
                    $"'{nameof(Converter)}' must be initialized to a non null value."
                );
            }
        }
    }
}
