
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Runtime.OpenSilver.Tests.Maintenance.MemoryLeak
{
    [TestClass]
    public class MemoryLeakTest
    {
        private void CreateRemoveGrid(GarbageCollectorTracker c)
        {
            var tc = new GridWithTrackingComponent(c);
            var mainWindow = Application.Current.MainWindow;
            mainWindow.Content = tc;
            mainWindow.Content = new Grid();
        }

        [TestMethod]
        public void Grid_Must_Be_Collected()
        {
            var c = new GarbageCollectorTracker();
            CreateRemoveGrid(c);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.IsTrue(c.Collected);
        }
    }
}
