using Autofac;
using ContentLibrary.UI.DI;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ContentLibrary.UI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.InputEncoding = Encoding.GetEncoding("Windows-1251");

            var configuration = ConfigureAppSettings();

            // Використання нової DI конфігурацію з Autofac
            using (var container = DependencyConfig.Configure(configuration))
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var app = scope.Resolve<ConsoleApplication>();
                    if (app != null)
                    {
                        app.Run();
                    }
                    else
                    {
                        Console.WriteLine("Помилка ініціалізації додатку.");
                    }
                }
            }
        }

        private static IConfiguration ConfigureAppSettings()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}