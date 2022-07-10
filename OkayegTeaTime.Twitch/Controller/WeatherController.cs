using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using HLE.Emojis;
using HLE.Http;
using HLE.Time;
using OkayegTeaTime.Files;
using OkayegTeaTime.Files.Models;

namespace OkayegTeaTime.Twitch.Controller;

public class WeatherController
{
    private readonly Dictionary<WeatherDataKey, OpenWeatherMapResponse> _dataCache = new();
    private readonly long _cacheTime = (long)TimeSpan.FromMinutes(30).TotalMilliseconds;

    public OpenWeatherMapResponse? GetWeatherData(string city, bool loadFromCache = true)
    {
        city = city.ToLower();
        WeatherDataKey key = new(city);
        if (loadFromCache && _dataCache.TryGetValue(key, out OpenWeatherMapResponse? data) && data.TimeOfRequest + _cacheTime > TimeHelper.Now())
        {
            return data;
        }

        HttpGet request = new($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
        if (request.Result is null || !request.IsValidJsonData)
        {
            return null;
        }

        data = JsonSerializer.Deserialize<OpenWeatherMapResponse>(request.Result);
        if (data is null)
        {
            return null;
        }

        if (!_dataCache.TryAdd(key, data))
        {
            _dataCache[key] = data;
        }

        return data;
    }

    public OpenWeatherMapResponse? GetWeatherData(int latitude, int longitude, bool loadFromCache = true)
    {
        WeatherDataKey key = new(latitude, longitude);
        if (loadFromCache && _dataCache.TryGetValue(key, out OpenWeatherMapResponse? data) && data.TimeOfRequest + _cacheTime > TimeHelper.Now())
        {
            return data;
        }

        HttpGet request = new($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid={AppSettings.OpenWeatherMapApiKey}");
        if (request.Result is null || !request.IsValidJsonData)
        {
            return null;
        }

        data = JsonSerializer.Deserialize<OpenWeatherMapResponse>(request.Result);
        if (data is null)
        {
            return null;
        }

        if (!_dataCache.TryAdd(key, data))
        {
            _dataCache[key] = data;
        }

        return data;
    }

    public string ToResponse(OpenWeatherMapResponse weatherData, bool isPrivateLocation)
    {
        string location = $"{weatherData.CityName}, {weatherData.Location.Country}";
        if (isPrivateLocation)
        {
            location = "(private location)";
        }
        else if (weatherData.Location.Country.Length == 2)
        {
            string country = new RegionInfo(weatherData.Location.Country).EnglishName;
            location = $"{weatherData.CityName}, {country}";
        }

        return $"{location}: {weatherData.WeatherConditions[0].Description} {GetWeatherEmoji(weatherData.WeatherConditions[0].Id)}, {weatherData.Weather.Temperature}°C, " +
               $"min. {weatherData.Weather.MinTemperature}°C, max. {weatherData.Weather.MaxTemperature}°C, {GetDirection(weatherData.Wind.Direction)} wind speed: {weatherData.Wind.Speed} m/s, " +
               $"cloud cover: {weatherData.Clouds.Percentage}%, humidity: {weatherData.Weather.Humidity}%, air pressure: {weatherData.Weather.Pressure} hPa";
    }

    private string GetWeatherEmoji(int weatherId) =>
        weatherId switch
        {
            >= 200 and <= 231 => Emoji.CloudWithLightningAndRain,
            >= 300 and <= 321 => Emoji.CloudWithRain,
            >= 500 and <= 504 => Emoji.SunBehindRainCloud,
            511 => Emoji.CloudWithSnow,
            >= 520 and <= 531 => Emoji.CloudWithRain,
            >= 600 and <= 622 => Emoji.CloudWithSnow,
            762 => Emoji.Volcano,
            >= 701 and <= 771 => Emoji.Fog,
            781 => Emoji.Tornado,
            800 => Emoji.Sunny,
            801 => Emoji.SunBehindSmallCloud,
            802 => Emoji.SunBehindLargeCloud,
            803 or 804 => Emoji.Cloud,
            _ => Emoji.Question
        };

    private string GetDirection(double deg) =>
        deg switch
        {
            < 0 => Emoji.Question,
            < 11.25 => "N",
            < 33.75 => "NNE",
            < 56.25 => "NE",
            < 78.75 => "ENE",
            < 101.25 => "E",
            < 123.75 => "ESE",
            < 146.25 => "SE",
            < 168.75 => "SSE",
            < 191.25 => "S",
            < 213.75 => "SSW",
            < 236.25 => "SW",
            < 258.75 => "WSW",
            < 281.25 => "W",
            < 303.75 => "WNW",
            < 325.25 => "NW",
            < 348.75 => "NNW",
            <= 360 => "N",
            _ => Emoji.Question
        };

    private class WeatherDataKey
    {
        public string? City { get; }

        public int Longitude { get; }

        public int Latitude { get; }

        public WeatherDataKey(string city)
        {
            City = city;
        }

        public WeatherDataKey(int latitude, int longitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public override bool Equals(object? obj)
        {
            return obj is WeatherDataKey k && ((k.City is not null && City is not null && k.City == City) || (k.Longitude == Longitude && k.Latitude == Latitude));
        }
    }
}
