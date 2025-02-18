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
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("  hello   world  ", " hello world ")]
        [TestCase("   multiple   spaces   ", " multiple spaces ")]
        [TestCase("no extra spaces", "no extra spaces")]
        [TestCase(" leading and trailing ", " leading and trailing ")]
        [TestCase("", "")]
        public void EliminarEspaciosInnecesariosTest(
            string value,
            string expected)
        {
            // Act
            string result = value.EliminarEspaciosInnecesarios();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("line1\nline2\nline3", "line1\nline2\nline3")]
        [TestCase("line1\n\nline2\n\n\nline3", "line1\n\nline2\r\n\r\nline3")]
        [TestCase("\n\nline1\n\n", "\n\nline1\n\n")]
        [TestCase("no new lines", "no new lines")]
        [TestCase("", "")]
        public void EliminarNewLinesInnecesariasTest(
            string value,
            string expected)
        {
            // Act
            string result = value.EliminarNewLinesInnecesarias();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
