
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
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver.Internal.Xaml.Context;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Effects;
using OpenSilver.Internal.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace OpenSilver.MemoryLeak.Tests;

[TestClass]
public class MemoryLeakTest
{
    [TestMethod]
    public void Element_Must_Be_Collected()
    {
        static void CreateRemoveElement(GCTracker c)
        {
            var tc = new MyFrameworkElement();
            MemoryLeaksHelper.SetTracker(tc, c);

            var mainWindow = Application.Current.MainWindow;
            mainWindow.Content = tc;
            mainWindow.Content = new Grid();
        }

        var c = new GCTracker();
        CreateRemoveElement(c);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void CoreDispatcher_Should_Release_Callback()
    {
        static void InvokeCoreDispatcher(GCTracker c)
        {
            var resetEvent = new ManualResetEvent(false);
            var trackableCallback = new ItemWithTrackableCallback(c, resetEvent);
            Dispatcher.CurrentDispatcher.BeginInvoke(trackableCallback.Callback);
            resetEvent.WaitOne(5000);
        }

        var c = new GCTracker();
        InvokeCoreDispatcher(c);
        //The next situation is possible:
        //Callback was executed, but the rest of the body in Task.Run(inside CoreDispatcher.BeginInvokeInternal) has not been completed.
        //As a result, the action in Task.Run is not collected and it has a link to ItemWithTrackableCallback.
        //That is why we need to try to collect several times. If we did not collect after 10 attempts with a delay,
        //it means that something is broken.
        for (var i = 0; i < 10; i++)
        {
            MemoryLeaksHelper.Collect();
            if (c.CollectedResetEvent.WaitOne(100))
            {
                return;
            }
        }

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void WebBrowser_Must_Be_Collected()
    {
        static void CreateRemoveWebBrowser(GCTracker c)
        {
            var tc = new WebBrowser();
            MemoryLeaksHelper.SetTracker(tc, c);

            var mainWindow = Application.Current.MainWindow;
            mainWindow.Content = tc;
            mainWindow.Content = new Grid();
        }

        var c = new GCTracker();
        CreateRemoveWebBrowser(c);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void PasswordBoxView_Must_Be_Collected()
    {
        static void CreateRemovePasswordBoxView(GCTracker tracker)
        {
            var pwbView = new PasswordBoxView(new PasswordBox());
            MemoryLeaksHelper.SetTracker(pwbView, tracker);

            Application.Current.MainWindow.Content = pwbView;
            Application.Current.MainWindow.Content = null;
        }

        var c = new GCTracker();
        CreateRemovePasswordBoxView(c);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void TextBoxView_Must_Be_Collected()
    {
        static void CreateRemoveTextBoxView(GCTracker tracker)
        {
            var tbView = new TextBoxView(new TextBox());
            MemoryLeaksHelper.SetTracker(tbView, tracker);

            Application.Current.MainWindow.Content = tbView;
            Application.Current.MainWindow.Content = null;
        }

        var c = new GCTracker();
        CreateRemoveTextBoxView(c);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void Brush_Should_Not_Keep_Owner_Alive()
    {
        static void CreateRemoveControlWithBackground(GCTracker tracker, Brush brush)
        {
            var control = new MyControl { Background = brush };
            MemoryLeaksHelper.SetTracker(control, tracker);

            Application.Current.MainWindow.Content = control;
            Application.Current.MainWindow.Content = null;
        }

        var c = new GCTracker();
        var brush = new SolidColorBrush();
        CreateRemoveControlWithBackground(c, brush);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void SizeChanged_Should_Not_Keep_FrameworkElement_Alive()
    {
        static void CreateFrameworkElement(GCTracker tracker)
        {
            var fe = new MyFrameworkElement();
            fe.SizeChanged += (o, e) => { };
            MemoryLeaksHelper.SetTracker(fe, tracker);

            Application.Current.RootVisual = fe;
            Application.Current.RootVisual = null;
        }

        var c = new GCTracker();
        CreateFrameworkElement(c);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void DependencyObject_Should_Release_InheritedContext()
    {
        static DependencyObject CreateDependencyObject(GCTracker tracker)
        {
            var depObj = new DependencyObject();
            var ctx = new MyFrameworkElement { MyProperty = depObj };
            MemoryLeaksHelper.SetTracker(ctx, tracker);
            return depObj;
        }

        var c = new GCTracker();
        CreateDependencyObject(c);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void FrameworkElement_Should_Release_TemplatedParent()
    {
        static FrameworkElement CreateFrameworkElementWithTemplateParent(GCTracker tracker)
        {
            var templatedParent = new MyControl
            {
                Template = new ControlTemplate
                {
                    TargetType = typeof(MyControl),
                    Template = new TemplateContent(
                    new XamlContext(),
                    (owner, context) => new Border { TemplatedParent = owner.AsDependencyObject() }),
                }
            };
            MemoryLeaksHelper.SetTracker(templatedParent, tracker);
            templatedParent.ApplyTemplate();
            var border = (Border)VisualTreeHelper.GetChild(templatedParent, 0);
            templatedParent.Template = null;
            return border;
        }

        var c = new GCTracker();
        var child = CreateFrameworkElementWithTemplateParent(c);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void CollectionChanged_Event_Should_Not_Keep_ItemsControl_Alive()
    {
        static void CreateItemsControl(GCTracker tracker, IEnumerable itemsSource)
        {
            var ic = new ItemsControl();
            MemoryLeaksHelper.SetTracker(ic, tracker);
            ic.ItemsSource = itemsSource;
        }

        var c = new GCTracker();
        var itemsSource = new ObservableCollection<object>();
        CreateItemsControl(c, itemsSource);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void PropertyChanged_Event_Should_Not_Keep_Binding_Target_Alive()
    {
        static void CreateBinding(GCTracker tracker, MyViewModel source)
        {
            var target = new MyFrameworkElement();
            MemoryLeaksHelper.SetTracker(target, tracker);
            var binding = new Binding("Prop1") { Source = source };
            BindingOperations.SetBinding(target, MyFrameworkElement.MyPropertyProperty, binding);

            Assert.AreEqual(target.MyProperty, source.Prop1);
        }

        var c = new GCTracker();
        var source = new MyViewModel { Prop1 = "HELLO" };
        CreateBinding(c, source);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void PropertyChanged_Event_Should_Not_Keep_Binding_Target_Alive_Indexer()
    {
        static void CreateBinding(GCTracker tracker, MyViewModel source)
        {
            var target = new MyFrameworkElement();
            MemoryLeaksHelper.SetTracker(target, tracker);
            var binding = new Binding("Prop2[2]") { Source = source };
            BindingOperations.SetBinding(target, MyFrameworkElement.MyPropertyProperty, binding);

            Assert.AreEqual((int)target.MyProperty, source.Prop2[2]);
        }

        var c = new GCTracker();
        var source = new MyViewModel { Prop1 = "HELLO", Prop2 = new ObservableCollection<int> { 1, 2, 3, 4 } };
        CreateBinding(c, source);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void ErrorsChanged_Event_Should_Not_Keep_Binding_Target_Alive()
    {
        static void CreateBinding(GCTracker tracker, MyViewModelWithValidation source)
        {
            var target = new MyFrameworkElement();
            MemoryLeaksHelper.SetTracker(target, tracker);
            var binding = new Binding("Prop1") { Source = source, ValidatesOnNotifyDataErrors = true };
            BindingOperations.SetBinding(target, MyFrameworkElement.MyPropertyProperty, binding);
        }

        var c = new GCTracker();
        var source = new MyViewModelWithValidation();
        CreateBinding(c, source);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void DependencyProperty_Listener_Should_Not_Keep_Binding_Target_Alive()
    {
        static void CreateBinding(GCTracker tracker, MyViewModel source)
        {
            var target = new MyFrameworkElement();
            MemoryLeaksHelper.SetTracker(target, tracker);
            var binding = new Binding("Prop3") { Source = source };
            BindingOperations.SetBinding(target, MyFrameworkElement.MyPropertyProperty, binding);

            Assert.AreEqual((string)target.MyProperty, (string)source.Prop3);
        }

        var c = new GCTracker();
        var source = new MyViewModel { Prop3 = "SomeValue" };
        CreateBinding(c, source);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void Effect_Should_Not_Keep_Owner_Alive()
    {
        static void CreateElementAndApplyEffect(GCTracker tracker, Effect effect)
        {
            var element = new MyControl();
            MemoryLeaksHelper.SetTracker(element, tracker);
            element.Effect = effect;
        }

        var c = new GCTracker();
        var effect = new BlurEffect();
        CreateElementAndApplyEffect(c, effect);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    [TestMethod]
    public void ForeverStoryboard_Should_Not_Keep_Target_Alive()
    {
        static void CreateElementAndStoryboard(GCTracker tracker)
        {
            var element = new MyControl();
            MemoryLeaksHelper.SetTracker(element, tracker);

            var opacityAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(1))
            };

            var storyboard = new Storyboard
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            storyboard.Children.Add(opacityAnimation);

            Storyboard.SetTarget(opacityAnimation, element);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));

            storyboard.Begin();
        }

        var c = new GCTracker();
        CreateElementAndStoryboard(c);
        MemoryLeaksHelper.Collect();

        Assert.IsTrue(c.IsCollected);
    }

    private class MyFrameworkElement : FrameworkElement
    {
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register(
                nameof(MyProperty),
                typeof(object),
                typeof(MyFrameworkElement),
                null);

        public object MyProperty
        {
            get => GetValue(MyPropertyProperty);
            set => SetValue(MyPropertyProperty, value);
        }
    }

    private class MyControl : Control { }

    private class MyViewModel : DependencyObject, INotifyPropertyChanged
    {
        private string _prop1;

        public string Prop1
        {
            get => _prop1;
            set
            {
                _prop1 = value;
                OnPropertyChanged(nameof(Prop1));
            }
        }

        private ObservableCollection<int> _prop2;

        public ObservableCollection<int> Prop2
        {
            get => _prop2;
            set
            {
                _prop2 = value;
                OnPropertyChanged(nameof(Prop2));
            }
        }

        public static readonly DependencyProperty Prop3Property =
            DependencyProperty.Register(
                nameof(Prop3),
                typeof(object),
                typeof(MyViewModel),
                null);

        public object Prop3
        {
            get => GetValue(Prop3Property);
            set => SetValue(Prop3Property, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private class MyViewModelWithValidation : MyViewModel, INotifyDataErrorInfo
    {
        public bool HasErrors => false;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName) => Enumerable.Empty<object>();
    }
}
