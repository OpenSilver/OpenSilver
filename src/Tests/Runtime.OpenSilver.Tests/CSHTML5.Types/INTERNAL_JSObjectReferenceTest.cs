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
        public void ConvertToBool()
        {
            var objRef = new INTERNAL_JSObjectReference(TrueJsonElement);
            Assert.IsTrue((bool)objRef);
            Assert.IsTrue(Convert.ToBoolean(objRef));
        }

        [TestMethod]
        public void ConvertToString()
        {
            var objRef = new INTERNAL_JSObjectReference(StrJsonElement);
            Assert.AreEqual(StrJsonElement.GetString(), (string)objRef);
            Assert.AreEqual(StrJsonElement.GetString(), Convert.ToString(objRef, CultureInfo.InvariantCulture));
            Assert.AreEqual(StrJsonElement.GetString(), objRef.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void ConvertToIntegerNumber()
        {
            var objRef = new INTERNAL_JSObjectReference(IntegerNumberJsonElement);
            Assert.AreEqual(IntegerNumberJsonElement.GetByte(), (byte)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetByte(), Convert.ToByte(objRef));
            Assert.AreEqual(IntegerNumberJsonElement.GetSByte(), (sbyte)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetSByte(), Convert.ToSByte(objRef));
            Assert.AreEqual(IntegerNumberJsonElement.GetDecimal(), (decimal)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetDecimal(), Convert.ToDecimal(objRef));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt16(), (short)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetInt16(), Convert.ToInt16(objRef));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt32(), (int)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetInt32(), Convert.ToInt32(objRef));
            Assert.AreEqual(IntegerNumberJsonElement.GetInt64(), (long)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetInt64(), Convert.ToInt64(objRef));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt16(), (ushort)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt16(), Convert.ToUInt16(objRef));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt32(), (uint)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt32(), Convert.ToUInt32(objRef));
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt64(), (ulong)objRef);
            Assert.AreEqual(IntegerNumberJsonElement.GetUInt64(), Convert.ToUInt64(objRef));
        }

        [TestMethod]
        public void ConvertToRealNumber()
        {
            var objRef = new INTERNAL_JSObjectReference(RealNumberJsonElement);
            Assert.AreEqual(RealNumberJsonElement.GetDouble(), (double)objRef, Delta);
            Assert.AreEqual(RealNumberJsonElement.GetDouble(), Convert.ToDouble(objRef), Delta);
            Assert.AreEqual(RealNumberJsonElement.GetSingle(), (float)objRef, Delta);
            Assert.AreEqual(RealNumberJsonElement.GetSingle(), Convert.ToSingle(objRef), Delta);
        }

        [TestMethod]
        public void IsUndefined()
        {
            var objRef = new INTERNAL_JSObjectReference(UndefinedJsonElement);
            Assert.IsTrue(objRef.IsUndefined());
        }

        [TestMethod]
        public void IsNull()
        {
            var objRef = new INTERNAL_JSObjectReference(NullJsonElement);
            Assert.IsTrue(objRef.IsNull());
        }

        [TestMethod]
        public void ConvertToGuid()
        {
            var objRef = new INTERNAL_JSObjectReference(GuidJsonElement);
            Assert.AreEqual(Guid.Parse(GuidJsonElement.ToString()), Convert.ChangeType(objRef, typeof(Guid)));
        }
    }
}
