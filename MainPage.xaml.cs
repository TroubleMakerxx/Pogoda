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
using Windows.UI;
using System.ServiceModel.Channels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using System.Text.RegularExpressions;

namespace Pogoda
{
    public sealed partial class MainPage : Page
    {
        public List<WeatherData> WeeklyWeatherData { get; set; }
        private Geolocator geolocator;
        private ApplicationDataContainer localSettings;

        public MainPage()
        {
            this.InitializeComponent();
            geolocator = new Geolocator();
            WeeklyWeatherData = new List<WeatherData>();
            localSettings = ApplicationData.Current.LocalSettings;
            LoadSavedLocation();
            LoadSavedCountry();
            LoadData();
            LoadDayData();
            Window.Current.SizeChanged += Current_SizeChanged;

        }

        

        string API_KEY = ApiKey.Value;

        double latitude;
        double longitude;
        int status;
        string city = "Bydgoszcz";
        string Country = "Poland";

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

        private void LoadSavedCountry()
        {
            if (localSettings.Values.ContainsKey("SelectedCountry"))
            {
                string savedCountry = localSettings.Values["SelectedCountry"].ToString();
            }
        }

        private void SaveSelectedCountry(string country)
        {
            localSettings.Values["SelectedCountry"] = country;
        }

        public async void wyswietl(string json)
        {
            string message="City: " + city+ " Kraj: "+Country+" Json: "+json;
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }
        


        public void LoadDayData()
        {
            using (WebClient webClient = new WebClient())
            {
                string json;
                if (status == 1)
                {
                    json = webClient.DownloadString($"http://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid=" + API_KEY);
                }
                else
                {
                    json = webClient.DownloadString($"http://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid=" + API_KEY);
                }
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
                string country = weatherTodayDate.Sys.Country;
                json = webClient.DownloadString("https://restcountries.com/v3.1/alpha/" + country);
                string pattern = @"""common"":""([^""]*)"",";
                Match match = Regex.Match(json, pattern);
                string result = match.Groups[1].Value;
                Country = result;
                SaveSelectedCountry(Country);


            }
        }

        private void LoadData()
        {

            using (WebClient webClient = new WebClient())
            {
                string json;
                if (status == 1)
                {
                    json = webClient.DownloadString($"http://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&appid=" + API_KEY);
                }
                else
                {
                    json = webClient.DownloadString($"http://api.openweathermap.org/data/2.5/forecast?q={city}&units=metric&appid=" + API_KEY);
                }

                WetherData weatherData = JsonConvert.DeserializeObject<WetherData>(json);
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

                    BitmapImage WeatherIcon = new BitmapImage(new Uri("ms-appx:///Assets/WeatherIcons/" + iconCode + ".png"));

                    if (!dayExists)
                    {
                        WeeklyWeatherData.Add(new WeatherData { Date = day, Temperature = temperature.ToString(), WeatherDescription = description, WeatherIcon = WeatherIcon });
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
                var emptyCollection = new List<WetherData>();
                WeeklyData.ItemsSource = emptyCollection;
                WeeklyData.ItemsSource = WeeklyWeatherData;
            }
        }

        private void PogodaKrajprzycisk_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PogodaKraj));
        }

        private void LoadSavedLocation()
        {
            if (localSettings.Values.ContainsKey("SelectedLocation"))
            {
                string savedLocation = localSettings.Values["SelectedLocation"].ToString();

               
                UstawieniaLokalizacji.Content = savedLocation;

                if (savedLocation == "Location 3")
                {
                    city = "Kioto";
                    status = 2;
                    LoadData();
                    LoadDayData();
                }
                else if (savedLocation == "Lokalizacja")
                {
                    status = 1;
                    LoadLokalizacja();
                    LoadData();
                    LoadDayData();
                }
                else if (savedLocation == "Bydgosz")
                {
                    city = "Bydgoszcz";
                    status = 2;
                    LoadData();
                    LoadDayData();
                }
            }
        }

        private void UstawieniaLokalizacji_Click(object sender, RoutedEventArgs e)
        {
            StackPanel locationPanel = new StackPanel();
            locationPanel.Background = new SolidColorBrush(Colors.White);
            locationPanel.BorderBrush = new SolidColorBrush(Colors.Gray);
            locationPanel.BorderThickness = new Thickness(1);
            locationPanel.CornerRadius = new CornerRadius(4);
            locationPanel.Padding = new Thickness(8);

            // Create a ListBox to display the location options
            ListBox locationListBox = new ListBox();
            locationListBox.FontSize = 18;
            locationListBox.SelectionChanged += LocationListBox_SelectionChanged;

            // Add some sample location options to the ListBox
            locationListBox.Items.Add("Bydgosz");
            locationListBox.Items.Add("Lokalizacja");
            locationListBox.Items.Add("Location 3");

            // Add the ListBox to the panel
            locationPanel.Children.Add(locationListBox);

            // Create a Flyout to display the panel
            Flyout flyout = new Flyout();
            flyout.Content = locationPanel;

            // Attach the Flyout to the button
            flyout.ShowAt(UstawieniaLokalizacji);
        }

        public void LocationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected location
            string selectedLocation = (string)((ListBox)sender).SelectedItem;

            // Update the location text
            UstawieniaLokalizacji.Content = selectedLocation;
            
            if(selectedLocation=="Location 3")
            {
                city = "Kioto";
                status = 2;
                SaveSelectedLocation(selectedLocation);
                LoadData();
                LoadDayData();
            }
            else if (selectedLocation == "Lokalizacja")
            {
                status = 1;
                LoadLokalizacja();
                SaveSelectedLocation(selectedLocation);
                LoadData();
                LoadDayData();
            }
            else if (selectedLocation == "Bydgosz")
            {
                city = "Bydgoszcz";
                status = 2;
                SaveSelectedLocation(selectedLocation);
                LoadData();
                LoadDayData();
            }
        }
        private void SaveSelectedLocation(string location)
        {
            localSettings.Values["SelectedLocation"] = location;
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
