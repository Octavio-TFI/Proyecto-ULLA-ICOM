using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Helpers
{
    internal static class StringHelper
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
    }
}
