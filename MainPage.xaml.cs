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
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using System.Data;
using System.Globalization;


namespace Pogoda
{
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

                CultureInfo polishCulture = new CultureInfo("pl-PL");
                DateTime currentDate = DateTime.Now.Date;

                for (int i = 0; i < weatherData.list.Count; i++)
                {
                    string date = weatherData.list[i].dt_txt;
                    DateTime dateTime = DateTime.Parse(date);

                    // Skip the current day's weather forecast
                    if (dateTime.Date == currentDate)
                        continue;

                    string day = polishCulture.DateTimeFormat.GetDayName(dateTime.DayOfWeek);

                    // Capitalize the first letter
                    day = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(day);

                    double temperature = weatherData.list[i].main.temp;
                    string temp = Math.Round(temperature).ToString() + "°";

                    string description = weatherData.list[i].weather[0].description;
                    string iconCode = weatherData.list[i].weather[0].icon;
                    string dayIconCode = iconCode.Replace("n", "d");
                    string iconUrl = $"https://openweathermap.org/img/wn/{dayIconCode}@2x.png";

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
                        WeeklyWeatherData.Add(new WeatherData { Date = day, Temperature = temp, WeatherDescription = description, WeatherIcon = iconUrl });
                    }
                }

                foreach (var kvp in temperatureDataByDay)
                {
                    double averageTemperature = kvp.Value.Average();
                    double roundedTemperature = Math.Round(averageTemperature);

                    var weatherDay = WeeklyWeatherData.FirstOrDefault(w => w.Date == kvp.Key);
                    if (weatherDay != null)
                    {
                        weatherDay.Temperature = roundedTemperature.ToString() + "°";
                    }
                }
            }
        }


    }

    public class WeatherData
    {
        public string WeatherIcon { get; set; }

        public string Date { get; set; }
        public string Temperature { get; set; }
        public string WeatherDescription { get; set; }
    }
}
