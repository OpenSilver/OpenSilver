using CSHTML5;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenSilver.Tests
{
    [TestClass]
    public class ReplaceArgsTest
    {
        [TestMethod]
        public void Simple_Test()
        {
            string str = "$0.style.backgroundColor = \"red\";";
            string[] args = new string[] { "document.body" };
            string expected = "document.body.style.backgroundColor = \"red\";";

            Assert.AreEqual(InteropHelper.ReplaceArgs(str, args), expected);
        }

        [TestMethod]
        public void NoArguments_Test()
        {
            string str = "document.body.style.backgroundColor = \"red\";";
            string[] args = new string[] { };

            Assert.AreEqual(InteropHelper.ReplaceArgs(str, args), str);
        }

        [TestMethod]
        public void ArgumentNotFound_Test()
        {
            string str = "$2.style.backgroundColor = \"red\";";
            string[] args = new string[] { "document.body" };

            Assert.AreEqual(InteropHelper.ReplaceArgs(str, args), str);
        }

        [TestMethod]
        public void MultipleDollars_Test()
        {
            string str = "$$$$0 $$1 $11 $";
            string[] args = new string[] { "00", "11" };
            string expected = "$$$00 $11 111 $";

            Assert.AreEqual(InteropHelper.ReplaceArgs(str, args), expected);
        }

        [TestMethod]
        public void MoreThanTen_Test()
        {
            string str = "$0 $1 $10 $11 $15 $2 $3 $4 $3 $44   $3 $5 $6 $7 $8 $10";
            string[] args = new string[] { "00", "11", "22", "33", "44", "55", "66", "77", "88", "99", "1010", "1111" };
            string expected = "00 11 1010 1111 115 22 33 44 33 444   33 55 66 77 88 1010";

            Assert.AreEqual(InteropHelper.ReplaceArgs(str, args), expected);
        }
    }
}
