using SoporteLLM.Abstractions;
using System.Xml.Linq;

namespace SoporteLLM.Business
{
    public class MesaDeAyudaExtractor : IDataExtractor
    {
        public async Task ExtractDataAsync(
            string mesaDeAyudaFile,
            string mesaDeAyudaExtracted)
        {
            using var file = new StreamReader(mesaDeAyudaFile);

            var document = await XDocument.LoadAsync(
                file,
                LoadOptions.None,
                default);

            var defects = document.Descendants("defect");

            var finalDoc = new XDocument();
            var rootElement = new XElement("MesaDeAyuda");
            finalDoc.Add(rootElement);

            foreach (var defect in defects)
            {
                var element = new XElement("Defect");

                string? id = defect
                    .Element("record-id")?
                    .Value;

                if (id is null)
                {
                    continue;
                }

                string? summary = defect
                    .Element("summary")?
                    .Value;

                if (summary is null)
                {
                    continue;
                }

                string? description = defect
                        .Element("reported-by-record")?
                        .Element("description")?
                        .Value;

                string? fix = defect.Elements("defect-event")
                    .FirstOrDefault(
                        x => x.Element("event-name")?.Value == "Fix")?
                    .Element("notes")?.Value;

                if (fix is null)
                {
                    continue;
                }

                var preFixes = defect.Elements("defect-event")
                    .Where(
                        x => x.Element("event-name")?.Value == "Pre-fix")
                    .Select(x => x.Element("notes")?.Value ?? string.Empty)
                    .Where(x => x != string.Empty)
                    .Where(x => x != fix);

                element.Add(new XElement("Id", id));
                element.Add(new XElement("Summary", summary));

                if (description is not null)
                {
                    element.Add(
                        new XElement("Descripcion", RemoveEMailData(description)));
                }

                foreach (var preFix in preFixes)
                {
                    element.Add(new XElement("PreFix", RemoveEMailData(preFix)));
                }

                element.Add(new XElement("Fix", RemoveEMailData(fix)));

                rootElement.Add(element);
            }

            using var parsedFile = new StreamWriter(mesaDeAyudaExtracted);

            await finalDoc.SaveAsync(
                parsedFile,
                SaveOptions.None,
                default);
        }

        static string RemoveEMailData(string text)
        {
            string[] dataToRemove = ["De:", "Enviado el:", "Para:", "CC:", "Website:", "E-mail:", "Sent:", "Cc:", "To:", "From:", "Tel.:"];

            var lineasDescripcion = text.Split("\n")
                .Where(x => !dataToRemove.Any(d => x.Contains(d)));

            return string.Join("\n", lineasDescripcion);
        }
    }
}
