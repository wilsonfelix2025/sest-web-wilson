using System;
using System.Globalization;

namespace SestWeb.Domain.Helpers
{
    public static class ConversionExtensions
    {
        public static double ToDouble(this string str)
        {
            if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new ArgumentException($"{str} não pode ser convertido para double! Função: {nameof(ToDouble)}");
        }

        public static int ToInt(this string str)
        {
            if (int.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new ArgumentException($"{str} não pode ser convertido para int! Função: {nameof(ToInt)}");
        }
    }
}