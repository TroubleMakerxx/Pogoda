﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System.Net;
using Windows.UI.Popups;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using System.Data;


//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Pogoda
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<WeatherData> WeeklyWeatherData { get; set; }
        private Geolocator geolocator;

        public MainPage()
        {
            this.InitializeComponent();
            geolocator = new Geolocator();
            LoadLokalizacja();
            LoadData();
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        string API_KEY = ApiKey.Value;

        double latitude;
        double longitude;
        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            double width = e.Size.Width;
            double height = e.Size.Height;

        }
            public void LoadLokalizacja()
        {
            Geoposition currentPosition = geolocator.GetGeopositionAsync().GetAwaiter().GetResult();
            latitude = currentPosition.Coordinate.Point.Position.Latitude;
            longitude = currentPosition.Coordinate.Point.Position.Longitude;
        }

        public async void wyswietl()
        {
            MessageDialog messageDialog = new MessageDialog($"http://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&appid=" + API_KEY);
            await messageDialog.ShowAsync();
        }


        private void LoadData()
        {
            using (WebClient webClient = new WebClient())
            {
                string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/forecast?lat=" + latitude + "&lon=" + longitude + "&units=metric&appid=" + API_KEY);
                WetherData weatherData = JsonConvert.DeserializeObject<WetherData>(json);
                WeeklyWeatherData = new List<WeatherData>();
                Dictionary<string, List<double>> temperatureDataByDay = new Dictionary<string, List<double>>();
                wyswietl();

                for (int i = 0; i < weatherData.list.Count; i++)
                {
                    string date = weatherData.list[i].dt_txt;
                    DateTime dateTime = DateTime.Parse(date);
                    string day = dateTime.DayOfWeek.ToString();

                    double temperature = weatherData.list[i].main.temp;
                    string temp = Math.Round(temperature).ToString() + "°C";

                    string description = weatherData.list[i].weather[0].description;

                    if (temperatureDataByDay.ContainsKey(day))
                    {
                        temperatureDataByDay[day].Add(temperature);
                    }
                    else
                    {
                        temperatureDataByDay.Add(day, new List<double> { temperature });
                    }

                    bool dayExists = WeeklyWeatherData.Any(w => w.Date == day);

                    if (!dayExists)
                    {
                        WeeklyWeatherData.Add(new WeatherData { Date = day, Temperature = temp, WeatherDescription = description });
                    }
                }

                foreach (var kvp in temperatureDataByDay)
                {
                    double averageTemperature = kvp.Value.Average();
                    double roundedTemperature = Math.Round(averageTemperature);

                    var weatherDay = WeeklyWeatherData.FirstOrDefault(w => w.Date == kvp.Key);
                    if (weatherDay != null)
                    {
                        weatherDay.Temperature = roundedTemperature.ToString() + "°C";
                    }
                }
            }
        }



    }
    public class WeatherData
    {
        public string Date { get; set; }
        public string Temperature { get; set; }
        public string WeatherDescription { get; set; }
    }
}
