using System.Text;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

namespace TheMovieDBBot;

internal static class Extensions
{
    public static InlineQueryResultArticle ToInlineSearchResult(this SearchMovie movie, TMDbClient client)
    {
        var message = new StringBuilder();
        message.AppendLine($"<b>{movie.Title} - {movie.ReleaseDate?.Year}</b>");
        message.AppendLine();
        message.AppendLine($"{movie.Overview}");
        message.AppendLine();

        if (movie.GenreIds.Any())
        {
            var genres = client.GetMovieGenresAsync().Result
                .IntersectBy(movie.GenreIds, x => x.Id)
                .Select(s => s.Name)
                .Aggregate((first, second) => first + ", " + second);

            message.AppendLine($"Жанр: {genres}");
        }

        message.AppendLine($"Рейтинг: {Math.Round(movie.VoteAverage, 1)} ({movie.VoteCount} голосов)");
        message.AppendLine($"ID: {movie.Id}");
        message.AppendLine();

        switch (movie.MediaType)
        {
            case TMDbLib.Objects.General.MediaType.Movie:
                message.AppendLine("https://www.themoviedb.org/movie/" + movie.Id);
                break;
        }

        var output = new InlineQueryResultArticle(movie.Id.ToString(), $"{movie.Title} - {movie.ReleaseDate?.Year}",
        new InputTextMessageContent(message.ToString()) { ParseMode = ParseMode.Html, DisableWebPagePreview = false })
        {
            ThumbnailUrl = client.GetImageUrl("w92", movie.PosterPath, true).AbsoluteUri,
            Description = movie.Overview,
        };

        return output;
    }
}
