using Telegram.Bot;
namespace TMDB_BOT.Telegram;

public interface ITelegramBotInstance : ITelegramBotClient
{
    public Task Run();
}
