using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TheMovieDBBot.Configuration;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace TheMovieDBBot.Telegram
{
    internal class TelegramBot : TelegramBotClient, ITelegramBotInstance
    {
        public ConfigurationBot config;
        ILogger loggerProvider;

        private UpdateType[] allowedUpdates = new[] { UpdateType.InlineQuery };

        TMDbClient client;
        TMDbConfig client_config;

        public TelegramBot(ILogger logger, IConfigurationProvider configProvider) :
            base(configProvider.GetConfiguration(new ConfigurationBotValidator())!.TelegramBotToken!)
        {
            loggerProvider = logger;
            config = configProvider.GetConfiguration(new ConfigurationBotValidator());

            client = new TMDbClient(config.TheMovieDBToken);
            client_config = client.GetConfigAsync().Result;

            client.DefaultLanguage = config.DefaultLanguage;
            client.DefaultCountry = config.DefaultCountry;
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

            while (true)
            {
                 Thread.Sleep(60000);
            }
        }

        private async Task UpdateHandler(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            int currentPage = 0;
            int nextPage = 0;

            int.TryParse(update.InlineQuery!.Offset, out currentPage);
            try
            {
                SearchContainer<SearchMovie> results = await client.SearchMovieAsync(update.InlineQuery!.Query, page: currentPage);
                if (currentPage < (results.TotalPages - 1)) nextPage = currentPage + 1;

                var output = results.Results.Select(s => s.ToInlineSearchResult(client));
                await bot.AnswerInlineQueryAsync(update.InlineQuery!.Id, output, cacheTime: 10, nextOffset: nextPage > 0 ? nextPage.ToString() : string.Empty);
            }
            catch (Exception ex)  
            {
                loggerProvider.Error(ex, "Unknown error in {Bot}", nameof(TelegramBot));
                return;
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
