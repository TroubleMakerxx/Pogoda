using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
namespace Pogoda
{

    public sealed partial class PogodaKraj : Page
    {
        public PogodaKraj()
        {
            this.InitializeComponent();
            LoadCountry();
            LoadInfo();
            this.DataContext = new ViewModel();

        }

        public class Weather
        {
            public string Place { get; set; }
            public BitmapImage WeatherIcon { get; set; }
            public string Temperature { get; set; }
        }

        public class ViewModel
        {
            public ObservableCollection<Weather> WeatherCollection { get; set; }

            public ViewModel()
            {
                WeatherCollection = new ObservableCollection<Weather>();
                PopulateData();
            }

            string[] miasta = { "Warszawa", "Poznań", "Gdańsk", "Wrocław", "Kraków", "Łódź", "Szczecin", "Bydgoszcz", "Lublin" };
            private void PopulateData()
            {
                using (WebClient webClient = new WebClient())
                {
                    string API_KEY = ApiKey.Value;
                    foreach (string M in miasta)    
                    {
                        string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?q=" + M + "&units=metric&appid=" + API_KEY);
                        WeatherTodayDate weatherTodayDate = JsonConvert.DeserializeObject<WeatherTodayDate>(json);
                        double temperature = Math.Round(weatherTodayDate.Main.Temp);

                        string icon = weatherTodayDate.Weather[0].Icon;
                        if (icon.Contains("n"))
                        {
                            icon = icon.Replace("n", "d");
                        }
                        string imagePath = "ms-appx:///Assets/WeatherIcons/" + icon + ".png";
                        BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
                        WeatherCollection.Add(new Weather { Place = M, WeatherIcon = bitmapImage, Temperature = temperature.ToString()+ "°C" });
                    }
                    string jsonPoland = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?q=Poland&units=metric&appid=" + API_KEY);
                    WeatherTodayDate weatherTodayDatePoland = JsonConvert.DeserializeObject<WeatherTodayDate>(jsonPoland);
                    double temperaturePoland = Math.Round(weatherTodayDatePoland.Main.Temp);
                    
                }

                using (WebClient webClient = new WebClient())
                {
                    string API_KEY = ApiKey.Value;
                    string M = "Poland";
                    string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?q=" + M + "&units=metric&appid=" + API_KEY);
                    WeatherTodayDate weatherTodayDate = JsonConvert.DeserializeObject<WeatherTodayDate>(json);
                    double temperature = Math.Round(weatherTodayDate.Main.Temp);

                    string icon = weatherTodayDate.Weather[0].Icon;
                    if (icon.Contains("n"))
                    {
                        icon = icon.Replace("n", "d");
                    }
                    string imagePath = "ms-appx:///Assets/WeatherIcons/" + icon + ".png";
                    BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
                }

            }
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
        }
        private void Pogoda5dniaprzycisk_Click()
        {
            Frame.Navigate(typeof(MainPage));
        }
        public void LoadInfo()
        {
            
        }
    }
}
