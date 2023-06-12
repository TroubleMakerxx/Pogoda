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
using Windows.UI.Text;
using Windows.UI;
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
            if (Country == "Poland")
            {
                this.DataContext = new ViewModel();
            }
            else
            {
                LoadInfo();
            }
            

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
            using (WebClient webClient = new WebClient())
            {
                string json = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?q=" + Country + "&units=metric&appid=" + API_KEY);
                WeatherTodayDate weatherTodayDate = JsonConvert.DeserializeObject<WeatherTodayDate>(json);
                double feelsLike = Math.Round(weatherTodayDate.Main.feels_like);
                double minTemperature = Math.Round(weatherTodayDate.Main.temp_min);
                double maxTemperature = Math.Round(weatherTodayDate.Main.temp_max);
                int visibility = weatherTodayDate.Visibility;
                int cloudiness = weatherTodayDate.Clouds.All;
                int sunrise = weatherTodayDate.Sys.Sunrise;
                int sunset = weatherTodayDate.Sys.Sunset;

                double temperature = Math.Round(weatherTodayDate.Main.Temp);
                string description = weatherTodayDate.Weather[0].Description;
                double pressure = weatherTodayDate.Main.Pressure;
                int humidity = weatherTodayDate.Main.Humidity;
                double windSpeed = weatherTodayDate.Wind.Speed;
                string icon = weatherTodayDate.Weather[0].Icon;

                TextBlock temperaturaDnia = new TextBlock();
                temperaturaDnia.Text = temperature.ToString() + "°C";

                TextBlock minTemp = new TextBlock();
                minTemp.Text = "Min: " + minTemperature.ToString() + "°C";

                TextBlock maxTemp = new TextBlock();
                maxTemp.Text = "Max: " + maxTemperature.ToString() + "°C";

                TextBlock feelsLikeTextBlock = new TextBlock();
                feelsLikeTextBlock.Text = "Odczuwalna: " + feelsLike.ToString() + "°C";

                TextBlock visibilityTextBlock = new TextBlock();
                visibilityTextBlock.Text = "Widoczność: " + visibility.ToString() + " m";

                TextBlock cloudinessTextBlock = new TextBlock();
                cloudinessTextBlock.Text = "Zachmurzenie: " + cloudiness.ToString() + "%";

                TextBlock pressureTextBlock = new TextBlock();
                pressureTextBlock.Text = "Ciśnienie: " + pressure.ToString() + " hPa";

                TextBlock humidityTextBlock = new TextBlock();
                humidityTextBlock.Text = "Wilgotność: " + humidity.ToString() + "%";

                TextBlock windSpeedTextBlock = new TextBlock();
                windSpeedTextBlock.Text = "Prędkość wiatru: " + windSpeed.ToString() + " m/s";

                if (icon.Contains("n"))
                {
                    icon = icon.Replace("n", "d");
                }

                string imagePath = "ms-appx:///Assets/WeatherIcons/" + icon + ".png";
                BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));

                Image Icon = new Image();
                Icon.Source = bitmapImage;

                // Create Grid and StackPanel for layout
                Grid todayGrid = new Grid();
                todayGrid.Padding = new Thickness(8);
                todayGrid.Margin = new Thickness(5);
                todayGrid.CornerRadius = new CornerRadius(16);
                Color backgroundColor = Color.FromArgb(77, 173, 216, 230); 
                SolidColorBrush backgroundBrush = new SolidColorBrush(backgroundColor);
                todayGrid.Background = backgroundBrush;
                todayGrid.HorizontalAlignment = HorizontalAlignment.Center;
                todayGrid.VerticalAlignment = VerticalAlignment.Center;

                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                todayGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                todayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                todayGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                TextBlock obecnieTextBlock = new TextBlock();
                obecnieTextBlock.Text = Country;
                obecnieTextBlock.FontSize = 50;
                obecnieTextBlock.Margin = new Thickness(0);
                obecnieTextBlock.Foreground = new SolidColorBrush(Colors.Black);
                obecnieTextBlock.FontWeight = FontWeights.Bold;
                obecnieTextBlock.VerticalAlignment = VerticalAlignment.Center;
                obecnieTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(obecnieTextBlock, 0);
                Grid.SetColumn(obecnieTextBlock, 0);
                Grid.SetColumnSpan(obecnieTextBlock, 2);

                StackPanel imageAndTemperatureStackPanel = new StackPanel();
                imageAndTemperatureStackPanel.Orientation = Orientation.Horizontal;
                imageAndTemperatureStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(imageAndTemperatureStackPanel, 1);
                Grid.SetColumn(imageAndTemperatureStackPanel, 0);
                Grid.SetColumnSpan(imageAndTemperatureStackPanel, 2);

                Image IkonkaDnia = new Image();
                IkonkaDnia.Name = "IkonkaDnia";
                IkonkaDnia.Width = 200;
                IkonkaDnia.Height = 200;
                IkonkaDnia.Margin = new Thickness(-50, -50, 50, -50);
                IkonkaDnia.HorizontalAlignment = HorizontalAlignment.Center;
                IkonkaDnia.VerticalAlignment = VerticalAlignment.Center;
                IkonkaDnia.Source = bitmapImage;

                TextBlock TemperaturaDnia = new TextBlock();
                TemperaturaDnia.Name = "TemperaturaDnia";
                TemperaturaDnia.Text = $"{temperature}°C";
                TemperaturaDnia.FontSize = 60;
                TemperaturaDnia.Foreground = new SolidColorBrush(Colors.OrangeRed);
                TemperaturaDnia.FontWeight = FontWeights.Bold;
                TemperaturaDnia.HorizontalAlignment = HorizontalAlignment.Stretch;
                TemperaturaDnia.VerticalAlignment = VerticalAlignment.Center;

                imageAndTemperatureStackPanel.Children.Add(IkonkaDnia);
                imageAndTemperatureStackPanel.Children.Add(TemperaturaDnia);

                Grid infoGrid = new Grid();
                Grid.SetRow(infoGrid, 2);

                infoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                infoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                infoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                infoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                infoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                infoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                infoGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                infoGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                infoGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

                TextBlock MinTemp = new TextBlock();
                MinTemp.Name = "MinTemp";
                MinTemp.Margin = new Thickness(10, 2, 10, 2);
                MinTemp.Text = $"Min: {minTemperature}°C";
                MinTemp.FontSize = 25;
                MinTemp.Foreground = new SolidColorBrush(Colors.Black);
                MinTemp.HorizontalAlignment = HorizontalAlignment.Center;
                MinTemp.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(MinTemp, 0);
                Grid.SetColumn(MinTemp, 0);

                TextBlock MaxTemp = new TextBlock();
                MaxTemp.Name = "MaxTemp";
                MaxTemp.Margin = new Thickness(10, 2, 10, 2);
                MaxTemp.Text = $"Max: {maxTemperature}°C";
                MaxTemp.FontSize = 25;
                MaxTemp.Foreground = new SolidColorBrush(Colors.Black);
                MaxTemp.HorizontalAlignment = HorizontalAlignment.Center;
                MaxTemp.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(MaxTemp, 0);
                Grid.SetColumn(MaxTemp, 1);

                TextBlock Pressure = new TextBlock();
                Pressure.Name = "Pressure";
                Pressure.Margin = new Thickness(10, 2, 10, 2);
                Pressure.Text = $"Ciśnienie: {pressure} hPa";
                Pressure.FontSize = 25;
                Pressure.Foreground = new SolidColorBrush(Colors.Black);
                Pressure.HorizontalAlignment = HorizontalAlignment.Center;
                Pressure.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(Pressure, 1);
                Grid.SetColumn(Pressure, 1);

                TextBlock FeelsLike = new TextBlock();
                FeelsLike.Name = "FeelsLike";
                FeelsLike.Margin = new Thickness(10, 2, 10, 2);
                FeelsLike.Text = "Odczuwalna: "+feelsLike+ "°C";
                FeelsLike.FontSize = 25;
                FeelsLike.Foreground = new SolidColorBrush(Colors.Black);
                FeelsLike.HorizontalAlignment = HorizontalAlignment.Center;
                FeelsLike.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(FeelsLike, 1);
                Grid.SetColumn(FeelsLike, 0);

                TextBlock Humidity = new TextBlock();
                Humidity.Name = "Humidity";
                Humidity.Margin = new Thickness(10, 2, 10, 2);
                Humidity.Text = $"Wilgotność: {humidity}%";
                Humidity.FontSize = 25;
                Humidity.Foreground = new SolidColorBrush(Colors.Black);
                Humidity.HorizontalAlignment = HorizontalAlignment.Center;
                Humidity.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(Humidity, 2);
                Grid.SetColumn(Humidity, 0);

                TextBlock Cloudiness = new TextBlock();
                Cloudiness.Name = "Cloudiness";
                Cloudiness.Margin = new Thickness(10, 2, 10, 2);
                Cloudiness.Text = "Zachmurzenie:"+cloudiness+"%";
                Cloudiness.FontSize = 25;
                Cloudiness.Foreground = new SolidColorBrush(Colors.Black);
                Cloudiness.HorizontalAlignment = HorizontalAlignment.Center;
                Cloudiness.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(Cloudiness, 2);
                Grid.SetColumn(Cloudiness, 1);

                TextBlock WindSpeed = new TextBlock();
                WindSpeed.Name = "WindSpeed";
                WindSpeed.Margin = new Thickness(10, 2, 10, 2);
                WindSpeed.Text = $"Prędkość wiatru: {windSpeed} m/s";
                WindSpeed.FontSize = 25;
                WindSpeed.Foreground = new SolidColorBrush(Colors.Black);
                WindSpeed.HorizontalAlignment = HorizontalAlignment.Center;
                WindSpeed.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(WindSpeed, 3);
                Grid.SetColumn(WindSpeed, 0);

                TextBlock Visibility = new TextBlock();
                Visibility.Name = "Visibility";
                Visibility.Margin = new Thickness(10, 2, 10, 2);
                Visibility.Text = $"Widoczność: {visibility} m";
                Visibility.FontSize = 25;
                Visibility.Foreground = new SolidColorBrush(Colors.Black);
                Visibility.HorizontalAlignment = HorizontalAlignment.Center;
                Visibility.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRow(Visibility, 3);
                Grid.SetColumn(Visibility, 1);

                todayGrid.Children.Add(obecnieTextBlock);
                todayGrid.Children.Add(imageAndTemperatureStackPanel);
                todayGrid.Children.Add(infoGrid);

                infoGrid.Children.Add(MinTemp);
                infoGrid.Children.Add(MaxTemp);
                infoGrid.Children.Add(Pressure);
                infoGrid.Children.Add(FeelsLike);
                infoGrid.Children.Add(Humidity);
                infoGrid.Children.Add(Cloudiness);
                infoGrid.Children.Add(WindSpeed);
                infoGrid.Children.Add(Visibility);

                WeatherStackPanel.Children.Add(todayGrid);
            }
        }

    }
}
