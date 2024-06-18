
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

using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSHTML5.Internal.Tests;

[TestClass]
public class INTERNAL_UriHelperTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        StartupAssemblyInfo.OutputResourcesPath = @"resources\";
    }

    [TestMethod]
    public void ComponentURI()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("/AssemblyName;component/Folder1/Folder2/file.txt");

        Assert.AreEqual("resources/assemblyname/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void ComponentURI_With_BlackSlashes()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path(@"/AssemblyName;component/Folder1/Folder2\file.txt");

        Assert.AreEqual("resources/assemblyname/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void MsAppxURI()
    {
        string uri1 = INTERNAL_UriHelper.ConvertToHtml5Path("ms-appx:/Folder1/Folder2/file.txt");
        string uri2 = INTERNAL_UriHelper.ConvertToHtml5Path("ms-appx://Folder1/Folder2/file.txt");
        string uri3 = INTERNAL_UriHelper.ConvertToHtml5Path("ms-appx:///Folder1/Folder2/file.txt");

        Assert.AreEqual($"resources/{Application.Current.GetType().Assembly.GetName().Name.ToLowerInvariant()}/folder1/folder2/file.txt", uri1);
        Assert.AreEqual($"resources/{Application.Current.GetType().Assembly.GetName().Name.ToLowerInvariant()}/folder1/folder2/file.txt", uri2);
        Assert.AreEqual($"resources/{Application.Current.GetType().Assembly.GetName().Name.ToLowerInvariant()}/folder1/folder2/file.txt", uri3);
    }

    [TestMethod]
    public void MsAppxURI_With_BackSlashes()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path(@"ms-appx:///Folder1/Folder2\file.txt");

        Assert.AreEqual($"resources/{Application.Current.GetType().Assembly.GetName().Name.ToLowerInvariant()}/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void MsAppxURI_With_AssemblyName()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("ms-appx:/OpenSilver/Folder1/Folder2/file.txt");

        Assert.AreEqual("resources/opensilver/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void MsAppxURI_With_AssemblyName_Case_Insensitive()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("ms-appx:/opensilver/Folder1/Folder2/file.txt");

        Assert.AreEqual("resources/opensilver/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void PackApplicationURI()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("pack://application:,,,/Folder1/Folder2/file.txt");

        Assert.AreEqual($"resources/{Application.Current.GetType().Assembly.GetName().Name.ToLowerInvariant()}/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void PackApplicationURI_With_BackSlashes()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path(@"pack://application:,,,/Folder1\Folder2/file.txt");

        Assert.AreEqual($"resources/{Application.Current.GetType().Assembly.GetName().Name.ToLowerInvariant()}/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void PackApplicationURI_With_AssemblyName()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path(@"pack://application:,,,/OpenSilver/Folder1\Folder2/file.txt");

        Assert.AreEqual("resources/opensilver/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void PackApplicationURI_With_AssemblyName_Case_Insensitive()
    {
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path(@"pack://application:,,,/openSILVER/Folder1\Folder2/file.txt");

        Assert.AreEqual("resources/opensilver/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void RelativeURI()
    {
        UIElement relativeTo = new TextBlock { XamlSourcePath = @"MyAssemblyName\Path1\Path2\FileName.xaml" };
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("Folder1/Folder2/file.txt", relativeTo);

        Assert.AreEqual("resources/myassemblyname/path1/path2/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void RelativeURI_With_Rooted_URI()
    {
        UIElement relativeTo = new TextBlock { XamlSourcePath = @"MyAssemblyName\Path1\Path2\FileName.xaml" };
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("/Folder1/Folder2/file.txt", relativeTo);

        Assert.AreEqual("resources/myassemblyname/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void RelativeURI_With_Collapse_Separator_1()
    {
        UIElement relativeTo = new TextBlock { XamlSourcePath = @"MyAssemblyName\Path1\Path2\FileName.xaml" };
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("../Folder1/Folder2/file.txt", relativeTo);

        Assert.AreEqual("resources/myassemblyname/path1/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void RelativeURI_With_Collapse_Separator_2()
    {
        UIElement relativeTo = new TextBlock { XamlSourcePath = @"MyAssemblyName\Path1\Path2\FileName.xaml" };
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("../../../Folder1/Folder2/file.txt", relativeTo);

        Assert.AreEqual("resources/myassemblyname/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void RelativeURI_With_Collapse_Separator_3()
    {
        UIElement relativeTo = new TextBlock { XamlSourcePath = @"MyAssemblyName\Path1\Path2\FileName.xaml" };
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path(@"../..\Folder1/Folder2/file.txt", relativeTo);

        Assert.AreEqual("resources/myassemblyname/folder1/folder2/file.txt", uri);
    }

    [TestMethod]
    public void RelativeURI_With_Collapse_Separator_4()
    {
        UIElement relativeTo = new TextBlock { XamlSourcePath = @"MyAssemblyName\Path1\Path2\FileName.xaml" };
        string uri = INTERNAL_UriHelper.ConvertToHtml5Path("../Folder1/../Folder2/file.txt", relativeTo);

        Assert.AreEqual("resources/myassemblyname/path1/folder2/file.txt", uri);
    }
}
