using System.Configuration;

namespace SkyScraper
{
    public static class ConfigurationReader
    {
        public static string GetSection(string section)
        {
            var reader = new AppSettingsReader();
            return reader.GetValue(section, typeof(string)).ToString();
        }
    }
}
