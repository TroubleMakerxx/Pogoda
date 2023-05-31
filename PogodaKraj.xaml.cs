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
    public sealed partial class PogodaKraj : Page
    {
        public PogodaKraj()
        {
            this.InitializeComponent();
            LoadInfo();
        }

        private void Pogoda5dniaprzycisk_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        string API_KEY = ApiKey.Value;

        public void LoadInfo()
        {
            using (WebClient webClient = new WebClient())
            {
                string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?lat=53,1281262873261&lon=18,0118665168758&units=metric&appid=" + API_KEY);
                WeatherTodayDate weatherTodayDate = JsonConvert.DeserializeObject<WeatherTodayDate>(json);

                double temperature = Math.Round(weatherTodayDate.Main.Temp);
                string description = weatherTodayDate.Weather[0].Description;
                double minTemperature = weatherTodayDate.Main.TempMin;
                double maxTemperature = weatherTodayDate.Main.TempMax;
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
