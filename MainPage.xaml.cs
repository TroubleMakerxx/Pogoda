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

        private void SaveSelectedCity(string city)
        {
            localSettings.Values["SelectedCity"] = city;
        }

        private void AddSelection(List<string> city)
        {
            string serializedCityList = string.Join(",", city);
            localSettings.Values["SelectionCity"] = serializedCityList;
        }

        private List<string> GetSelection()
        {
            string serializedCityList = localSettings.Values["SelectionCity"] as string;
            if (string.IsNullOrEmpty(serializedCityList))
            {
                return new List<string>();
            }
            List<string> cityList = serializedCityList.Split(',').ToList();
            cityList.RemoveAll(location => location == "Warszawa" || location == "GPS" || location == "Bydgoszcz");

            return cityList;
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

        public string LoadLokalizacja()
        {
            string Spr = "OK";
            try
            {
                Geoposition currentPosition = geolocator.GetGeopositionAsync().GetAwaiter().GetResult();
                latitude = currentPosition.Coordinate.Point.Position.Latitude;
                longitude = currentPosition.Coordinate.Point.Position.Longitude;
            }
            catch (UnauthorizedAccessException ex)
            {
                Spr = "NO";
                return Spr;
            }
            return Spr;
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

        public async void wyswietl()
        {
            string message="Nieprawidłowa nazwa lokalizacji";
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        public async void WyswietlBuG()
        {
            string message = "Włącz Lokalizację";
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
                    if (dateTime.Date == currentDate)
                        continue;
                    string day = polishCulture.DateTimeFormat.GetDayName(dateTime.DayOfWeek);
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
                    double averageTemperature = kvp.Value.Max();
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

                if (savedLocation == "Warszawa")
                {
                    city = "Warszawa";
                    status = 2;
                    LoadData();
                    LoadDayData();
                }
                else if (savedLocation == "GPS")
                {
                    string spr;
                    spr = LoadLokalizacja();
                    if (spr == "NO")
                    {
                        WyswietlBuG();
                        return;
                    }
                    else
                    {
                        status = 1;
                        SaveSelectedLocation(savedLocation);
                        city = spr;
                        LoadData();
                        LoadDayData();
                    }
                }
                else if (savedLocation == "Bydgoszcz")
                {
                    city = "Bydgoszcz";
                    status = 2;
                    LoadData();
                    LoadDayData();
                }
                else
                {
                    city = UstawieniaLokalizacji.Content.ToString();
                    status = 2;
                    SaveSelectedLocation(city);
                    LoadData();
                    LoadDayData();
                }
            }
        }

        private void UstawieniaLokalizacji_Click(object sender, RoutedEventArgs e)
        {
            StackPanel locationPanel = new StackPanel();
            locationPanel.Background = new SolidColorBrush(Colors.LightBlue);
            locationPanel.BorderBrush = new SolidColorBrush(Colors.Gray);
            locationPanel.BorderThickness = new Thickness(1);
            locationPanel.CornerRadius = new CornerRadius(4);
            locationPanel.Padding = new Thickness(8);
            ListBox locationListBox = new ListBox();
            locationListBox.FontSize = 18;
            locationListBox.SelectionChanged += LocationListBox_SelectionChanged;
            locationListBox.Items.Add("Bydgoszcz");
            locationListBox.Items.Add("Warszawa");
            locationListBox.Items.Add("GPS");
            if (localSettings.Values.ContainsKey("SelectionCity"))
            {
            List<string> savedLocations = GetSelection();

            foreach (string location in savedLocations)
            {
            locationListBox.Items.Add(location);
            }
            }
            locationPanel.Children.Add(locationListBox);
            Button addButton = new Button();
            addButton.Content = "Dodaj";
            addButton.Width = 150;
            addButton.Click += AddButton_Click;
            addButton.Background = new SolidColorBrush(Colors.Green);
            addButton.Foreground = new SolidColorBrush(Colors.White);
            Button removeButton = new Button();
            removeButton.Content = "Usuń";
            removeButton.Width = 150;
            removeButton.Click += RemoveButton_Click;
            removeButton.Background = new SolidColorBrush(Colors.Red); 
            removeButton.Foreground = new SolidColorBrush(Colors.White); 
            locationPanel.Children.Add(addButton);
            locationPanel.Children.Add(removeButton);
            Flyout flyout = new Flyout();
            flyout.Content = locationPanel;
            flyout.ShowAt(UstawieniaLokalizacji);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Button addButton = (Button)sender;
            StackPanel locationPanel = (StackPanel)addButton.Parent;
            foreach (UIElement element in locationPanel.Children)
            {
                if (element is Button button)
                {
                    button.IsEnabled = false;
                    button.Visibility = Visibility.Collapsed;
                }
            }
            StackPanel enterCityPanel = new StackPanel();
            enterCityPanel.Background = new SolidColorBrush(Colors.LightBlue);
            enterCityPanel.BorderBrush = new SolidColorBrush(Colors.Gray);
            enterCityPanel.BorderThickness = new Thickness(1);
            enterCityPanel.CornerRadius = new CornerRadius(4);
            enterCityPanel.Padding = new Thickness(8);
            TextBox cityNameTextBox = new TextBox();
            cityNameTextBox.FontSize = 18;
            cityNameTextBox.Margin = new Thickness(0, 0, 0, 8);
            Button addCityButton = new Button();
            addCityButton.Content = "Dodaj";
            addCityButton.Width = 130;
            addCityButton.Background = new SolidColorBrush(Colors.Green); 
            addCityButton.Foreground = new SolidColorBrush(Colors.White);
            addCityButton.Padding = new Thickness(12);
            addCityButton.Click += (s, args) =>
            {
                string cityName = cityNameTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(cityName))
                {
                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    string spr = SprawdzLokacje(cityNameTextBox.Text);
                    if (spr == "404")
                    {
                        wyswietl();
                        return;
                    }
                    localSettings.Values[cityName] = cityNameTextBox.Text;
                    ListBox locationListBox = (ListBox)locationPanel.Children[0];
                    if (!locationListBox.Items.Contains(cityName))
                    {
                        locationListBox.Items.Add(cityName);
                    }
                    List<string> cityList = new List<string>();
                    foreach (var item in locationListBox.Items)
                    {
                        cityList.Add(item.ToString());
                    }
                    AddSelection(cityList);
                }
                locationPanel.Children.Remove(enterCityPanel);
                locationPanel.Visibility = Visibility.Visible;
                foreach (UIElement element in locationPanel.Children)
                {
                    if (element is Button button)
                    {
                        button.IsEnabled = true;
                        button.Visibility = Visibility.Visible;
                    }
                }
            };
            Button cancelButton = new Button();
            cancelButton.Content = "Anuluj";
            cancelButton.Width = 130;
            cancelButton.Background = new SolidColorBrush(Colors.Red); 
            cancelButton.Foreground = new SolidColorBrush(Colors.White); 
            cancelButton.Padding = new Thickness(12);
            cancelButton.Click += (s, args) =>
            {
                locationPanel.Children.Remove(enterCityPanel);
                locationPanel.Visibility = Visibility.Visible;
                foreach (UIElement element in locationPanel.Children)
                {
                    if (element is Button button)
                    {
                        button.IsEnabled = true;
                        button.Visibility = Visibility.Visible;
                    }
                }
            };
            enterCityPanel.Children.Add(cityNameTextBox);
            enterCityPanel.Children.Add(addCityButton);
            enterCityPanel.Children.Add(cancelButton);
            locationPanel.Children.Insert(1, enterCityPanel);
        }
        public string SprawdzLokacje(string lokacja)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    string json = webClient.DownloadString($"http://api.openweathermap.org/data/2.5/forecast?q={lokacja}&units=metric&appid={API_KEY}");
                    string pattern = @"""cod"":""([^""]*)"",";
                    Match match = Regex.Match(json, pattern);
                    string result = match.Groups[1].Value;
                    return result;
                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            return "404";
                        }
                    }
                    return "404";
                }
            }
        }
        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Button removeButton = (Button)sender;
            StackPanel locationPanel = (StackPanel)removeButton.Parent;
            ListBox locationListBox = (ListBox)locationPanel.Children[0];
            string selectedLocation = locationListBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedLocation) && !IsBasicLocation(selectedLocation))
            {
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Potwierdź usuwanie",
                    Content = $"Czy na pewno chcesz usunąć '{selectedLocation}'?",
                    PrimaryButtonText = "Usuń",
                    CloseButtonText = "Anuluj"
                };
                ContentDialogResult result = await confirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    List<string> cityList = GetSelection();
                    cityList.Remove(selectedLocation);
                    AddSelection(cityList);
                    ListBox listBox = (ListBox)locationPanel.Children[0];
                    listBox.Items.Remove(selectedLocation);
                    SaveSelectedLocation("Bydgoszcz");
                    city = "Bydgoszcz";
                    UstawieniaLokalizacji.Content = "Bydgoszcz";
                    LoadData();
                    LoadDayData();
                }
            }
        }
        private bool IsBasicLocation(string location)
        {
            List<string> basicLocations = new List<string>
        {
        "Bydgoszcz",
        "GPS",
        "Warszawa"
        };

            return basicLocations.Contains(location);
        }
        public void LocationListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedLocation = (string)((ListBox)sender).SelectedItem;
            UstawieniaLokalizacji.Content = selectedLocation;
            
            if (selectedLocation == "GPS")
            {
                string spr;
                spr=LoadLokalizacja();
                if (spr == "NO")
                {
                    WyswietlBuG();
                    return;
                }
                else
                {
                    status = 1;
                    SaveSelectedLocation(selectedLocation);
                    city = spr;
                    LoadData();
                    LoadDayData();
                }
            }
            else if (selectedLocation == "Bydgosz")
            {
                city = "Bydgoszcz";
                status = 2;
                SaveSelectedLocation(selectedLocation);
                LoadData();
                LoadDayData();
            }
            else
            {
                if (UstawieniaLokalizacji.Content != null)
                {
                    city = UstawieniaLokalizacji.Content.ToString();
                    status = 2;
                    SaveSelectedLocation(city);
                    LoadData();
                    LoadDayData();
                }

            }
        }
        public void SaveSelectedLocation(string location)
        {
            localSettings.Values["SelectedLocation"] = location;
        }
        private void SzczegulyPogody_Click(object sender, RoutedEventArgs e)
        {
            SaveSelectedCity(city);
            Frame.Navigate(typeof(Pogoda.PogodaDetails));
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
