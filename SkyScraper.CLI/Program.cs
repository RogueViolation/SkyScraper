using SkyScraper.Interface;

namespace SkyScraper.CLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            IPrinter printer = new Printer();
            IConfigurationReader configurationReader = new ConfigurationReader();
            ISkyParser skyParser = new SkyParser();

            new SkyWorkflow(configurationReader, printer, skyParser).Start();
        }
    }
}
