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
using Windows.UI.Xaml.Media.Imaging;

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
            LoadDayData();
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
        
        public void LoadDayData()
        {
            using (WebClient webClient = new WebClient())
            {
                string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?lat=53,1281262873261&lon=18,0118665168758&units=metric&appid="+ API_KEY);
                WeatherTodayDate weatherTodayDate= JsonConvert.DeserializeObject<WeatherTodayDate>(json);

                double temperature = Math.Round(weatherTodayDate.Main.Temp);
                string icon = "";
                if (weatherTodayDate.Weather.Length > 0)
                {
                    icon = weatherTodayDate.Weather[0].Icon;
                }
                TemperaturaDnia.Text = temperature.ToString()+ "°C";
                if (icon.Contains("n"))
                {
                    icon = icon.Replace("n", "d");
                }
                string imagePath = "ms-appx:///Assets/WeatherIcons/"+icon+".png";
                BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
                IkonkaDnia.Source = bitmapImage;



            }
        }

        private void LoadData()
        {
            using (WebClient webClient = new WebClient())
            {
                string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/forecast?lat=" + latitude + "&lon=" + longitude + "&units=metric&appid=" + API_KEY);
                WetherData weatherData = JsonConvert.DeserializeObject<WetherData>(json);
                WeeklyWeatherData = new List<WeatherData>();
                Dictionary<string, List<double>> temperatureDataByDay = new Dictionary<string, List<double>>();
                Dictionary<string, Dictionary<string, int>> iconCountByDay = new Dictionary<string, Dictionary<string, int>>();
                
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
                    string temp = Math.Round(temperature).ToString() + "°C";

                    string description = weatherData.list[i].weather[0].description;
                    string iconCode = weatherData.list[i].weather[0].icon;
                    if (iconCode.Contains("n"))
                    {
                        iconCode = iconCode.Replace("n", "d");
                    }
                    
                    if (!iconCountByDay.ContainsKey(day))
                    {
                        iconCountByDay.Add(day, new Dictionary<string, int>());
                    }

                    if (iconCountByDay[day].ContainsKey(iconCode))
                    {
                        iconCountByDay[day][iconCode]++;
                    }
                    else
                    {
                        iconCountByDay[day].Add(iconCode, 1);
                    }

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

                foreach (var kvp in iconCountByDay)
                {
                    string day = kvp.Key;
                    string mostFrequentIcon = kvp.Value.OrderByDescending(x => x.Value).FirstOrDefault().Key;

                    // Update the icon for the corresponding day in the WeeklyWeatherData list
                    var weatherDay = WeeklyWeatherData.FirstOrDefault(w => w.Date == day);
                    if (weatherDay != null)
                    {
                        weatherDay.WeatherIcon = new BitmapImage(new Uri("ms-appx:///Assets/WeatherIcons/" + mostFrequentIcon + ".png"));
                    }
                }
            }
        }

        private void PogodaKrajprzycisk_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PogodaKraj));
        }
    }

    public class WeatherData
    {
        public BitmapImage WeatherIcon { get; set; }

        public string Date { get; set; }
        public string Temperature { get; set; }
        public string WeatherDescription { get; set; }
    }
}
