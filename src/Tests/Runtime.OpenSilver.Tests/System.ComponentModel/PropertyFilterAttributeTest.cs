
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

namespace System.ComponentModel.Tests;

[TestClass]
public class PropertyFilterAttributeTests
{
    [DataTestMethod]
    [DataRow(PropertyFilterOptions.All)]
    [DataRow(PropertyFilterOptions.SetValues)]
    [DataRow(PropertyFilterOptions.Invalid)]
    [DataRow(PropertyFilterOptions.None)]
    [DataRow(PropertyFilterOptions.None - 1)]
    public void Ctor_PropertyFilterOptions(PropertyFilterOptions filter)
    {
        var attribute = new PropertyFilterAttribute(filter);
        Assert.AreEqual(filter, attribute.Filter);
    }

    [TestMethod]
    public void Default_Get_ReturnsExpected()
    {
        PropertyFilterAttribute attribute = PropertyFilterAttribute.Default;
        Assert.AreEqual(PropertyFilterOptions.All, attribute.Filter);
        Assert.AreSame(attribute, PropertyFilterAttribute.Default);
    }

    public static IEnumerable<object[]> Equals_TestData()
    {
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), new PropertyFilterAttribute(PropertyFilterOptions.None), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), new PropertyFilterAttribute(PropertyFilterOptions.All), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), new PropertyFilterAttribute(PropertyFilterOptions.SetValues), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.SetValues), new PropertyFilterAttribute(PropertyFilterOptions.SetValues), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.SetValues), new PropertyFilterAttribute(PropertyFilterOptions.All), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.SetValues), new PropertyFilterAttribute(PropertyFilterOptions.None), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.All), new PropertyFilterAttribute(PropertyFilterOptions.All), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.All), new PropertyFilterAttribute(PropertyFilterOptions.None), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.All), new PropertyFilterAttribute(PropertyFilterOptions.SetValues), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), new object(), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), null, false };
    }

    [DataTestMethod]
    [DynamicData(nameof(Equals_TestData), DynamicDataSourceType.Method)]
    public void Equals_Object_ReturnsExpected(PropertyFilterAttribute attribute, object other, bool expected)
    {
        Assert.AreEqual(expected, attribute.Equals(other));
        if (other is PropertyFilterAttribute)
        {
            Assert.AreEqual(expected, attribute.GetHashCode().Equals(other.GetHashCode()));
        }
    }

    public static IEnumerable<object[]> Match_TestData()
    {
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), new PropertyFilterAttribute(PropertyFilterOptions.None), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), new PropertyFilterAttribute(PropertyFilterOptions.All), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), new PropertyFilterAttribute(PropertyFilterOptions.SetValues), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.All), new PropertyFilterAttribute(PropertyFilterOptions.All), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.All), new PropertyFilterAttribute(PropertyFilterOptions.None), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.All), new PropertyFilterAttribute(PropertyFilterOptions.SetValues), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.SetValues), new PropertyFilterAttribute(PropertyFilterOptions.SetValues), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.SetValues), new PropertyFilterAttribute(PropertyFilterOptions.All), true };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.SetValues), new PropertyFilterAttribute(PropertyFilterOptions.None), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), new object(), false };
        yield return new object[] { new PropertyFilterAttribute(PropertyFilterOptions.None), null, false };
    }

    [DataTestMethod]
    [DynamicData(nameof(Match_TestData), DynamicDataSourceType.Method)]
    public void Match_Object_ReturnsExpected(PropertyFilterAttribute attribute, object value, bool expected)
    {
        Assert.AreEqual(expected, attribute.Match(value));
    }
}