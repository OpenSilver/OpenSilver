
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
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Runtime.OpenSilver.Tests;

namespace System.Windows.Tests;

[TestClass]
public class WindowTest
{
    private const double ViewportWidth = 800;
    private const double ViewportHeight = 600;

    private sealed class MeasureRecorder : FrameworkElement
    {
        public List<Size> Constraints { get; } = new();

        protected override Size MeasureOverride(Size availableSize)
        {
            Constraints.Add(availableSize);
            return new Size();
        }
    }

    [TestMethod]
    public void Main_Window_Shown_Maximized_Should_Only_Measure_Its_Content_With_The_Viewport_Size()
    {
        Application app = Application.Current;
        Window oldMainWindow = app.MainWindow;
        var content = new MeasureRecorder();
        var window = new Window { WindowState = WindowState.Maximized, Content = content };

        TestSetup.ExecuteJavascript += OnExecuteJavascript;
        try
        {
            app.MainWindow = window;

            AssertOnlyFiniteConstraints(content);
        }
        finally
        {
            TestSetup.ExecuteJavascript -= OnExecuteJavascript;
            app.MainWindow = oldMainWindow;
            window.Close();
        }
    }

    [TestMethod]
    public void Main_Window_Shown_Full_Screen_Should_Only_Measure_Its_Content_With_The_Viewport_Size()
    {
        Application app = Application.Current;
        Window oldMainWindow = app.MainWindow;
        List<Window> otherWindows = new();
        foreach (Window w in app.Windows)
        {
            otherWindows.Add(w);
        }
        // The main window is shown full screen only when it is the only window.
        otherWindows.ForEach(app.Windows.Remove);

        var content = new MeasureRecorder();
        var window = new Window { Content = content };
        WindowState? stateWhenLoaded = null;
        content.Loaded += (o, e) => stateWhenLoaded = window.WindowState;

        TestSetup.ExecuteJavascript += OnExecuteJavascript;
        try
        {
            app.MainWindow = window;

            Assert.AreEqual(WindowState.Maximized, window.WindowState);
            Assert.AreEqual(WindowState.Normal, stateWhenLoaded, "Loaded must be raised before the window is shown full screen.");
            AssertOnlyFiniteConstraints(content);
        }
        finally
        {
            TestSetup.ExecuteJavascript -= OnExecuteJavascript;
            otherWindows.ForEach(app.Windows.Add);
            app.MainWindow = oldMainWindow;
            window.Close();
        }
    }

    private static void AssertOnlyFiniteConstraints(MeasureRecorder content)
    {
        Assert.AreNotEqual(0, content.Constraints.Count);
        foreach (Size constraint in content.Constraints)
        {
            Assert.IsFalse(double.IsInfinity(constraint.Width) || double.IsInfinity(constraint.Height), $"Measured with {constraint}.");
        }
    }

    private static void OnExecuteJavascript(object sender, ExecuteJavascriptEventArgs e)
    {
        Match match = Regex.Match(e.Javascript, @"osjs\.getProp\('[^']*', 'offset(Width|Height)'\)");
        if (match.Success)
        {
            e.Result = match.Groups[1].Value == "Width" ? ViewportWidth : ViewportHeight;
            e.Handled = true;
        }
    }
}
