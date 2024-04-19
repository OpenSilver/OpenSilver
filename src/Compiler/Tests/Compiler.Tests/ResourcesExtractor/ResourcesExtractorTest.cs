
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


using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver.Compiler.Common.Helpers;
using OpenSilver.Compiler.Resources;

namespace Compiler.Tests.ResourcesExtractor
{
    [TestClass]
    public class ResourcesExtractorTest
    {
        private const string ExperimentalSubjectName = "Experimental";
        private const string ExperimentalSubjectDll = ExperimentalSubjectName + ".dll";
        
        private static readonly MonoCecilAssemblyStorage Storage = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            Storage.LoadAssembly(ExperimentalSubjectDll);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Storage.Dispose();
        }

        [TestMethod]
        public void GetManifestResources_Should_Include_File_Js()
        {
            var resources = ResourcesExtractorAndCopier.GetManifestResources(Storage.Assemblies[0]);
            resources.Any(r => r.Name == "Experimental.file.js").Should().BeTrue();
        }
    }
}
