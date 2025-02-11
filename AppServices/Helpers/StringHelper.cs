using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppServices.Helpers
{
    internal static partial class StringHelper
    {
        internal static string EliminarAcentos(this string value)
        {
            return new string(
                value.Normalize(NormalizationForm.FormD)
                    .Where(
                        c => CharUnicodeInfo.GetUnicodeCategory(c) !=
                                UnicodeCategory.NonSpacingMark)
                    .ToArray());
        }

        internal static string EliminarEspaciosInnecesarios(this string value)
        {
            Regex regex = EspaciosInnecesariosRegex();

            return regex.Replace(value, " ");
        }

        [GeneratedRegex("[ ]{2,}")]
        private static partial Regex EspaciosInnecesariosRegex();

        internal static string EliminarNewLinesInnecesarias(this string value)
        {
            Regex regex = NewLinesInnecesariosRegex();
            return regex.Replace(value, Environment.NewLine);
        }

        [GeneratedRegex("(\r?\n){3,}")]
        private static partial Regex NewLinesInnecesariosRegex();
    }
}
