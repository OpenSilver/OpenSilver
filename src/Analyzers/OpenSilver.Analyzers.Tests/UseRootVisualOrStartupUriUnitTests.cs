
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

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.Testing;
using AnalyzerVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
    OpenSilver.CodeAnalysis.UseRootVisualOrStartupUriAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using CodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    OpenSilver.CodeAnalysis.UseRootVisualOrStartupUriAnalyzer,
    OpenSilver.CodeAnalysis.UseRootVisualOrStartupUriCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace OpenSilver.CodeAnalysis.Tests;

[TestClass]
public class UseRootVisualOrStartupUriUnitTests
{
    private const string OpenSilverSources =
        """
        namespace System.Windows
        {
            public class UIElement { }
            public class FrameworkElement : UIElement { }

            public class Window
            {
                public static Window Current { get; set; }
                public FrameworkElement Content { get; set; }
            }

            public class Application
            {
                public UIElement RootVisual { get; set; }
                public event EventHandler Startup;
            }
        }
        """;

    #region Analyzer Tests

    [TestMethod]
    public async Task When_Empty()
    {
        var source = OpenSilverSources;

        await AnalyzerVerifier.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task When_ApplicationClass_SetsWindowCurrentContent_ShouldWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        {|#0:Window.Current.Content|} = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        var expected = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);

        await AnalyzerVerifier.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task When_ApplicationClass_SetsWindowCurrentContent_InMethod_ShouldWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public void Initialize()
                    {
                        {|#0:Window.Current.Content|} = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        var expected = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);

        await AnalyzerVerifier.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task When_DerivedApplicationClass_SetsWindowCurrentContent_ShouldWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class BaseApp : Application { }

                public class App : BaseApp
                {
                    public App()
                    {
                        {|#0:Window.Current.Content|} = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        var expected = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);

        await AnalyzerVerifier.VerifyAnalyzerAsync(source, expected);
    }

    [TestMethod]
    public async Task When_NonApplicationClass_SetsWindowCurrentContent_ShouldNotWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class SomeClass
                {
                    public void DoSomething()
                    {
                        Window.Current.Content = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        await AnalyzerVerifier.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task When_ApplicationClass_SetsRootVisual_ShouldNotWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        this.RootVisual = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        await AnalyzerVerifier.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task When_ApplicationClass_SetsOtherWindowProperty_ShouldNotWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        Window.Current = new Window();
                    }
                }
            }
            """ + OpenSilverSources;

        await AnalyzerVerifier.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task When_ApplicationClass_SetsWindowCurrentContent_MultipleTimes_ShouldWarnEachTime()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        {|#0:Window.Current.Content|} = new FrameworkElement();
                    }

                    public void Reset()
                    {
                        {|#1:Window.Current.Content|} = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        var expected1 = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);
        var expected2 = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(1);

        await AnalyzerVerifier.VerifyAnalyzerAsync(source, expected1, expected2);
    }

    [TestMethod]
    public async Task When_ApplicationClass_ReadsWindowCurrentContent_ShouldNotWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public FrameworkElement GetContent()
                    {
                        return Window.Current.Content;
                    }
                }
            }
            """ + OpenSilverSources;

        await AnalyzerVerifier.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task When_NestedClass_InApplicationClass_SetsWindowCurrentContent_ShouldNotWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    private class Helper
                    {
                        public void DoSomething()
                        {
                            Window.Current.Content = new FrameworkElement();
                        }
                    }
                }
            }
            """ + OpenSilverSources;

        await AnalyzerVerifier.VerifyAnalyzerAsync(source);
    }

    [TestMethod]
    public async Task When_ApplicationClass_SetsWindowCurrentContent_InsideLambda_ShouldWarn()
    {
        var source =
            """
            namespace MyApp
            {
                using System;
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        Startup += (o, e) =>
                        {
                            {|#0:Window.Current.Content|} = new FrameworkElement();
                        };
                    }

                    public void Initialize()
                    {
                        Action setup = () =>
                        {
                            {|#1:Window.Current.Content|} = new FrameworkElement();
                        };
                        setup();
                    }
                }
            }
            """ + OpenSilverSources;

        var expected1 = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);
        var expected2 = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(1);

        await AnalyzerVerifier.VerifyAnalyzerAsync(source, expected1, expected2);
    }

    #endregion

    #region CodeFix Tests

    [TestMethod]
    public async Task CodeFix_ReplacesWindowCurrentContent_WithRootVisual()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        {|#0:Window.Current.Content|} = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        var fixedSource =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        RootVisual = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        var expected = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);

        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [TestMethod]
    public async Task CodeFix_ReplacesWindowCurrentContent_InMethod()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public void Initialize()
                    {
                        {|#0:Window.Current.Content|} = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        var fixedSource =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    public void Initialize()
                    {
                        RootVisual = new FrameworkElement();
                    }
                }
            }
            """ + OpenSilverSources;

        var expected = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);

        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [TestMethod]
    public async Task CodeFix_ReplacesWindowCurrentContent_InsideLambda()
    {
        var source =
            """
            namespace MyApp
            {
                using System;
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        Startup += (o, e) =>
                        {
                            {|#0:Window.Current.Content|} = new FrameworkElement();
                        };
                    }
                }
            }
            """ + OpenSilverSources;

        var fixedSource =
            """
            namespace MyApp
            {
                using System;
                using System.Windows;

                public class App : Application
                {
                    public App()
                    {
                        Startup += (o, e) =>
                        {
                            RootVisual = new FrameworkElement();
                        };
                    }
                }
            }
            """ + OpenSilverSources;

        var expected = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);

        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    [TestMethod]
    public async Task CodeFix_PreservesRightHandSideExpression()
    {
        var source =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    private FrameworkElement CreateMainPage() => new FrameworkElement();

                    public App()
                    {
                        {|#0:Window.Current.Content|} = CreateMainPage();
                    }
                }
            }
            """ + OpenSilverSources;

        var fixedSource =
            """
            namespace MyApp
            {
                using System.Windows;

                public class App : Application
                {
                    private FrameworkElement CreateMainPage() => new FrameworkElement();

                    public App()
                    {
                        RootVisual = CreateMainPage();
                    }
                }
            }
            """ + OpenSilverSources;

        var expected = new DiagnosticResult(UseRootVisualOrStartupUriAnalyzer.OS0002)
            .WithLocation(0);

        await CodeFixVerifier.VerifyCodeFixAsync(source, expected, fixedSource);
    }

    #endregion
}
