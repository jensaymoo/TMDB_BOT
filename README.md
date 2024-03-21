Telegram bot for get info about movies and TV shows from TMDB service.

To install about, you need to create a bot and set the built-in mode for it using https://t.me/botfather, as well as on the website www.themoviedb.org generate an API key to access the TMDB service.

After that, in the same directory where the executable file is located, you need to create a config configuration file `config.json` with the following contents (replace need config values):
~~~
{
  "TelegramBotToken": <Put here telegram API token>,
  "TheMovieDBToken": <Put here TMDB API token>,
  "DefaultLanguage": "ru", //ISO 639-1
  "DefaultCountry": "RU" //ISO 3166-1
}
~~~
