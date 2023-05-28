using System;
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

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace Pogoda
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<WeatherData> WeeklyWeatherData { get; set; }
        
        public MainPage()
        {
            this.InitializeComponent();
            LoadData();
        }
        string APIKey = "eca043052aa31fb6a7c12c7fc52357a6";
        
        private void LoadData()
        {
            using (WebClient webClient = new WebClient())
            {
                string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/forecast?q=Krakow&units=metric&appid=" + APIKey);
                WetherData weatherData = JsonConvert.DeserializeObject<WetherData>(json);
                WeeklyWeatherData = new List<WeatherData>();
                Dictionary<string, List<double>> temperatureDataByDay = new Dictionary<string, List<double>>();
                
                for (int i = 0; i < weatherData.list.Count; i++)
                {
                    string date = weatherData.list[i].dt_txt;
                    DateTime dateTime = DateTime.Parse(date);
                    string day = dateTime.DayOfWeek.ToString();
                    
                    string temp = String.Format("{0} \u00B0C", weatherData.list[i].main.temp);
                    string description = weatherData.list[i].weather[0].description;

                    if (temperatureDataByDay.ContainsKey(day))
                    {
                        temperatureDataByDay[day].Add(weatherData.list[i].main.temp);
                    }
                    else
                    {
                        temperatureDataByDay.Add(day, new List<double> { weatherData.list[i].main.temp });
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
                    double roundedTemperature = Math.Round(averageTemperature, 2);

                    var weatherDay = WeeklyWeatherData.FirstOrDefault(w => w.Date == kvp.Key);
                    if (weatherDay != null)
                    {
                        weatherDay.Temperature = String.Format("{0} \u00B0C", roundedTemperature);
                    }
                }

            }
        }



        private void expandButton_Click(object sender, RoutedEventArgs e)
        {
            if (sidepanel.Visibility == Visibility.Collapsed)
            {
                sidepanel.Visibility = Visibility.Visible;
            }
            else
            {
                sidepanel.Visibility = Visibility.Collapsed;
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
