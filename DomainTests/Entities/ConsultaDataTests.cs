using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Tests
{
    class ConsultaDataTests
    {
        static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(
                    new ConsultaData
                    {
                        Id = 1,
                        Titulo = "Titulo",
                        PreFixes = [],
                        Fix = "Fix"
                    }).Returns(
                    @"# Titulo
## Fix
Fix
");

                yield return new TestCaseData(
                    new ConsultaData
                    {
                        Id = 1,
                        Titulo = "Titulo",
                        PreFixes = ["PreFix1", "PreFix2"],
                        Fix = "Fix"
                    }).Returns(
                    @"# Titulo
## Pre-Fixes
### Pre-Fix 1
PreFix1
### Pre-Fix 2
PreFix2
## Fix
Fix
");

                yield return new TestCaseData(
                    new ConsultaData
                    {
                        Id = 1,
                        Titulo = "Titulo",
                        Descripcion = "Descripcion",
                        PreFixes = [],
                        Fix = "Fix"
                    }).Returns(
                    @"# Titulo
## Descripcion
Descripcion
## Fix
Fix
");

                yield return new TestCaseData(
                    new ConsultaData
                    {
                        Id = 1,
                        Titulo = "Titulo",
                        Descripcion = "Descripcion",
                        PreFixes = ["PreFix1", "PreFix2"],
                        Fix = "Fix"
                    }).Returns(
                    @"# Titulo
## Descripcion
Descripcion
## Pre-Fixes
### Pre-Fix 1
PreFix1
### Pre-Fix 2
PreFix2
## Fix
Fix
");
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public string ToStringTest(ConsultaData consultaData)
        {
            // Act
            var result = consultaData.ToString();

            // Assert
            return result;
        }
    }
}
