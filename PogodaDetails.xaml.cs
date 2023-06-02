using Newtonsoft.Json;
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
            LoadCountry();
            LoadInfo();
        }

        private void Pogoda5dniaprzycisk_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        string API_KEY = ApiKey.Value;
        string Country = "Poland";
        
        public void LoadCountry()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string myData = localSettings.Values["SelectedCountry"] as string;
            Country = myData;
            Kraj.Text = myData;
        }

        public void LoadInfo()
        {
            using (WebClient webClient = new WebClient())
            {
                string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?q="+ Country + "&units=metric&appid=" + API_KEY);
                WeatherTodayDate weatherTodayDate = JsonConvert.DeserializeObject<WeatherTodayDate>(json);

                double temperature = Math.Round(weatherTodayDate.Main.Temp);
                string description = weatherTodayDate.Weather[0].Description;
                double minTemperature = weatherTodayDate.Main.temp_min;
                double maxTemperature = weatherTodayDate.Main.temp_max;
                double pressure = weatherTodayDate.Main.Pressure;
                int humidity = weatherTodayDate.Main.Humidity;
                double windSpeed = weatherTodayDate.Wind.Speed;
                string icon = weatherTodayDate.Weather[0].Icon;

                TemperaturaDnia.Text = temperature.ToString() + "°C, " + description;
                MinMaxTemp.Text = "Min: " + minTemperature.ToString() + "°C   Max: " + maxTemperature.ToString() + "°C";
                Pressure.Text = "Pressure: " + pressure.ToString() + " hPa";
                Humidity.Text = "Humidity: " + humidity.ToString() + "%";
                WindSpeed.Text = "Wind Speed: " + windSpeed.ToString() + " m/s";

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
