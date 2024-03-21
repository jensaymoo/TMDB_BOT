using FluentValidation;

namespace TMDB_BOT.Configuration
{
    internal class ConfigurationBot
    {
        public string? TelegramBotToken { get; set; } = null;
        public string? TheMovieDBToken { get; set; } = null;

    }
    internal class ConfigurationBotValidator : AbstractValidator<ConfigurationBot>
    {
        public ConfigurationBotValidator()
        {
            RuleFor(opt => opt.TelegramBotToken)
                .NotNull()
                .NotEmpty()
                .MinimumLength(43)
                .MaximumLength(46)
                .Matches(@"^[0-9]{8,10}:[a-zA-Z0-9_-]{35}$");
        }
    }

}
