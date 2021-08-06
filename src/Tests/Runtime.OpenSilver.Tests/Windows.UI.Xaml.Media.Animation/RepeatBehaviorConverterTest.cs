using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Design.Serialization;

#if MIGRATION
namespace System.Windows.Media.Animation.Tests
#else
namespace Windows.UI.Xaml.Media.Animation.Tests
#endif
{
    [TestClass]
    public class RepeatBehaviorConverterTest
    {
        [TestMethod]
        public void CanConvertFrom_String_ShouldReturnTrue()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            var test = repeatBehaviorConverter.CanConvertFrom(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertFrom_Bool_ShouldReturnFalse()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            var test = repeatBehaviorConverter.CanConvertFrom(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void CanConvertTo_String_ShouldReturnTrue()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            var test = repeatBehaviorConverter.CanConvertTo(typeof(string));
            test.Should().BeTrue();
        }

        [TestMethod]
        public void CanConvertTo_Bool_ShouldReturnFalse()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            var test = repeatBehaviorConverter.CanConvertTo(typeof(bool));
            test.Should().BeFalse();
        }

        [TestMethod]
        public void ConvertFrom_Forever_ShouldReturnRepeatBehaviorForever()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            var test = repeatBehaviorConverter.ConvertFrom("Forever");
            test.Should().Be(new RepeatBehavior());
        }

        [TestMethod]
        public void ConvertFrom_Iterations_ShouldReturnRepeatBehaviorForever()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            var test = repeatBehaviorConverter.ConvertFrom("100x");
            test.Should().Be(new RepeatBehavior(100));
        }

        [TestMethod]
        public void ConvertFrom_InvalidRepeatBehavior_ShouldThrowFormatException()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            Assert.ThrowsException<FormatException>(() => repeatBehaviorConverter.ConvertFrom("100"));
        }

        [TestMethod]
        public void ConvertFrom_Null_ShouldThrow_NotSupportedException()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            Assert.ThrowsException<NotSupportedException>(() => repeatBehaviorConverter.ConvertFrom(null));
        }

        [TestMethod]
        public void ConvertFrom_Bool_ShouldThrow_NotSupportedException()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            Assert.ThrowsException<NotSupportedException>(() => repeatBehaviorConverter.ConvertFrom(true));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldReturnForever()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            var test = repeatBehaviorConverter.ConvertTo(new RepeatBehavior { Type = RepeatBehaviorType.Forever }, typeof(string));
            test.Should().Be("Forever");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldReturnIterations()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            var test = repeatBehaviorConverter.ConvertTo(new RepeatBehavior(100), typeof(string));
            test.Should().Be("100x");
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_ArgumentNullException()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            Assert.ThrowsException<ArgumentNullException>(() => repeatBehaviorConverter.ConvertTo(new RepeatBehavior(), null));
        }

        [TestMethod]
        public void ConvertTo_String_ShouldThrow_NotSupportedException()
        {
            var repeatBehaviorConverter = new RepeatBehaviorConverter();
            Assert.ThrowsException<NotSupportedException>(() => repeatBehaviorConverter.ConvertTo(true, typeof(bool)));
        }
    }
}
