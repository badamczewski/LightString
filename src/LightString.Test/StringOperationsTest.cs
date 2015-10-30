using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightString.Test
{
    [TestClass]
    public class StringOperationsTest
    {
        [TestMethod]
        public void TheStringShouldMutate()
        {
            string nonMutableString = "Non mutable";
            var mutableString = nonMutableString.Mutate();

            Assert.ReferenceEquals(nonMutableString, mutableString.Value);
        }

        [TestMethod]
        public void TheStringShouldMutateImplicitFrom()
        {
            string nonMutableString = "Non mutable";
            MutableString mutable = nonMutableString;

            Assert.ReferenceEquals(nonMutableString, mutable);
        }

        [TestMethod]
        public void TheStringShouldMutateImplicitTo()
        {
            MutableString mutable = new MutableString("Non mutable");
            string nonMutable = mutable;


            Assert.ReferenceEquals(nonMutable, mutable);
        }

        [TestMethod]
        public void MutableStringShouldDoToUpperInPlace()
        {
            string nonMutableString = "Non mutable";
            var mutableString = nonMutableString.Mutate();

            mutableString.ToUpperInPlace();

            Assert.AreEqual(nonMutableString.ToUpper(), mutableString.Value);
        }

        [TestMethod]
        public void MutableStringShouldBeAbleToSplit()
        {
            string nonMutableString = "Non,mutable";
            var mutableString = nonMutableString.Mutate();

            var split = mutableString.SplitInPlace(',');

            Assert.IsTrue(split.Count == 2);

            var first = split.UnsafeGetStringWithCopy(0);
            var second = split.UnsafeGetStringWithCopy(1);


            Assert.AreEqual("Non", first);
            Assert.AreEqual("mutable", second);
        }

        [TestMethod]
        public void MutableStringSplitEmpty()
        {
            string nonMutableString = "";
            var mutableString = nonMutableString.Mutate();

            var split = mutableString.SplitInPlace(',');

            Assert.IsTrue(split.Count == 0);
        }
    }
}
