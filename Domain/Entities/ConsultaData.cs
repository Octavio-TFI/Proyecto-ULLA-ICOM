using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Datos de una consulta extraida de la base de datos de mesa de ayuda
    /// </summary>
    public record ConsultaData
    {
        /// <summary>
        /// Id de la consulta en la base de datos de mesa de ayuda
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// Titulo de la consulta
        /// </summary>
        public required string Titulo { get; init; }

        /// <summary>
        /// Descripcion de la consulta
        /// </summary>
        public string? Descripcion { get; init; }

        /// <summary>
        /// Pre-fixes de la consulta
        /// </summary>
        public required string[] PreFixes { get; init; } = [];

        /// <summary>
        /// Fix de la consulta
        /// </summary>
        public required string Fix { get; init; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {Titulo}");

            if (Descripcion is not null)
            {
                sb.AppendLine("## Descripcion");
                sb.AppendLine(Descripcion);
            }

            if (PreFixes.Length > 0)
            {
                int i = 1;

                sb.AppendLine("## Pre-Fixes");
                foreach (var preFix in PreFixes)
                {
                    sb.AppendLine($"### Pre-Fix {i++}");
                    sb.AppendLine(preFix);
                }
            }

            sb.AppendLine("## Fix");
            sb.AppendLine(Fix);

            return sb.ToString();
        }
    }
}
