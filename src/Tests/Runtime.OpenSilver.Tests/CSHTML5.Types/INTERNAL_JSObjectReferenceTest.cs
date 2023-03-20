using CSHTML5.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Text.Json;

namespace Runtime.OpenSilver.Tests.CSHTML5.Types
{
    [TestClass]
    public class INTERNAL_JSObjectReferenceTest
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
        public void INTERNAL_JSObjectReferenceToBool()
        {
            Assert.IsTrue(INTERNAL_JSObjectReference.ToBoolean(TrueJsonElement));
        }

        [TestMethod]
        public void INTERNAL_JSObjectReferenceToString()
        {
            Assert.AreEqual(StrJsonElement.GetString(), INTERNAL_JSObjectReference.ToString(StrJsonElement));
        }

        [TestMethod]
        public void INTERNAL_JSObjectReferenceToIntegerNumber()
        {
            Assert.AreEqual(IntegerNumberJsonElement.GetByte(), INTERNAL_JSObjectReference.ToByte(IntegerNumberJsonElement));

            Assert.AreEqual(IntegerNumberJsonElement.GetSByte(), INTERNAL_JSObjectReference.ToSByte(IntegerNumberJsonElement));
            
            Assert.AreEqual(IntegerNumberJsonElement.GetDecimal(), INTERNAL_JSObjectReference.ToDecimal(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt16(), INTERNAL_JSObjectReference.ToInt16(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt32(), INTERNAL_JSObjectReference.ToInt32(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt64(), INTERNAL_JSObjectReference.ToInt64(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt16(), INTERNAL_JSObjectReference.ToUInt16(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt32(), INTERNAL_JSObjectReference.ToUInt32(IntegerNumberJsonElement));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt64(), INTERNAL_JSObjectReference.ToUInt64(IntegerNumberJsonElement));
        }

        [TestMethod]
        public void INTERNAL_JSObjectReferenceToRealNumber()
        {
            Assert.AreEqual(RealNumberJsonElement.GetDouble(), INTERNAL_JSObjectReference.ToDouble(RealNumberJsonElement), Delta);
            Assert.AreEqual(RealNumberJsonElement.GetSingle(), INTERNAL_JSObjectReference.ToSingle(RealNumberJsonElement), Delta);
        }

        [TestMethod]
        public void IsUndefined()
        {
            Assert.IsTrue(INTERNAL_JSObjectReference.IsUndefined(UndefinedJsonElement));
        }

        [TestMethod]
        public void IsNull()
        {
            Assert.IsTrue(INTERNAL_JSObjectReference.IsNull(NullJsonElement));
        }

        [TestMethod]
        public void INTERNAL_JSObjectReferenceToGuid()
        {
            var guid = GuidJsonElement.ToString();
            Assert.AreEqual(Guid.Parse(GuidJsonElement.ToString()), INTERNAL_JSObjectReference.ToType(typeof(Guid), guid));
        }
    }
}
