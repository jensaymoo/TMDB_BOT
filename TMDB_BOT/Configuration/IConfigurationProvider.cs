using FluentValidation;

namespace TMDB_BOT.Configuration;

internal interface IConfigurationProvider
{
    public T GetConfiguration<T>(AbstractValidator<T>? validator = null);
}
