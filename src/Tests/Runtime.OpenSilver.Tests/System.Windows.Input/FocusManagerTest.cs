using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var window = new Window();
            Window.Current = window;

            var element = new Control();

            FocusManager.SetFocusedElement(window, element);

            var focusedElement = FocusManager.GetFocusedElement();
            focusedElement.Should().BeSameAs(element);
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
            var scope = new Control();

            var focusedElement = FocusManager.GetFocusedElement(scope);
            focusedElement.Should().BeNull();
        }

        [TestMethod]
        public void GetFocusedElement_WithNoCurrentWindow_ShouldReturnNull()
        {
            Window.Current = null;
            var focusedElement = FocusManager.GetFocusedElement();
            focusedElement.Should().BeNull();
        }

        [TestMethod]
        public void GetFocusedElement_WithNoFocusedElement_ShouldReturnNull()
        {
            var window = new Window();
            Window.Current = window;
            
            var focusedElement = FocusManager.GetFocusedElement();
            focusedElement.Should().BeNull();
        }

        [TestMethod]
        public void GetFocusedElement_ControlFocus()
        {
            var window = new Window();
            Window.Current = window;

            var element = new Control();
            element.INTERNAL_ParentWindow = window;

            element.Focus();

            var focusedElement = FocusManager.GetFocusedElement();
            focusedElement.Should().Be(element);
        }

        [TestMethod]
        public void GetFocusedElement_TwoControls_ControlFocus()
        {
            var window = new Window();
            Window.Current = window;

            var firstElement = new Control();
            firstElement.INTERNAL_ParentWindow = window;

            var secondElement = new Control();
            secondElement.INTERNAL_ParentWindow = window;

            firstElement.Focus();

            var focusedElement = FocusManager.GetFocusedElement();
            focusedElement.Should().Be(firstElement);

            secondElement.Focus();
            focusedElement = FocusManager.GetFocusedElement();
            focusedElement.Should().NotBe(firstElement);
            focusedElement.Should().Be(secondElement);
        }
    }
}
