using System;
using System.Globalization;
using System.Text;

namespace SestWeb.Domain.Helpers
{
    public class FractionalNumber
    {
        public double DoubleResult { get; }
        public string StringResult { get; }

        public FractionalNumber(string input)
        {
            DoubleResult = Parse(input);
            StringResult = input;
        }

        public FractionalNumber(double input)
        {
            DoubleResult = input;
            StringResult = Parse(input);
        }

        private string Parse(double input)
        {
            // get the whole value of the fraction
            double mWhole = Math.Truncate(input);

            // get the fractional value
            double mFraction = input - mWhole;

            // initialize a numerator and denomintar
            uint mNumerator = 0;
            uint mDenomenator = 1;

            // ensure that there is actual a fraction
            if (mFraction > 0)
            {
                // convert the value to a string so that you can count the number of decimal places there are
                string strFraction = mFraction.ToString().Remove(0, 2);

                // store teh number of decimal places
                uint intFractLength = (uint)strFraction.Length;

                // set the numerator to have the proper amount of zeros
                mNumerator = (uint)Math.Pow(10, intFractLength);

                // parse the fraction value to an integer that equals [fraction value] * 10^[number of decimal places]
                uint.TryParse(strFraction, out mDenomenator);

                // get the greatest common divisor for both numbers
                uint gcd = GreatestCommonDivisor(mDenomenator, mNumerator);

                // divide the numerator and the denominator by the gratest common divisor
                mNumerator = mNumerator / gcd;
                mDenomenator = mDenomenator / gcd;
            }

            // create a string builder
            StringBuilder mBuilder = new StringBuilder();

            // add the whole number if it's greater than 0
            if (mWhole > 0)
            {
                mBuilder.Append(mWhole);
            }

            // add the fraction if it's greater than 0m
            if (mFraction > 0)
            {
                if (mBuilder.Length > 0)
                {
                    mBuilder.Append(" ");
                }

                mBuilder.Append(mDenomenator);
                mBuilder.Append("/");
                mBuilder.Append(mNumerator);
            }

            return mBuilder.ToString();
        }

        private double Parse(string input)
        {
            input = (input ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input), "FractionalNumber - Input is null or empty.");
            }

            // standard decimal number (e.g. 1.125)
            if (input.IndexOf('.') != -1 || (input.IndexOf(' ') == -1 && input.IndexOf('/') == -1 && input.IndexOf('\\') == -1))
            {
                if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                {
                    return result;
                }
            }

            string[] parts = input.Split(new[] { ' ', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            // stand-off fractional (e.g. 7/8)
            if (input.IndexOf(' ') == -1 && parts.Length == 2)
            {
                if (double.TryParse(parts[0], out var num) && double.TryParse(parts[1], out var den))
                {
                    return num / den;
                }
            }

            // Number and fraction (e.g. 2 1/2)
            if (parts.Length == 3)
            {
                if (double.TryParse(parts[0], out var whole) && double.TryParse(parts[1], out var num) && double.TryParse(parts[2], out var den))
                {
                    return whole + (num / den);
                }
            }

            // Bogus / unable to parse
            return double.NaN;
        }

        public override string ToString()
        {
            return StringResult;
        }

        public static implicit operator double(FractionalNumber number)
        {
            return number.DoubleResult;
        }

        private static uint GreatestCommonDivisor(uint valA, uint valB)
        {
            // return 0 if both values are 0 (no GSD)
            if (valA == 0 &&
                valB == 0)
            {
                return 0;
            }
            // return value b if only a == 0

            if (valA == 0 &&
                valB != 0)
            {
                return valB;
            }
            // return value a if only b == 0

            if (valA != 0 && valB == 0)
            {
                return valA;
            }
            // actually find the GSD

            uint first = valA;
            uint second = valB;

            while (first != second)
            {
                if (first > second)
                {
                    first = first - second;
                }
                else
                {
                    second = second - first;
                }
            }

            return first;
        }
    }
}