using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TheMovieDBBot.Configuration;

namespace TheMovieDBBot.Telegram
{
    internal class TelegramBot : TelegramBotClient, ITelegramBotInstance
    {
        public ConfigurationBot config;
        ILogger loggerProvider;

        private UpdateType[] allowedUpdates = new[] { UpdateType.Message };

        public TelegramBot(ILogger logger, IConfigurationProvider configProvider) :
            base(configProvider.GetConfiguration(new ConfigurationBotValidator())!.TelegramBotToken!)
        {
            loggerProvider = logger;
            config = configProvider.GetConfiguration(new ConfigurationBotValidator());
        }

        async public Task Run()
        {
            using var cts = new CancellationTokenSource();
            var _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = allowedUpdates,
                ThrowPendingUpdates = true,
            };

            this.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

            try
            {
                PeriodicTimer timer = new(TimeSpan.FromMilliseconds(25));
                while (await timer.WaitForNextTickAsync(cts.Token))
                {
                    await Task.Run(async () =>
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey();

                            if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.Q)
                            {
                                await cts.CancelAsync();
                                return;
                            }
                        }
                    }, cts.Token);
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {

            bool message_instance_is_not_null = update.Message != null || update.EditedMessage != null;
            bool message_text_is_not_null = update.Message?.Text != null || update.EditedMessage?.Text != null || update.Message?.Caption != null || update.EditedMessage?.Caption != null;

            if (allowedUpdates.Any(type => type.Equals(update.Type)) && message_instance_is_not_null && message_text_is_not_null)
            {

            }
        }
        private Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()

            };

            loggerProvider.Fatal(error, "Fatal error in {Bot}", nameof(TelegramBot));
            return Task.CompletedTask;
        }


    }

}
