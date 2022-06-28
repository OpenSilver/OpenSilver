
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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if !MIGRATION
using System;
#endif

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class DependencyObjectCollectionTest
    {
        [TestMethod]
        public void DependencyObjectCollection_IndexOf_ItemNotFound_ReturnsMinusOne()
        {
            DependencyObjectCollection<DependencyObject> dependencyObjectCollection =
                new DependencyObjectCollection<DependencyObject>();

            dependencyObjectCollection.IndexOf(new DependencyObject())
                .Should()
                .Be(-1);
        }

        [TestMethod]
        public void DependencyObjectCollection_IndexOf_NotDependencyObject_Throws()
        {
            DependencyObjectCollection<object> dependencyObjectCollection =
                new DependencyObjectCollection<object>();

            dependencyObjectCollection.Invoking(c => c.IndexOf("Some item"))
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("item is not a DependencyObject.");
        }
    }
}
