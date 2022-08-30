using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenSilver;

#if MIGRATION
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

#if MIGRATION
namespace System.Windows.Input.Tests
#else
namespace Windows.UI.Xaml.Input.Tests
#endif
{
    [TestClass]
    public class FocusManagerTest
    {
        [TestMethod]
        public void GetFocusedElement()
        {
            var element = new Control();
            FocusManager.SetFocusedElement(Window.Current, element);

            FocusManager.GetFocusedElement().Should().BeSameAs(element);
        }

        [TestMethod]
        public void GetFocusedElement_WithScopeParameter()
        {
            var window = new Window();
            var otherWindow = new Window();

            var element = new Control();
            var otherElement = new Control();

            FocusManager.SetFocusedElement(window, element);

            var focusedElement = FocusManager.GetFocusedElement(window);
            focusedElement.Should().BeSameAs(element);

            FocusManager.SetFocusedElement(window, otherElement);

            focusedElement = FocusManager.GetFocusedElement(window);
            focusedElement.Should().BeSameAs(otherElement);


            FocusManager.GetFocusedElement(otherWindow).Should().BeNull();
        }

        [TestMethod]
        public void GetFocusedElement_WithNonWindowScope_ShouldReturnNull()
        {
            FocusManager.GetFocusedElement(new TextBlock()).Should().BeNull();
        }

        [TestMethod]
        public void GetFocusedElement_WithNoCurrentWindow_ShouldReturnNull()
        {
            var window = Window.Current;
            Window.Current = null;
            
            try
            {
                FocusManager.GetFocusedElement().Should().BeNull();
            }
            finally
            {
                Window.Current = window;
            }
        }

        [TestMethod]
        public void GetFocusedElement_WithNoFocusedElement_ShouldReturnNull()
        {
            var window = Window.Current;
            Window.Current = new Window();

            try
            {
                FocusManager.GetFocusedElement().Should().BeNull();
            }
            finally
            {
                Window.Current = window;
            }
        }

        [TestMethod]
        public void GetFocusedElement_ControlFocus()
        {
            using (var element = new FocusableControlWrapper<Control>(new Control()))
            {
                element.Control.Focus();
                FocusManager.GetFocusedElement().Should().Be(element.Control);
            }
        }

        [TestMethod]
        public void GetFocusedElement_TwoControls_ControlFocus()
        {
            var firstElement = new FocusableControlWrapper<Control>(new Control());
            var secondElement = new FocusableControlWrapper<Control>(new Control());

            try
            {
                firstElement.Control.Focus();
                FocusManager.GetFocusedElement().Should().Be(firstElement.Control);

                secondElement.Control.Focus();
                FocusManager.GetFocusedElement().Should().NotBe(firstElement.Control);
                FocusManager.GetFocusedElement().Should().Be(secondElement.Control);
            }
            finally
            {
                firstElement.Dispose();
                firstElement.Dispose();
            }
        }
    }
}
