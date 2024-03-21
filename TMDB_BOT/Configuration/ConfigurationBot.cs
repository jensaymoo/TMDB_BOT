using FluentValidation;

namespace TheMovieDBBot.Configuration
{
    internal class ConfigurationBot
    {
        public string? TelegramBotToken { get; set; } = null;
        public string? TheMovieDBToken { get; set; } = null;
        public string? DefaultLanguage { get; set; } = null;
        public string? DefaultCountry { get; set; } = null;


    }
    internal class ConfigurationBotValidator : AbstractValidator<ConfigurationBot>
    {
        public ConfigurationBotValidator()
        {
            RuleFor(opt => opt.TelegramBotToken)
                .NotEmpty()
                .MinimumLength(43)
                .MaximumLength(46)
                .Matches(@"^[0-9]{8,10}:[a-zA-Z0-9_-]{35}$");

            RuleFor(opt => opt.TheMovieDBToken)
                .NotEmpty();

        }
    }

}
