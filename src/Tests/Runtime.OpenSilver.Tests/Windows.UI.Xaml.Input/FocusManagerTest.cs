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
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
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
    }
}
