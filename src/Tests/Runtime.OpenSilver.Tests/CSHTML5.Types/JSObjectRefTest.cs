using CSHTML5.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Text.Json;

namespace Runtime.OpenSilver.Tests.CSHTML5.Types
{
    [TestClass]
    public class JSObjectRefTest
    {
        private static readonly JsonElement TrueJsonElement = JsonDocument.Parse("true").RootElement;

        private static readonly JsonElement StrJsonElement = JsonDocument.Parse(@"""string""").RootElement;

        private static readonly JsonElement IntegerNumberJsonElement = JsonDocument.Parse("7").RootElement;

        private static readonly JsonElement RealNumberJsonElement = JsonDocument.Parse("7.77").RootElement;

        private static readonly JsonElement NullJsonElement = JsonDocument.Parse("null").RootElement;

        private static readonly JsonElement GuidJsonElement = JsonDocument.Parse(@"""9e1948f9-4f7e-45a9-842e-1b89e3fb5c80""").RootElement;

        private static readonly JsonElement UndefinedJsonElement = default;

        private const double Delta = 0.0001;

        [TestMethod]
        public void JSObjectRefToBool()
        {
            Assert.IsTrue(JSObjectRef.ToBoolean(TrueJsonElement));
        }

        [TestMethod]
        public void JSObjectRefToString()
        {
            Assert.AreEqual(StrJsonElement.GetString(), JSObjectRef.ToString(StrJsonElement));
        }

        [TestMethod]
        public void JSObjectRefToIntegerNumber()
        {
            Assert.AreEqual(IntegerNumberJsonElement.GetByte(), JSObjectRef.ToByte(IntegerNumberJsonElement));

            Assert.AreEqual(IntegerNumberJsonElement.GetSByte(), JSObjectRef.ToSByte(IntegerNumberJsonElement));
            
            Assert.AreEqual(IntegerNumberJsonElement.GetDecimal(), JSObjectRef.ToDecimal(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt16(), JSObjectRef.ToInt16(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt32(), JSObjectRef.ToInt32(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt64(), JSObjectRef.ToInt64(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt16(), JSObjectRef.ToUInt16(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt32(), JSObjectRef.ToUInt32(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt64(), JSObjectRef.ToUInt64(IntegerNumberJsonElement));
        }

        [TestMethod]
        public void JSObjectRefToRealNumber()
        {
            Assert.AreEqual(RealNumberJsonElement.GetDouble(), JSObjectRef.ToDouble(RealNumberJsonElement), Delta);
            Assert.AreEqual(RealNumberJsonElement.GetSingle(), JSObjectRef.ToSingle(RealNumberJsonElement), Delta);
        }

        [TestMethod]
        public void IsUndefined()
        {
            Assert.IsTrue(JSObjectRef.IsUndefined(UndefinedJsonElement));
        }

        [TestMethod]
        public void IsNull()
        {
            Assert.IsTrue(JSObjectRef.IsNull(NullJsonElement));
        }

        [TestMethod]
        public void JSObjectRefToGuid()
        {
            var guid = GuidJsonElement.ToString();
            Assert.AreEqual(Guid.Parse(GuidJsonElement.ToString()), JSObjectRef.ToType(typeof(Guid), guid));
        }
    }
}
