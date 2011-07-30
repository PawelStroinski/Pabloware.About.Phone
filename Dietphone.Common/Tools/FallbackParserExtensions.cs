using System.Globalization;
using System.Text.RegularExpressions;

namespace Dietphone.Tools
{
    public static class FallbackParserExtensions
    {
        public static float TryGetValueOf(this float caller, string input)
        {
            var parser = new FallbackParser(input);
            if (parser.Success)
            {
                return (float)parser.Result;
            }
            else
            {
                return caller;
            }
        }

        public static short TryGetValueOf(this short caller, string input)
        {
            var parser = new FallbackParser(input);
            if (parser.Success)
            {
                return (short)parser.Result;
            }
            else
            {
                return caller;
            }
        }

        private class FallbackParser
        {
            public bool Success { get; private set; }
            public double Result { get; private set; }
            private readonly CultureInfo fallbackCulture = CultureInfo.InvariantCulture;
            private readonly NumberStyles fallbackStyle = NumberStyles.Any;
            private readonly static Regex nonNumber = new Regex(@"[^\d., '()·—E+-]");

            public FallbackParser(string input)
            {
                double result = 0;
                input = CleanNumber(input);
                if (input == string.Empty)
                {
                    Success = true;
                }
                else
                    if (double.TryParse(input, out result))
                    {
                        Success = true;
                    }
                    else
                    {
                        if (double.TryParse(input, fallbackStyle, fallbackCulture, out result))
                        {
                            Success = true;
                        }
                    }
                Result = result;
            }

            private string CleanNumber(string input)
            {
                return nonNumber.Replace(input, string.Empty);
            }
        }
    }
}