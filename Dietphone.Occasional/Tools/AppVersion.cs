using System.Reflection;

namespace Dietphone.Tools
{
    public class AppVersion
    {
        private const byte NAME_PART_NUMBER = 0;
        private const byte VERSION_PART_NUMBER = 1;
        private const string USELESS_MINOR_VERSION = ".0.0";

        public string GetAppName()
        {
            var name = GetPartOfAssemblyName(NAME_PART_NUMBER);
            var dotParts = name.Split('.');
            return dotParts[0];
        }

        public string GetAppVersion()
        {
            var version = GetPartOfAssemblyName(VERSION_PART_NUMBER);
            var equationParts = version.Split('=');
            var numbers = equationParts[1];
            if (numbers.EndsWith(USELESS_MINOR_VERSION))
            {
                numbers = numbers.Remove(numbers.Length -
                    USELESS_MINOR_VERSION.Length, USELESS_MINOR_VERSION.Length);
            }
            return numbers;
        }

        private string GetPartOfAssemblyName(byte partNumber)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.FullName;
            var parts = name.Split(',');
            return parts[partNumber];
        }
    }
}
