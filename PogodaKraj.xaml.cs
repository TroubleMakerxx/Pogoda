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
                        // Use a service to populate your data, such as an API call.
                        // This is just dummy data for illustration.
                        WeatherCollection.Add(new Weather { Place = M, WeatherIcon = bitmapImage, Temperature = temperature.ToString() });
                        //Add other 8 cities here in similar manner

                    }

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
            //Kraj.Text = myData;
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
