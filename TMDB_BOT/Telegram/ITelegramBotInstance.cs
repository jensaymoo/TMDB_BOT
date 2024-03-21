using Telegram.Bot;
namespace TheMovieDBBot.Telegram;

public interface ITelegramBotInstance : ITelegramBotClient
{
    public Task Run();
}
