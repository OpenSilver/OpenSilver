
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace System.Windows.Tests;

[TestClass]
public class AttachedPropertyBrowsableForTypeAttributeTests
{
    // TODO:
    // - IsBrowsable
    // - UnionResults

    [TestMethod]
    public void Ctor_Type()
    {
        var attribute = new AttachedPropertyBrowsableForTypeAttribute(typeof(string));
        Assert.AreEqual(typeof(string), attribute.TargetType);
        Assert.AreSame(attribute, attribute.TypeId);
    }

    [TestMethod]
    public void Ctor_NullTargetType_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new AttachedPropertyBrowsableForTypeAttribute(null));
        Assert.AreEqual("targetType", ex.ParamName);
    }

    public static IEnumerable<object[]> Equals_TestData()
    {
        var attribute = new AttachedPropertyBrowsableForTypeAttribute(typeof(string));
        yield return new object[] { attribute, attribute, true };
        yield return new object[] { attribute, new AttachedPropertyBrowsableForTypeAttribute(typeof(string)), true };
        yield return new object[] { attribute, new AttachedPropertyBrowsableForTypeAttribute(typeof(int)), false };
        yield return new object[] { attribute, new object(), false };
        yield return new object[] { attribute, null, false };
    }

    [TestMethod]
    [DynamicData(nameof(Equals_TestData))]
    public void Equals_Object_ReturnsExpected(AttachedPropertyBrowsableForTypeAttribute attribute, object obj, bool expected)
    {
        Assert.AreEqual(expected, attribute.Equals(obj));
        if (obj is AttachedPropertyBrowsableForTypeAttribute otherAttribute)
        {
            Assert.AreEqual(expected, otherAttribute.Equals(attribute));
            Assert.AreEqual(expected, attribute.GetHashCode().Equals(obj.GetHashCode()));
        }
    }

    [TestMethod]
    public void GetHashCode_Invoke_ReturnsEqual()
    {
        var attribute = new AttachedPropertyBrowsableForTypeAttribute(typeof(string));
        Assert.AreNotEqual(0, attribute.GetHashCode());
        Assert.AreEqual(attribute.GetHashCode(), attribute.GetHashCode());
    }
}