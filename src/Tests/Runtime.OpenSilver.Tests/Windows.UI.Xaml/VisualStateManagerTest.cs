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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class VisualStateManagerTest
    {
        [TestMethod]
        public void VSM_GoToState_When_Control_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => VisualStateManager.GoToState(null, "state", true));
        }

        [TestMethod]
        public void VSM_GoToState_When_State_Is_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() => VisualStateManager.GoToState(new ContentControl(), null, true));
        }

        [TestMethod]
        public void VSM_GoToState()
        {
            var myControl1 = new MyControl1();

            var prop1Default = (double)InnerControl.Property1Property.GetMetadata(myControl1.InnerControl.GetType()).DefaultValue;
            var prop2Default = (double)InnerControl.Property2Property.GetMetadata(myControl1.InnerControl.GetType()).DefaultValue;
            var prop3Default = (double)InnerControl.Property3Property.GetMetadata(myControl1.InnerControl.GetType()).DefaultValue;
            var prop4Default = (double)InnerControl.Property4Property.GetMetadata(myControl1.InnerControl.GetType()).DefaultValue;

            myControl1.InnerControl.Property1.Should().Be(prop1Default);
            myControl1.InnerControl.Property2.Should().Be(prop2Default);
            myControl1.InnerControl.Property3.Should().Be(prop3Default);
            myControl1.InnerControl.Property4.Should().Be(prop4Default);

            // 1 - Move from no current state to a state
            myControl1.GoToState1().Should().BeTrue();

            myControl1.InnerControl.Property1.Should().Be(42d);
            myControl1.InnerControl.Property2.Should().Be(prop2Default);
            myControl1.InnerControl.Property3.Should().Be(prop3Default);
            myControl1.InnerControl.Property4.Should().Be(prop4Default);

            // 2 - Move from a state to a state in the same group
            myControl1.GoToState2().Should().BeTrue();

            myControl1.InnerControl.Property1.Should().Be(prop1Default);
            myControl1.InnerControl.Property2.Should().Be(84d);
            myControl1.InnerControl.Property3.Should().Be(prop3Default);
            myControl1.InnerControl.Property4.Should().Be(prop4Default);

            // 3 - Move from a group to another
            myControl1.GoToState4().Should().BeTrue();

            myControl1.InnerControl.Property1.Should().Be(prop1Default);
            myControl1.InnerControl.Property2.Should().Be(84d);
            myControl1.InnerControl.Property3.Should().Be(prop3Default);
            myControl1.InnerControl.Property4.Should().Be(336d);

            // 4 - Move to a state that does not exist

            VisualStateManager.GoToState(myControl1, "some_state_that_does_not_exist", false).Should().BeFalse();

            myControl1.InnerControl.Property1.Should().Be(prop1Default);
            myControl1.InnerControl.Property2.Should().Be(84d);
            myControl1.InnerControl.Property3.Should().Be(prop3Default);
            myControl1.InnerControl.Property4.Should().Be(336d);
        }

        [TestMethod]
        public void VSM_CustomVisualStateManager()
        {
            var myControl2 = new MyControl2();

            var prop1Default = (double)InnerControl.Property1Property.GetMetadata(myControl2.InnerControl.GetType()).DefaultValue;
            var prop2Default = (double)InnerControl.Property2Property.GetMetadata(myControl2.InnerControl.GetType()).DefaultValue;
            var prop3Default = (double)InnerControl.Property3Property.GetMetadata(myControl2.InnerControl.GetType()).DefaultValue;
            var prop4Default = (double)InnerControl.Property4Property.GetMetadata(myControl2.InnerControl.GetType()).DefaultValue;

            myControl2.InnerControl.Property1.Should().Be(prop1Default);
            myControl2.InnerControl.Property2.Should().Be(prop2Default);
            myControl2.InnerControl.Property3.Should().Be(prop3Default);
            myControl2.InnerControl.Property4.Should().Be(prop4Default);
            myControl2.InnerControl.CurrentState.Should().BeNull();

            myControl2.GoToState1().Should().BeTrue();

            myControl2.InnerControl.Property1.Should().Be(prop1Default);
            myControl2.InnerControl.Property2.Should().Be(prop2Default);
            myControl2.InnerControl.Property3.Should().Be(prop3Default);
            myControl2.InnerControl.Property4.Should().Be(prop4Default);
            myControl2.InnerControl.CurrentState.Should().Be("state1");

            myControl2.GoToState2().Should().BeTrue();

            myControl2.InnerControl.Property1.Should().Be(prop1Default);
            myControl2.InnerControl.Property2.Should().Be(prop2Default);
            myControl2.InnerControl.Property3.Should().Be(prop3Default);
            myControl2.InnerControl.Property4.Should().Be(prop4Default);
            myControl2.InnerControl.CurrentState.Should().Be("state2");

            myControl2.GoToState3().Should().BeTrue();

            myControl2.InnerControl.Property1.Should().Be(prop1Default);
            myControl2.InnerControl.Property2.Should().Be(prop2Default);
            myControl2.InnerControl.Property3.Should().Be(prop3Default);
            myControl2.InnerControl.Property4.Should().Be(prop4Default);
            myControl2.InnerControl.CurrentState.Should().Be("state3");

            myControl2.GoToState4().Should().BeTrue();

            myControl2.InnerControl.Property1.Should().Be(prop1Default);
            myControl2.InnerControl.Property2.Should().Be(prop2Default);
            myControl2.InnerControl.Property3.Should().Be(prop3Default);
            myControl2.InnerControl.Property4.Should().Be(prop4Default);
            myControl2.InnerControl.CurrentState.Should().Be("state4");

            VisualStateManager.GoToState(myControl2, "some_state_that_does_not_exist", false).Should().BeFalse();

            myControl2.InnerControl.Property1.Should().Be(prop1Default);
            myControl2.InnerControl.Property2.Should().Be(prop2Default);
            myControl2.InnerControl.Property3.Should().Be(prop3Default);
            myControl2.InnerControl.Property4.Should().Be(prop4Default);
            myControl2.InnerControl.CurrentState.Should().BeNull();
        }

        private class MyControl1 : Control
        {
            public InnerControl InnerControl { get; }

            public MyControl1()
            {
                // We emulate the following XAML code :

                /* <local:MyControl1>
                 *   <local:MyControl1.Template>
                 *     <ControlTemplate TargetType="local:MyControl1">
                 *       <Border>
                 *         <VisualStateManager.VisualStateGroups>
                 *           <VisualStateGroup x:Name="group1">
                 *             <VisualState x:Name="state1">
                 *               <Storyboard>
                 *                 <DoubleAnimation Storyboard.TargetName="InnerControl"
                 *                                  Storyboard.TargetProperty="Property1"
                 *                                  Duration="0"
                 *                                  To="42" />
                 *               </Storyboard>
                 *             </VisualState>
                 *             <VisualState x:Name="state2">
                 *               <Storyboard>
                 *                 <DoubleAnimation Storyboard.TargetName="InnerControl"
                 *                                  Storyboard.TargetProperty="Property2"
                 *                                  Duration="0"
                 *                                  To="84" />
                 *               </Storyboard>
                 *             </VisualState>
                 *           </VisualStateGroup>
                 *           <VisualStateGroup x:Name="group2">
                 *             <VisualState x:Name="state3">
                 *               <Storyboard>
                 *                 <DoubleAnimation Storyboard.TargetName="InnerControl"
                 *                                  Storyboard.TargetProperty="Property3"
                 *                                  Duration="0"
                 *                                  To="168" />
                 *               </Storyboard>
                 *             </VisualState>
                 *             <VisualState x:Name="state4">
                 *               <Storyboard>
                 *                 <DoubleAnimation Storyboard.TargetName="InnerControl"
                 *                                  Storyboard.TargetProperty="Property4"
                 *                                  Duration="0"
                 *                                  To="336" />
                 *               </Storyboard>
                 *             </VisualState>
                 *           </VisualStateGroup/>
                 *         </VisualStateManager.VisualStateGroups>
                 *         <local:InnerControl x:Name="InnerControl" />
                 *       </Border>
                 *     </ControlTemplate>
                 *   </local:MyControl1.Template>
                 * <local:MyControl1>
                 */

                InnerControl = new InnerControl() { Name = "InnerControl" };

                // Emulate a template
                TemplateChild = new Border
                {
                    Child = InnerControl,
                };

                // Ensure we can find the InnerControl with Control.GetTempleChild(...)
                RegisterName("InnerControl", InnerControl);

                // 1 - VisualStateGroup 1
                var group1 = new VisualStateGroup { Name = "group1" };

                // 1.1 - VisualState 1
                var state1 = new VisualState { Name = "state1" };
                var sb1 = new Storyboard();
                var db1 = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.Zero),
                    To = 42d,
                };
                sb1.Children.Add(db1);
                Storyboard.SetTargetName(db1, "InnerControl");
                Storyboard.SetTargetProperty(db1, new PropertyPath(InnerControl.Property1Property));
                state1.Storyboard = sb1;

                // 1.2 - VisualState 2
                var state2 = new VisualState { Name = "state2" };
                var sb2 = new Storyboard();
                var db2 = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.Zero),
                    To = 84d,
                };
                sb2.Children.Add(db2);
                Storyboard.SetTargetName(db2, "InnerControl");
                Storyboard.SetTargetProperty(db2, new PropertyPath(InnerControl.Property2Property));
                state2.Storyboard = sb2;

                group1.States.Add(state1);
                group1.States.Add(state2);

                // 2 - VisualStateGroup 2
                var group2 = new VisualStateGroup { Name = "group2" };

                // 2.1 - VisualState 3
                var state3 = new VisualState { Name = "state3" };
                var sb3 = new Storyboard();
                var db3 = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.Zero),
                    To = 168d,
                };
                sb3.Children.Add(db3);
                Storyboard.SetTargetName(db3, "InnerControl");
                Storyboard.SetTargetProperty(db3, new PropertyPath(InnerControl.Property3Property));
                state3.Storyboard = sb3;

                // 2.2 - VisualState 4
                var state4 = new VisualState { Name = "state4" };
                var sb4 = new Storyboard();
                var db4 = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.Zero),
                    To = 336d,
                };
                sb4.Children.Add(db4);
                Storyboard.SetTargetName(db4, "InnerControl");
                Storyboard.SetTargetProperty(db4, new PropertyPath(InnerControl.Property4Property));
                state4.Storyboard = sb4;

                group2.States.Add(state3);
                group2.States.Add(state4);

                // Register VisualStateGroups
                VisualStateManager.GetVisualStateGroups(TemplateChild).Add(group1);
                VisualStateManager.GetVisualStateGroups(TemplateChild).Add(group2);
            }

            public bool GoToState1() => VisualStateManager.GoToState(this, "state1", false);

            public bool GoToState2() => VisualStateManager.GoToState(this, "state2", false);

            public bool GoToState3() => VisualStateManager.GoToState(this, "state3", false); 

            public bool GoToState4() => VisualStateManager.GoToState(this, "state4", false);
        }

        private class MyControl2 : Control
        {
            public InnerControl InnerControl { get; }

            public MyControl2()
            {
                // We emulate the following XAML code :

                /* <local:MyControl2>
                 *   <local:MyControl2.Template>
                 *     <ControlTemplate TargetType="local:MyControl1">
                 *       <Border>
                 *         <VisualStateManager.CustomVisualStateManager>
                 *           <local:Test_CustomVisualStateManager />
                 *         </VisualStateManager.CustomVisualStateManager>
                 *         <VisualStateManager.VisualStateGroups>
                 *           <VisualStateGroup x:Name="group1">
                 *             <VisualState x:Name="state1">
                 *               <Storyboard>
                 *                 <DoubleAnimation Storyboard.TargetName="InnerControl"
                 *                                  Storyboard.TargetProperty="Property1"
                 *                                  Duration="0"
                 *                                  To="42" />
                 *               </Storyboard>
                 *             </VisualState>
                 *             <VisualState x:Name="state2">
                 *               <Storyboard>
                 *                 <DoubleAnimation Storyboard.TargetName="InnerControl"
                 *                                  Storyboard.TargetProperty="Property2"
                 *                                  Duration="0"
                 *                                  To="84" />
                 *               </Storyboard>
                 *             </VisualState>
                 *           </VisualStateGroup>
                 *           <VisualStateGroup x:Name="group2">
                 *             <VisualState x:Name="state3">
                 *               <Storyboard>
                 *                 <DoubleAnimation Storyboard.TargetName="InnerControl"
                 *                                  Storyboard.TargetProperty="Property3"
                 *                                  Duration="0"
                 *                                  To="168" />
                 *               </Storyboard>
                 *             </VisualState>
                 *             <VisualState x:Name="state4">
                 *               <Storyboard>
                 *                 <DoubleAnimation Storyboard.TargetName="InnerControl"
                 *                                  Storyboard.TargetProperty="Property4"
                 *                                  Duration="0"
                 *                                  To="336" />
                 *               </Storyboard>
                 *             </VisualState>
                 *           </VisualStateGroup/>
                 *         </VisualStateManager.VisualStateGroups>
                 *         <local:InnerControl x:Name="InnerControl" />
                 *       </Border>
                 *     </ControlTemplate>
                 *   </local:MyControl2.Template>
                 * <local:MyControl2>
                 */

                InnerControl = new InnerControl() { Name = "InnerControl" };

                // Emulate a template
                TemplateChild = new Border
                {
                    Child = InnerControl,
                };

                // Ensure we can find the InnerControl with Control.GetTempleChild(...)
                RegisterName("InnerControl", InnerControl);

                VisualStateManager.SetCustomVisualStateManager(TemplateChild, new Test_CustomVisualStateManager());

                // 1 - VisualStateGroup 1
                var group1 = new VisualStateGroup { Name = "group1" };

                // 1.1 - VisualState 1
                var state1 = new VisualState { Name = "state1" };
                var sb1 = new Storyboard();
                var db1 = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.Zero),
                    To = 42d,
                };
                sb1.Children.Add(db1);
                Storyboard.SetTargetName(db1, "InnerControl");
                Storyboard.SetTargetProperty(db1, new PropertyPath(InnerControl.Property1Property));
                state1.Storyboard = sb1;

                // 1.2 - VisualState 2
                var state2 = new VisualState { Name = "state2" };
                var sb2 = new Storyboard();
                var db2 = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.Zero),
                    To = 84d,
                };
                sb2.Children.Add(db2);
                Storyboard.SetTargetName(db2, "InnerControl");
                Storyboard.SetTargetProperty(db2, new PropertyPath(InnerControl.Property2Property));
                state2.Storyboard = sb2;

                group1.States.Add(state1);
                group1.States.Add(state2);

                // 2 - VisualStateGroup 2
                var group2 = new VisualStateGroup { Name = "group2" };

                // 2.1 - VisualState 3
                var state3 = new VisualState { Name = "state3" };
                var sb3 = new Storyboard();
                var db3 = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.Zero),
                    To = 168d,
                };
                sb3.Children.Add(db3);
                Storyboard.SetTargetName(db3, "InnerControl");
                Storyboard.SetTargetProperty(db3, new PropertyPath(InnerControl.Property3Property));
                state3.Storyboard = sb3;

                // 2.2 - VisualState 4
                var state4 = new VisualState { Name = "state4" };
                var sb4 = new Storyboard();
                var db4 = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.Zero),
                    To = 336d,
                };
                sb4.Children.Add(db4);
                Storyboard.SetTargetName(db4, "InnerControl");
                Storyboard.SetTargetProperty(db4, new PropertyPath(InnerControl.Property4Property));
                state4.Storyboard = sb4;

                group2.States.Add(state3);
                group2.States.Add(state4);

                // Register VisualStateGroups
                VisualStateManager.GetVisualStateGroups(TemplateChild).Add(group1);
                VisualStateManager.GetVisualStateGroups(TemplateChild).Add(group2);
            }

            public bool GoToState1() => VisualStateManager.GoToState(this, "state1", false);

            public bool GoToState2() => VisualStateManager.GoToState(this, "state2", false);

            public bool GoToState3() => VisualStateManager.GoToState(this, "state3", false);

            public bool GoToState4() => VisualStateManager.GoToState(this, "state4", false);
        }

        private class InnerControl : Control
        {
            public double Property1
            {
                get => (double)GetValue(Property1Property);
                set => SetValue(Property1Property, value);
            }

            public static readonly DependencyProperty Property1Property =
                DependencyProperty.Register(
                    nameof(Property1), 
                    typeof(double), 
                    typeof(InnerControl), 
                    new PropertyMetadata(1d));

            public double Property2
            {
                get => (double)GetValue(Property2Property);
                set => SetValue(Property2Property, value);
            }

            public static readonly DependencyProperty Property2Property =
                DependencyProperty.Register(
                    nameof(Property2),
                    typeof(double),
                    typeof(InnerControl),
                    new PropertyMetadata(2d));

            public double Property3
            {
                get => (double)GetValue(Property3Property);
                set => SetValue(Property3Property, value);
            }

            public static readonly DependencyProperty Property3Property =
                DependencyProperty.Register(
                    nameof(Property3),
                    typeof(double),
                    typeof(InnerControl),
                    new PropertyMetadata(3d));

            public double Property4
            {
                get => (double)GetValue(Property4Property);
                set => SetValue(Property4Property, value);
            }

            public static readonly DependencyProperty Property4Property =
                DependencyProperty.Register(
                    nameof(Property4),
                    typeof(double),
                    typeof(InnerControl),
                    new PropertyMetadata(4d));

            public string CurrentState { get; set; }
        }

        private class Test_CustomVisualStateManager : VisualStateManager
        {
            protected override bool GoToStateCore(Control control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
            {
                var innerControl = (control as MyControl2).InnerControl;
                if (innerControl == null)
                    return false;

                switch (stateName)
                {
                    case "state1":
                    case "state2":
                    case "state3":
                    case "state4":
                        innerControl.CurrentState = stateName;
                        return true;
                    default:
                        innerControl.CurrentState = null;
                        return false;
                }
            }
        }
    }
}
