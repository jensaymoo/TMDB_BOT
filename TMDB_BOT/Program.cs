using Autofac;
using Serilog;
using TheMovieDBBot.Configuration;
using TheMovieDBBot.Telegram;

namespace TheMovieDBBot;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = new ContainerBuilder();
        ILifetimeScope scope;
        ITelegramBotInstance instance;

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:dd-MM-yyyy HH:mm:ss} {Level}] {Message} {SourceContext:l} {Exception}{NewLine}")
            .CreateLogger();

        try
        {
            builder.Register(f => Log.Logger)
                .As<ILogger>()
                .SingleInstance();

            builder.RegisterType<TelegramBot>()
                .As<ITelegramBotInstance>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ConfigurationProviderJson>()
                .As<IConfigurationProvider>()
                .InstancePerLifetimeScope();

            scope = builder.Build().BeginLifetimeScope();
            instance = scope.Resolve<ITelegramBotInstance>();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Fatal error during initialization.");
            return;
        }

        Log.Information($"{AppDomain.CurrentDomain.FriendlyName} started...");

        await instance.Run();
    }
}
