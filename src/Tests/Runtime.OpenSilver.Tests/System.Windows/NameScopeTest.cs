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
using System.Collections;
using System.Collections.ObjectModel;
using OpenSilver.Internal;
using OpenSilver.Internal.Xaml.Context;
using OpenSilver.Internal.Xaml;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Tests
#else
namespace Windows.UI.Xaml.Tests
#endif
{
    [TestClass]
    public class NameScopeTest
    {
        private readonly UserControl1 _uc1 = new UserControl1();

        [TestMethod]
        public void FindName_When_Target_Has_NameScope()
        {
            _uc1.FindName("name1").Should().Be(_uc1.name1);
            _uc1.FindName("name2").Should().Be(_uc1.name2);
            _uc1.FindName("name3").Should().Be(_uc1.name3);
            _uc1.FindName("name6").Should().Be(_uc1.name6);
            _uc1.FindName("name7").Should().Be(_uc1.name7);
        }

        [TestMethod]
        public void FindName_Should_Use_Logical_Tree()
        {
            _uc1.name2.FindName("name1").Should().Be(_uc1.name1);
            _uc1.name2.FindName("name2").Should().Be(_uc1.name2);
            _uc1.name2.FindName("name3").Should().Be(_uc1.name3);
            _uc1.name2.FindName("name6").Should().Be(_uc1.name6);
            _uc1.name2.FindName("name7").Should().Be(_uc1.name7);
        }

        [TestMethod]
        public void FindName_Should_Use_TemplatedParent()
        {
            Control2 control2 = _uc1.name2.Children[4] as Control2;
            control2.Should().NotBeNull();

            ContentControl child = control2.Child as ContentControl;

            child.FindName("name1").Should().Be(_uc1.name1);
            child.FindName("name2").Should().Be(_uc1.name2);
            child.FindName("name3").Should().Be(_uc1.name3);
            child.FindName("name6").Should().Be(_uc1.name6);
            child.FindName("name7").Should().Be(_uc1.name7);
        }

        [TestMethod]
        public void FindName_Should_Use_VisualTree_1()
        {
            ItemsControl ic = _uc1.name2.Children[2] as ItemsControl;
            ic.Should().NotBeNull();

            FrameworkElement container = ic.ItemContainerGenerator.ContainerFromIndex(0) as FrameworkElement;
            container.Should().NotBeNull();

            container.FindName("name1").Should().Be(_uc1.name1);
            container.FindName("name2").Should().Be(_uc1.name2);
            container.FindName("name3").Should().Be(_uc1.name3);
            container.FindName("name6").Should().Be(_uc1.name6);
            container.FindName("name7").Should().Be(_uc1.name7);
        }

        [TestMethod]
        public void FindName_Should_Use_VisualTree_2()
        {
            ContentPresenter cp = _uc1.name2.Children[5] as ContentPresenter;
            cp.Should().NotBeNull();

            ContentControl cc = cp.Content as ContentControl;
            cc.Should().NotBeNull();

            cc.FindName("name1").Should().Be(_uc1.name1);
            cc.FindName("name2").Should().Be(_uc1.name2);
            cc.FindName("name3").Should().Be(_uc1.name3);
            cc.FindName("name6").Should().Be(_uc1.name6);
            cc.FindName("name7").Should().Be(_uc1.name7);
        }

        [TestMethod]
        public void FindName_Should_Not_Look_In_External_Templates()
        {
            _uc1.FindName("name4").Should().BeNull();
            _uc1.FindName("name5").Should().BeNull();
            _uc1.name2.FindName("name4").Should().BeNull();
            _uc1.name2.FindName("name5").Should().BeNull();
        }

        [TestMethod]
        public void FindName_Should_Find_In_Template()
        {
            Border child = VisualTreeHelper.GetChild(_uc1.name3, 0) as Border;
            Border childOfChild = child.Child as Border;
            child.Should().NotBeNull();
            childOfChild.Should().NotBeNull();

            child.FindName("name4").Should().Be(child);
            child.FindName("name5").Should().Be(childOfChild);

            childOfChild.FindName("name4").Should().Be(child);
            childOfChild.FindName("name5").Should().Be(childOfChild);
        }

        [TestMethod]
        public void FindName_Should_Look_Only_In_Template()
        {
            Border child = VisualTreeHelper.GetChild(_uc1.name3, 0) as Border;
            Border childOfChild = child.Child as Border;
            child.Should().NotBeNull();
            childOfChild.Should().NotBeNull();

            child.FindName("name1").Should().BeNull();
            child.FindName("name2").Should().BeNull();
            child.FindName("name3").Should().BeNull();
            child.FindName("name6").Should().BeNull();
            child.FindName("name7").Should().BeNull();

            childOfChild.FindName("name1").Should().BeNull();
            childOfChild.FindName("name2").Should().BeNull();
            childOfChild.FindName("name3").Should().BeNull();
            childOfChild.FindName("name6").Should().BeNull();
            childOfChild.FindName("name7").Should().BeNull();
        }

        [TestMethod]
        public void FindName_Should_Stop_At_First_NameScope()
        {
            _uc1.name6.FindName("uc_name1").Should().Be(_uc1.name6.uc_name1);
            _uc1.name7.FindName("uc_name1").Should().Be(_uc1.name6.uc_name1);

            _uc1.name6.FindName("name1").Should().BeNull();
            _uc1.name6.FindName("name2").Should().BeNull();
            _uc1.name6.FindName("name3").Should().BeNull();
            _uc1.name6.FindName("name6").Should().BeNull();
            _uc1.name6.FindName("name7").Should().BeNull();

            _uc1.name7.FindName("name1").Should().BeNull();
            _uc1.name7.FindName("name2").Should().BeNull();
            _uc1.name7.FindName("name3").Should().BeNull();
            _uc1.name7.FindName("name6").Should().BeNull();
            _uc1.name7.FindName("name7").Should().BeNull();
        }

        [TestMethod]
        public void FindName_Template_Should_Have_Priority()
        {
            Control1 control1 = _uc1.name2.Children[3] as Control1;
            control1.Should().NotBeNull();

            Border border = VisualTreeHelper.GetChild(control1, 0) as Border;
            border.Should().NotBeNull();

            UserControl2 uc2 = border.Child as UserControl2;
            uc2.Should().NotBeNull();

            ContentControl cc = uc2.Children[0] as ContentControl;
            cc.Should().NotBeNull();

            border.FindName("name8").Should().Be(border);
            border.FindName("name9").Should().Be(uc2);
            border.FindName("name10").Should().Be(cc);
            uc2.FindName("name8").Should().Be(border);
            uc2.FindName("name9").Should().Be(uc2);
            uc2.FindName("name10").Should().Be(cc);
            cc.FindName("name8").Should().Be(border);
            cc.FindName("name9").Should().Be(uc2);
            cc.FindName("name10").Should().Be(cc);

            uc2.FindName("uc_name1").Should().BeNull();
            cc.FindName("uc_name1").Should().BeNull();
        }

        /*
         * <UserControl x:Class="Local.UserControl1"
         *              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
         *              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
         *              xmlns:local="clr-namespace:Local"
         *              xmlns:sys="clr-namespace:System;assembly=mscorlib"
         *              x:Name="name1">
         *     <StackPanel x:Name="name2">
         *         <local:Control1 x:Name="name3">
         *             <local:Control1.Template>
         *                 <ControlTemplate TargetType="local:Control1">
         *                     <Border x:Name="name4">
         *                         <Border x:Name="name5" />
         *                     </Border>
         *                 </ControlTemplate>
         *             </local:Control1.Template>
         *         </local:Control1>
         *         <local:UserControl1 x:Name="name6">
         *             <local:UserControl1.Children>
         *                 <ContentControl x:Name="name7" />
         *             </local:UserControl1.Children>
         *         </local:UserControl1>
         *         <ItemsControl>
         *             <sys:String>string 1</sys:String>
         *         </ItemsControl>
         *         <local:Control1>
         *             <local:Control1.Template>
         *                 <ControlTemplate TargetType="local:Control1">
         *                     <Border x:Name="name8">
         *                         <local:UserControl1 x:Name="name9">
         *                             <local:UserControl1.Children>
         *                                 <ContentControl x:Name="name10" />
         *                             </local:UserControl1.Children>
         *                         </local:UserControl1>
         *                     </Border>
         *                 </ControlTemplate>
         *             </local:Control1.Template>
         *         </local:Control1>
         *         <local:Control2>
         *             <local:Control2.Template>
         *                 <ControlTemplate TargetType="local:Control2>
         *                     <Border x:Name="border" />
         *                 </ControlTemplate>
         *             </local:Control2.Template>
         *             <ContentControl />
         *         </local:Control2>
         *         <ContentPresenter>
         *             <ContentControl />
         *         </ContentPresenter>
         *     </StackPanel>
         * </UserControl>
         */
        private class UserControl1 : UserControl
        {
            private bool _contentLoaded;
            internal UserControl1 name1;
            internal StackPanel name2;
            internal Control1 name3;
            internal UserControl2 name6;
            internal ContentControl name7;

            public UserControl1()
            {
                InitializeComponent();
            }

            private void InitializeComponent()
            {
                if (_contentLoaded)
                {
                    return;
                }
                _contentLoaded = true;

                Name = "name1";

                StackPanel name2 = new StackPanel
                {
                    Name = "name2"
                };

                Control1 name3 = new Control1
                {
                    Name = "name3",
                    Template = new ControlTemplate
                    {
                        TargetType = typeof(Control1),
                        Template = new TemplateContent(new XamlContext(),
                            (owner, context) =>
                            {
                                Border name4 = new Border 
                                { 
                                    Name = "name4",
                                    TemplatedParent = owner,
                                };
                                Border name5 = new Border
                                {
                                    Name = "name5",
                                    TemplatedParent = owner,
                                };
                                name4.Child = name5;
                                RuntimeHelpers.XamlContext_RegisterName(context, "name4", name4);
                                RuntimeHelpers.XamlContext_RegisterName(context, "name5", name5);
                                return name4;
                            }),
                    }
                };
                name2.Children.Add(name3);

                UserControl2 name6 = new UserControl2
                {
                    Name = "name6",                    
                };
                ContentControl name7 = new ContentControl
                {
                    Name = "name7",
                };
                name6.Children.Add(name7);
                name2.Children.Add(name6);

                ItemsControl itemsControl = new ItemsControl();
                itemsControl.Items.Add("string 1");
                name2.Children.Add(itemsControl);

                Control1 control1 = new Control1
                {
                    Template = new ControlTemplate
                    {
                        TargetType = typeof(Control1),
                        Template = new TemplateContent(new XamlContext(),
                            (owner, context) =>
                            {
                                Border name8 = new Border
                                {
                                    Name = "name8",
                                    TemplatedParent = owner,
                                };
                                UserControl2 name9 = new UserControl2
                                {
                                    Name = "name9",
                                    TemplatedParent = owner,
                                };
                                ContentControl name10 = new ContentControl
                                {
                                    Name = "name10",
                                    TemplatedParent = owner,
                                };
                                name9.Children.Add(name10);
                                name8.Child = name9;

                                RuntimeHelpers.XamlContext_RegisterName(context, "name8", name8);
                                RuntimeHelpers.XamlContext_RegisterName(context, "name9", name9);
                                RuntimeHelpers.XamlContext_RegisterName(context, "name10", name10);

                                return name8;
                            })
                    }
                };
                name2.Children.Add(control1);

                Control2 control2 = new Control2
                {
                    Template = new ControlTemplate
                    {
                        TargetType = typeof(Control2),
                        Template = new TemplateContent(new XamlContext(),
                        (owner, context) =>
                        {
                            Border border = new Border
                            {
                                Name = "border",
                                TemplatedParent = owner,
                            };

                            RuntimeHelpers.XamlContext_RegisterName(context, "border", border);

                            return border;
                        })
                    },
                    Child = new ContentControl(),
                };

                name2.Children.Add(control2);

                ContentPresenter presenter = new ContentPresenter
                {
                    Content = new ContentControl(),
                };

                name2.Children.Add(presenter);

                Content = name2;                

                RuntimeHelpers.InitializeNameScope(this);
                RuntimeHelpers.RegisterName(this, "name1", this);
                RuntimeHelpers.RegisterName(this, "name2", name2);
                RuntimeHelpers.RegisterName(this, "name3", name3);
                RuntimeHelpers.RegisterName(this, "name6", name6);
                RuntimeHelpers.RegisterName(this, "name7", name7);

                name3.ApplyTemplate();
                itemsControl.ApplyTemplate();
                itemsControl.TemplateChild.ApplyTemplate();
                control1.ApplyTemplate();
                control2.ApplyTemplate();
                presenter.ApplyTemplate();

                this.name1 = this;
                this.name2 = name2;
                this.name3 = name3;
                this.name6 = name6;
                this.name7 = name7;
            }
        }

        /*
         * <UserControl x:Class="Local.UserControl2"
         *              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
         *              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
         *     <Grid x:Name="uc_name1" />
         * </UserControl>
         */
        private class UserControl2 : UserControl
        {
            internal Grid uc_name1;
            private bool _contentLoaded;

            public UserControl2()
            {
                InitializeComponent();
            }

            public UIElementCollection Children
            {
                get
                {
                    return uc_name1.Children;
                }
            }

            private void InitializeComponent()
            {
                if (_contentLoaded)
                {
                    return;
                }
                _contentLoaded = true;

                Grid grid = new Grid
                {
                    Name = "uc_name1"
                };

                Content = grid;

                RuntimeHelpers.InitializeNameScope(this);
                RuntimeHelpers.RegisterName(this, "uc_name1", grid);

                uc_name1 = grid;
            }
        }

        private class Control1 : Control
        {
            public object NotADependencyProperty { get; set; }

            public static readonly DependencyProperty ADependencyPropertyProperty =
                DependencyProperty.Register(
                    nameof(ADependencyProperty),
                    typeof(object),
                    typeof(Control1),
                    null);

            public object ADependencyProperty
            {
                get { return GetValue(ADependencyPropertyProperty); }
                set { SetValue(ADependencyPropertyProperty, value); }
            }
        }

        [ContentProperty("Child")]
        private class Control2 : Control
        {
#if MIGRATION
            public override void OnApplyTemplate()
#else
            protected override void OnApplyTemplate()
#endif
            {
                base.OnApplyTemplate();

                if (Border != null)
                {
                    Border.Child = null;
                }

                Border = GetTemplateChild("border") as Border;

                if (Border != null)
                {
                    Border.Child = Child;
                }
            }

            private Border Border { get; set; }

            public FrameworkElement Child { get; set; }
        }

        public class NotADependencyObject { }

        public class ADependencyObject : DependencyObject { }
    }
}
