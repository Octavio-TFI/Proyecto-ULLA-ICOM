using AppServices.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.MesaDeAyuda.Tests
{
    internal class ConsultaDataRepositoryTests
    {
        const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<mesaDeAyuda>
 <defects>

    <defect>
      <record-id>1</record-id>
      <summary>Issue with login functionality</summary>
      <reported-by-record>
        <description>User reported that they are unable to log in</description>
      </reported-by-record>
      <defect-event>
        <event-name>Fix</event-name>
        <notes>Adjusted session timeout settings, issue resolved</notes>
      </defect-event>
      <date-entered>2016-05-01</date-entered>
    </defect>

    <defect>
      <record-id>2</record-id>
      <summary>Database connection error</summary>
      <reported-by-record>
        <description>System reports intermittent database connectivity</description>
      </reported-by-record>
      <defect-event>
        <event-name>Pre-fix</event-name>
        <notes>Checked database logs, noticed connection drops</notes>
      </defect-event>
      <defect-event>
        <event-name>Pre-fix</event-name>
        <notes>Updated connection pooling configuration</notes>
      </defect-event>
      <defect-event>
        <event-name>Fix</event-name>
        <notes>Applied patch to database driver</notes>
      </defect-event>
      <date-entered>2017-07-15</date-entered>
    </defect>

    <defect>
      <record-id>3</record-id>
      <summary>UI display bug</summary>
      <reported-by-record>
        <description>On smaller screens, the layout breaks</description>
      </reported-by-record>
      <defect-event>
        <event-name>Pre-fix</event-name>
        <notes>Reproduced issue on mobile device</notes>
      </defect-event>
      <defect-event>
        <event-name>Fix</event-name>
        <notes>Adjusted CSS for responsive design</notes>
      </defect-event>
      <date-entered>2015-11-20</date-entered>
    </defect>

    <defect>
      <record-id>4</record-id>
      <summary>UI display bug</summary>
      <reported-by-record>
        <description>On smaller screens, the layout breaks</description>
      </reported-by-record>
      <defect-event>
        <event-name>Pre-fix</event-name>
        <notes>Reproduced issue on mobile device</notes>
      </defect-event>
      <defect-event>
        <event-name>Fix</event-name>
        <notes>Adjusted CSS for responsive design</notes>
      </defect-event>
      <date-entered>2014-11-20</date-entered>
    </defect>

    <defect>
      <record-id>5</record-id>
      <summary>UI display bug</summary>
      <reported-by-record>
        <description>On smaller screens, the layout breaks</description>
      </reported-by-record>
      <defect-event>
        <event-name>Pre-fix</event-name>
        <notes>Reproduced issue on mobile device</notes>
      </defect-event>
      <date-entered>2016-08-25</date-entered>
    </defect>

    <defect>
      <record-id>6</record-id>
      <summary>Issue with login functionality</summary>
      <reported-by-record>
        <description>User reported that they are unable to log in</description>
      </reported-by-record>
      <defect-event>
        <event-name>Fix</event-name>
        <notes>Adjusted session timeout settings, issue resolved</notes>
      </defect-event>
      <date-entered>2015-01-30</date-entered>
    </defect>

    <defect>
      <record-id>sad</record-id>
      <summary>UI display bug</summary>
      <reported-by-record>
        <description>On smaller screens, the layout breaks</description>
      </reported-by-record>
      <defect-event>
        <event-name>Pre-fix</event-name>
        <notes>Reproduced issue on mobile device</notes>
      </defect-event>
      <defect-event>
        <event-name>Fix</event-name>
        <notes>Adjusted CSS for responsive design</notes>
      </defect-event>
      <date-entered>2008-12-05</date-entered>
    </defect>

  </defects>
</mesaDeAyuda>";

        [Test]
        public async Task GetAllExceptExistingIdsAsync()
        {
            // Arrange
            var fileManagerMock = new Mock<IFileManager>();

            fileManagerMock.Setup(x => x.ReadAllTextAsync("MesaDeAyuda.xml"))
                .ReturnsAsync(xml);


            var consultaDataRepository = new ConsultaDataRepository(
                fileManagerMock.Object);

            // Act
            var consultaDatas = await consultaDataRepository.GetAllExceptExistingIdsAsync(
                [6]);

            // Assert
            Assert.That(consultaDatas, Has.Count.EqualTo(3));

            var consultaData1 = consultaDatas[0];
            Assert.Multiple(
                () =>
                {
                    Assert.That(consultaData1.Id, Is.EqualTo(1));
                    Assert.That(
                        consultaData1.Titulo,
                        Is.EqualTo("Issue with login functionality"));
                    Assert.That(
                        consultaData1.Descripcion,
                        Is.EqualTo(
                                "User reported that they are unable to log in"));
                    Assert.That(
                        consultaData1.Fix,
                        Is.EqualTo(
                                "Adjusted session timeout settings, issue resolved"));
                });

            var consultaData2 = consultaDatas[1];
            Assert.Multiple(
                () =>
                {
                    Assert.That(consultaData2.Id, Is.EqualTo(2));
                    Assert.That(
                        consultaData2.Titulo,
                        Is.EqualTo("Database connection error"));
                    Assert.That(
                        consultaData2.Descripcion,
                        Is.EqualTo(
                                "System reports intermittent database connectivity"));
                    Assert.That(
                        consultaData2.PreFixes[0],
                        Is.EqualTo(
                                "Checked database logs, noticed connection drops"));
                    Assert.That(
                        consultaData2.PreFixes[1],
                        Is.EqualTo("Updated connection pooling configuration"));
                    Assert.That(
                        consultaData2.Fix,
                        Is.EqualTo("Applied patch to database driver"));
                });

            var consultaData3 = consultaDatas[2];
            Assert.Multiple(
                () =>
                {
                    Assert.That(consultaData3.Id, Is.EqualTo(3));
                    Assert.That(
                        consultaData3.Titulo,
                        Is.EqualTo("UI display bug"));
                    Assert.That(
                        consultaData3.Descripcion,
                        Is.EqualTo("On smaller screens, the layout breaks"));

                    Assert.That(
                        consultaData3.PreFixes[0],
                        Is.EqualTo("Reproduced issue on mobile device"));
                    Assert.That(
                        consultaData3.Fix,
                        Is.EqualTo("Adjusted CSS for responsive design"));
                });
        }
    }
}
