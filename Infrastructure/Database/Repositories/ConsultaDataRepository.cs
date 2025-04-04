﻿using AppServices.Ports;
using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Infrastructure.Database.Repositories
{
    internal class ConsultaDataRepository(IFileManager fileManager)
        : IConsultaDataRepository
    {
        readonly IFileManager _fileManager = fileManager;

        public async Task<List<ConsultaData>> GetAllExceptExistingIdsAsync(
            int[] existingIds)
        {
            string mesaDeAyudaXml = await _fileManager.ReadAllTextAsync(
                "MesaDeAyuda.xml");

            using var xmlReader = new StringReader(mesaDeAyudaXml);

            var document = await XDocument.LoadAsync(
                xmlReader,
                LoadOptions.None,
                default);

            var defects = document.Descendants("defect")
                .Where(
                    defect =>
                    {
                        string? idString = defect
                        .Element("record-id")?
                        .Value;

                        return int.TryParse(idString, out int id) &&
                            !existingIds.Contains(id);
                    });

            List<ConsultaData> consultaDatas = [];

            foreach (var defect in defects)
            {
                string? dateString = defect.Element("date-entered")?.Value;

                if (!DateTime.TryParse(dateString, out DateTime date) ||
                    date.Year < 2015)
                {
                    continue;
                }

                string? idString = defect
                    .Element("record-id")?
                    .Value;

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
                    .Where(x => x.Element("event-name")?.Value == "Pre-fix")
                    .Select(x => x.Element("notes")?.Value ?? string.Empty)
                    .Where(x => x != string.Empty)
                    .Where(x => x != fix)
                    .Select(RemoveEMailData)
                    .ToArray();

                consultaDatas.Add(
                    new ConsultaData
                    {
                        Id = int.Parse(idString!),
                        Titulo = summary,
                        Descripcion =
                            description is null
                                    ? description
                                    : RemoveEMailData(description),
                        PreFixes = preFixes,
                        Fix = RemoveEMailData(fix)
                    });
            }

            return consultaDatas;
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
