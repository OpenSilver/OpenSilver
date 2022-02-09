using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Testing;

namespace OpenSilver.Analyzers.Test
{
    [TestClass]
    public class NotImplementedUnitTest : CSharpAnalyzerVerifier<NotImplementedAnalyzer, MSTestVerifier>
    {
        private const string NotImplementedAttribute = @"
        namespace OpenSilver
        {
            using System;

            [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
            public sealed class NotImplementedAttribute : Attribute
            {
                public NotImplementedAttribute() { }
            }
        }";

        [TestMethod]
        public async Task When_Empty()
        {
            var source = @"" + NotImplementedAttribute;

            await VerifyAnalyzerAsync(source);
        }

        #region Member access (SyntaxKind.ObjectCreationExpression)

        [TestMethod]
        public async Task When_Accessing_Not_Implemented_Assembly()
        {
            var source = @"
            [assembly: OpenSilver.NotImplemented]
            namespace OpenSilver
            {
                public class Class
                {
                    public int TestMethod() { return 1; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public int Method1(Class obj)
                    {
                        return obj.{|#0:TestMethod|#0}();
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("TestProject, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        [TestMethod]
        public async Task When_Accessing_Implemented_Member()
        {
            var source = @"
            namespace OpenSilver
            {
                using System;

                public class Class
                {
                    public double TestProperty { get { return 10d; } }

                    public void TestMethod() { }

                    public event EventHandler EventFieldTest;

                    public event EventHandler EventTest { add { } remove { } }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    private Class _class;

                    public App()
                    {
                        _class = new Class();
                        _class.TestMethod();
                        var d = _class.TestProperty;
                        _class.EventFieldTest += (o, e) => { };
                        _class.EventTest += (o, e) => { };
                    }
                }
            }
            " + NotImplementedAttribute;

            await VerifyAnalyzerAsync(source);
        }

        #region Method

        [TestMethod]
        public async Task When_Accessing_Not_Implemented_Method()
        {
            var source = @"
            namespace OpenSilver
            {
                public class Class
                {
                    [NotImplemented]
                    public int TestMethod() { return 1; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public int Method1()
                    {
                        return new Class().{|#0:TestMethod|#0}();
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class.TestMethod()")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        [TestMethod]
        public async Task When_Accessing_Override_Method_And_Base_Implementation_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                public abstract class BaseClass 
                {
                    [NotImplemented]
                    public abstract int TestMethod();
                }

                public class Class : BaseClass
                {
                    public override int {|#0:TestMethod|#0}() { return 1; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class MyClass : Class
                {
                    public override int {|#1:TestMethod|#1}() { return 2; }
                }

                public class App
                {
                    public int Method1()
                    {
                        return new MyClass().{|#2:TestMethod|#2}();
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass.TestMethod()")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass.TestMethod()")
                .WithLocation(1);
            var expected3 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass.TestMethod()")
                .WithLocation(2);

            await VerifyAnalyzerAsync(source, expected1, expected2, expected3);
        }

        [TestMethod]
        public async Task When_Accessing_Method_In_Not_Implemented_Type()
        {
            var source = @"
            namespace OpenSilver
            {
                [NotImplemented]
                public class Class
                {
                    public int TestMethod() { return 1; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public int Method1()
                    {
                        return new {|#0:Class|#0}().{|#1:TestMethod|#1}();
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(1);

            await VerifyAnalyzerAsync(source, expected1, expected2);
        }

        [TestMethod]
        public async Task When_Accessing_Method_In_Type_With_Not_Implemented_Base_Type()
        {
            var source = @"
            namespace OpenSilver
            {
                [NotImplemented]
                public class BaseClass { }

                public class Class : {|#0:BaseClass|#0} 
                {
                    public int TestMethod() { return 1; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public int Method1()
                    {
                        return new {|#1:Class|#1}().{|#2:TestMethod|#2}();
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(1);
            var expected3 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(2);

            await VerifyAnalyzerAsync(source, expected1, expected2, expected3);
        }

        #endregion Method

        #region Property

        [TestMethod]
        public async Task When_Accessing_Not_Implemented_Property()
        {
            var source = @"
            namespace OpenSilver
            {
                public class Class
                {
                    [NotImplemented]
                    public int TestProperty { get; set; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public int Method1()
                    {
                        return new Class().{|#0:TestProperty|#0};
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class.TestProperty")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        [TestMethod]
        public async Task When_Accessing_Override_Property_And_Base_Implementation_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                public abstract class BaseClass 
                {
                    [NotImplemented]
                    public abstract int TestProperty { get; set; }
                }

                public class Class : BaseClass
                {
                    protected int _testProperty;

                    public override int {|#0:TestProperty|#0}
                    {
                        get => _testProperty;
                        set => _testProperty = value;
                    }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class MyClass : Class
                {
                    public override int {|#1:TestProperty|#1}
                    {
                        get => _testProperty + 1;
                        set => _testProperty = value - 1;
                    }
                }

                public class App
                {
                    public int Method1()
                    {
                        var myClass = new MyClass();
                        myClass.{|#2:TestProperty|#2} = 42;
                        return myClass.{|#3:TestProperty|#3};
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass.TestProperty")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass.TestProperty")
                .WithLocation(1);
            var expected3 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass.TestProperty")
                .WithLocation(2);
            var expected4 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass.TestProperty")
                .WithLocation(3);

            await VerifyAnalyzerAsync(source, expected1, expected2, expected3, expected4);
        }

        [TestMethod]
        public async Task When_Accessing_Property_In_Not_Implemented_Type()
        {
            var source = @"
            namespace OpenSilver
            {
                [NotImplemented]
                public class Class
                {
                    public int TestProperty 
                    {
                        get => 1;
                        set { }
                    }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public int Method1()
                    {
                        var myClass = new {|#0:Class|#0}();
                        myClass.{|#1:TestProperty|#1} = 42;
                        return myClass.{|#2:TestProperty|#2};
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(1);
            var expected3 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(2);

            await VerifyAnalyzerAsync(source, expected1, expected2, expected3);
        }

        [TestMethod]
        public async Task When_Accessing_Property_In_Type_With_Not_Implemented_Base_Type()
        {
            var source = @"
            namespace OpenSilver
            {
                [NotImplemented]
                public class BaseClass { }

                public class Class : {|#0:BaseClass|#0} 
                {
                    public int TestProperty => 42;
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public int Method1()
                    {
                        return new {|#1:Class|#1}().{|#2:TestProperty|#2};
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(1);
            var expected3 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(2);

            await VerifyAnalyzerAsync(source, expected1, expected2, expected3);
        }

        #endregion Property

        #region Event

        [TestMethod]
        public async Task When_Accessing_Not_Implemented_Event()
        {
            var source = @"
            namespace OpenSilver
            {
                using System;

                public class Class
                {
                    [NotImplemented]
                    public event EventHandler TestEvent;
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public void Method1()
                    {
                        new Class().{|#0:TestEvent|#0} += (o, e) => { };
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class.TestEvent")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        [TestMethod]
        public async Task When_Accessing_Override_Event_And_Base_Implementation_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                using System;

                public abstract class BaseClass 
                {
                    [NotImplemented]
                    public abstract event EventHandler TestEvent;
                }

                public class Class : BaseClass
                {
                    public override event EventHandler TestEvent;
                }
            }

            namespace OpenSilverApplication1
            {
                using System;
                using OpenSilver;

                public class MyClass : Class
                {
                    public override event EventHandler TestEvent;
                }

                public class App
                {
                    public void Method1()
                    {
                        new MyClass().{|#0:TestEvent|#0} -= (o, e) => { };
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass.TestEvent")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected1);
        }

        [TestMethod]
        public async Task When_Accessing_Event_In_Not_Implemented_Type()
        {
            var source = @"
            namespace OpenSilver
            {
                using System;

                [NotImplemented]
                public class Class
                {
                    public event EventHandler TestEvent;
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public void Method1()
                    {
                        new {|#0:Class|#0}().{|#1:TestEvent|#1} += (o, e) => { };
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(1);

            await VerifyAnalyzerAsync(source, expected1, expected2);
        }

        [TestMethod]
        public async Task When_Accessing_Event_In_Type_With_Not_Implemented_Base_Type()
        {
            var source = @"
            namespace OpenSilver
            {
                using System;

                [NotImplemented]
                public class BaseClass { }

                public class Class : {|#0:BaseClass|#0} 
                {
                    public event EventHandler TestEvent;
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public void Method1()
                    {
                        new {|#1:Class|#1}().{|#2:TestEvent|#2} += (o, e) => { };
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(1);
            var expected3 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(2);

            await VerifyAnalyzerAsync(source, expected1, expected2, expected3);
        }

        #endregion Event

        #endregion Member access (SyntaxKind.ObjectCreationExpression)

        #region Object creation (SyntaxKind.ObjectCreationExpression)

        [TestMethod]
        public async Task When_Not_Implemented_Assembly()
        {
            var source = @"
            [assembly: OpenSilver.NotImplemented]
            namespace OpenSilver
            {
                public class Class
                {
                    public int TestMethod() { return 1; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public object Method1()
                    {
                        return new {|#0:Class|#0}();
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("TestProject, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        [TestMethod]
        public async Task When_Constructor_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                public class Class 
                {
                    [NotImplemented]
                    public Class(int id) { }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public App()
                    {
                        var tmp = new {|#0:Class|#0}(1);
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class.Class(int)")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        [TestMethod]
        public async Task When_Constructor_And_Type_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                [NotImplemented]
                public class Class { }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public App()
                    {
                        var tmp = new {|#0:Class|#0}();
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        [TestMethod]
        public async Task When_Constructor_And_Base_Type_Is_Not_Implemented()
        {
            var source = @"            
            namespace OpenSilver
            {
                [NotImplemented]
                public class BaseClass { }

                public class Class : {|#0:BaseClass|#0} { }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class App
                {
                    public App()
                    {
                        var tmp = new {|#1:Class|#1}();
                    }
                }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(0);
            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(1);

            await VerifyAnalyzerAsync(source, expected1, expected2);
        }

        #endregion Object creation (SyntaxKind.ObjectCreationExpression)        

        #region Method Declaration (SyntaxKind.MethodDeclaration)

        [TestMethod]
        public async Task When_Method_Is_Not_An_Override()
        {
            var source = @"
            namespace OpenSilver
            {
                public class Class { }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class MyClass : Class
                {
                    public int TestMethod() { return 1; }
                }
            }
            " + NotImplementedAttribute;

            await VerifyAnalyzerAsync(source);
        }

        [TestMethod]
        public async Task When_Method_Is_An_Override_And_Base_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                public class Class
                {
                    [NotImplemented]
                    public virtual int TestMethod() { return 0; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class MyClass : Class
                {
                    public override int {|#0:TestMethod|#0}() { return 1; }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class.TestMethod()")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        #endregion Method Declaration (SyntaxKind.MethodDeclaration)

        #region Property Declaration (SyntaxKind.PropertyDeclaration)

        [TestMethod]
        public async Task When_Property_Is_Not_An_Override()
        {
            var source = @"
            namespace OpenSilver
            {
                public class Class { }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class MyClass : Class
                {
                    public int TestProperty { get; set; }
                }
            }
            " + NotImplementedAttribute;

            await VerifyAnalyzerAsync(source);
        }

        [TestMethod]
        public async Task When_Property_Is_An_Override_And_Base_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                public class Class
                {
                    [NotImplemented]
                    public virtual int TestProperty { get; set; }
                }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class MyClass : Class
                {
                    public override int {|#0:TestProperty|#0} { get; set; }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class.TestProperty")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        #endregion Property Declaration (SyntaxKind.PropertyDeclaration)

        #region Event Declaration (SyntaxKind.EventDeclaration)

        [TestMethod]
        public async Task When_Event_Is_Not_An_Override()
        {
            var source = @"
            namespace OpenSilver
            {
                public class Class { }
            }

            namespace OpenSilverApplication1
            {
                using System;
                using OpenSilver;
                
                public class MyClass : Class
                {
                    public event EventHandler TestEvent;
                }
            }
            " + NotImplementedAttribute;

            await VerifyAnalyzerAsync(source);
        }

        [TestMethod]
        public async Task When_Event_Is_An_Override_And_Base_Is_Not_Implemented_And_Uses_Event_Field_Syntax()
        {
            var source = @"
            namespace OpenSilver
            {
                using System;

                public class Class
                {
                    [NotImplemented]
                    public virtual event EventHandler TestEvent;
                }
            }

            namespace OpenSilverApplication1
            {
                using System;
                using OpenSilver;

                public class MyClass : Class
                {
                    public override event EventHandler TestEvent;
                }
            }
            " + NotImplementedAttribute;

            await VerifyAnalyzerAsync(source);
        }

        [TestMethod]
        public async Task When_Event_Is_An_Override_And_Base_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                using System;

                public class Class
                {
                    [NotImplemented]
                    public virtual event EventHandler TestEvent;
                }
            }

            namespace OpenSilverApplication1
            {
                using System;
                using OpenSilver;

                public class MyClass : Class
                {
                    public override event EventHandler {|#0:TestEvent|#0} { add { } remove { } }
                }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class.TestEvent")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        #endregion Event Declaration (SyntaxKind.EventDeclaration)

        #region Base type (SyntaxKind.SimpleBaseType)

        [TestMethod]
        public async Task When_Base_Class_Declaration_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                [NotImplemented]
                public class Class { }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class DerivedClass : {|#0:Class|#0} { }
            }
            " + NotImplementedAttribute;

            var expected = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.Class")
                .WithLocation(0);

            await VerifyAnalyzerAsync(source, expected);
        }

        [TestMethod]
        public async Task When_Base_Base_Class_Declaration_Is_Not_Implemented()
        {
            var source = @"
            namespace OpenSilver
            {
                [NotImplemented]
                public class BaseClass { }

                public class Class : {|#0:BaseClass|#0} { }
            }

            namespace OpenSilverApplication1
            {
                using OpenSilver;

                public class DerivedClass : {|#1:Class|#1} { }
            }
            " + NotImplementedAttribute;

            var expected1 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(0);

            var expected2 = new DiagnosticResult(NotImplementedAnalyzer.OS0001)
                .WithArguments("OpenSilver.BaseClass")
                .WithLocation(1);

            await VerifyAnalyzerAsync(source, expected1, expected2);
        }

        #endregion Base type (SyntaxKind.SimpleBaseType)
    }
}
