namespace IntranetCalendarReader.Console
{
    using System.Threading.Tasks;
    using IntranetCalendar.Provider;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Options;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureHostConfiguration(config => config.AddEnvironmentVariables())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);

                    var env = hostingContext.HostingEnvironment;
                    if (env.IsDevelopment())
                    {
                        config.AddUserSecrets<Program>();
                    }

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }

                }).ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(services);
                    ConfigureHttpClients(services);

                    services.Configure<IntranetConfig>(hostContext.Configuration.GetSection("IntranetConfig"));
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                });

            await builder.RunConsoleAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddTransient<ICalendarProvider, CalendarProvider>();
            services.AddSingleton<IHostedService, CalendarReaderProcess>();
            services.AddSingleton<NltmHttpClientHandler>();
        }

        private static void ConfigureHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<IIntranetClient, IntranetClient>()
                .ConfigureHttpClient((provider, client) =>
                    client.BaseAddress = provider.GetRequiredService<IOptions<IntranetConfig>>().Value.Url)
                .ConfigurePrimaryHttpMessageHandler<NltmHttpClientHandler>();
        }
    }
}
