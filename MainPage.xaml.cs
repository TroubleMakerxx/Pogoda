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

        private void LoadData()
        {
            WeeklyWeatherData = new List<WeatherData>()
            {
                new WeatherData { Date = "Monday", Temperature = "25°C", WeatherDescription = "Sunny" },
                new WeatherData { Date = "Tuesday", Temperature = "22°C", WeatherDescription = "Cloudy" },
                new WeatherData { Date = "Wednesday", Temperature = "20°C", WeatherDescription = "Rainy" },
                new WeatherData { Date = "Thursday", Temperature = "24°C", WeatherDescription = "Partly Cloudy" },
                new WeatherData { Date = "Friday", Temperature = "27°C", WeatherDescription = "Sunny" },
                new WeatherData { Date = "Saturday", Temperature = "23°C", WeatherDescription = "Cloudy" },
                new WeatherData { Date = "Sunday", Temperature = "26°C", WeatherDescription = "Sunny" }
            };
        }


        private void expandButton_Click(object sender, RoutedEventArgs e)
        {
            //pokazanie Grida o nazwie sidepanel
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
