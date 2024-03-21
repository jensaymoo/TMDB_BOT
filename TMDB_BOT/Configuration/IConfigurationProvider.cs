using FluentValidation;

namespace TheMovieDBBot.Configuration;

internal interface IConfigurationProvider
{
    public T GetConfiguration<T>(AbstractValidator<T>? validator = null);
}
