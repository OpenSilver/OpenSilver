
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


using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver.Compiler.Common.Helpers;

namespace Compiler.Tests.ResourcesExtractor
{
    [TestClass]
    public class ResourcesExtractorTest
    {
        private const string ExperimentalSubjectName = "Experimental";
        private const string ExperimentalSubjectDll = ExperimentalSubjectName + ".dll";
        
        private static readonly MonoCecilAssemblyStorage Storage = new();
        private static readonly OpenSilver.Compiler.Resources.Helpers.ResourcesExtractor ResourcesExtractor = new();

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            Storage.LoadAssembly(ExperimentalSubjectDll, true);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Storage.Dispose();
        }

        [TestMethod]
        public void GetManifestResources_Should_Include_File_Js()
        {
            var res = ResourcesExtractor.GetManifestResources(Storage, ExperimentalSubjectName,
                new HashSet<string>
                {
                    ".js", ".css", ".png", ".jpg", ".gif"
                });
            res.ContainsKey("Experimental.file.js").Should().BeTrue();
        }
    }
}
