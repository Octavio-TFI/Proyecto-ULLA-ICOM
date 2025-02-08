using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Helpers.Tests
{
    [TestFixture]
    internal class StringHelper
    {
        [Test]
        [TestCase("áéíóú asd", "aeiou asd")]
        [TestCase("ÁÉÍÓÚ asd", "AEIOU asd")]
        [TestCase("áéíóú!@#", "aeiou!@#")]
        [TestCase("", "")]
        [TestCase("hello world", "hello world")]
        public void EliminarAcentosTest(string value, string expected)
        {
            // Act
            string result = value.EliminarAcentos();

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
