﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Globalization;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pogoda
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class PogodaDetails : Page
    {
        public PogodaDetails()
        {
            this.InitializeComponent();
            LoadLoacation();
            LoadInfo();
        }


        private void Pogoda5dniaprzycisk_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        string API_KEY = ApiKey.Value;
        string Location = "Bydgoszcz";

        public void LoadLoacation()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string myData = localSettings.Values["SelectedCity"] as string;
            Location = myData;

        }

        public void LoadInfo()
        {
            using (WebClient webClient = new WebClient())
            {
                string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?q="+ Location + "&units=metric&appid=" + API_KEY);
                WeatherTodayDate weatherTodayDate = JsonConvert.DeserializeObject<WeatherTodayDate>(json);
                double feelsLike = Math.Round(weatherTodayDate.Main.FeelsLike);
                double minTemperature = Math.Round(weatherTodayDate.Main.temp_min);
                double maxTemperature = Math.Round(weatherTodayDate.Main.temp_max);
                int visibility = weatherTodayDate.Visibility;
                int cloudiness = weatherTodayDate.Clouds.All;
                int sunrise = weatherTodayDate.Sys.Sunrise;
                int sunset = weatherTodayDate.Sys.Sunset;
                // Assigning the additional instances to variables
                double temperature = Math.Round(weatherTodayDate.Main.Temp);
                string description = weatherTodayDate.Weather[0].Description;
                double pressure = weatherTodayDate.Main.Pressure;
                int humidity = weatherTodayDate.Main.Humidity;
                double windSpeed = weatherTodayDate.Wind.Speed;
                string icon = weatherTodayDate.Weather[0].Icon;
                DateTime UnixTimestampToDateTime(long _UnixTimeStamp, long correction)
                {
                    return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(_UnixTimeStamp+correction);
                }
                // Updating the UI with the additional weather information
                TemperaturaDnia.Text = temperature.ToString() + "°C";
                MinTemp.Text = "Minimalna: " + minTemperature.ToString() + "°C";
                MaxTemp.Text = "Maksymalna: " + maxTemperature.ToString() + "°C";
                Pressure.Text = "Ciśnienie: " + pressure.ToString() + " hPa  ";
                Humidity.Text = "Wilgotność: " + humidity.ToString() + "%";
                WindSpeed.Text = "Wiatr: " + windSpeed.ToString() + " m/s";
                Visibility.Text = "Widocznośc: " + visibility.ToString() + " m";
                Sunrise.Text = "Zachód: " + UnixTimestampToDateTime(sunrise, 43500).ToString("HH:mm");
                Sunset.Text = "Wschód: " + UnixTimestampToDateTime(sunset, 14900).ToString("HH:mm");
                Cloudiness.Text = "Zachmurzenie: " + cloudiness.ToString() + "%";
                FeelsLike.Text = "Odczuwalna: " + feelsLike.ToString() + "°C";
                if (icon.Contains("n"))
                {
                    icon = icon.Replace("n", "d");
                }

                string imagePath = "ms-appx:///Assets/WeatherIcons/" + icon + ".png";
                BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
                IkonkaDnia.Source = bitmapImage;



            }
        }
    }
}
