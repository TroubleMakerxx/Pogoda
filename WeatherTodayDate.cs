using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogoda
{
    internal class WeatherTodayDate
    {
        public CoordData Coord { get; set; }
        public WeatherDetails[] Weather { get; set; }
        public string Base { get; set; }
        public MainWeatherData Main { get; set; }
        public int Visibility { get; set; }
        public WindData Wind { get; set; }
        public CloudData Clouds { get; set; }
        public int Dt { get; set; }
        public SysData Sys { get; set; }
        public int Timezone { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cod { get; set; }
    }

    internal class CoordData
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
    }

    internal class WeatherDetails
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    internal class MainWeatherData
    {
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
    }

    internal class WindData
    {
        public double Speed { get; set; }
        public int Deg { get; set; }
    }

    internal class CloudData
    {
        public int All { get; set; }
    }

    internal class SysData
    {
        public int Type { get; set; }
        public int Id { get; set; }
        public string Country { get; set; }
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }
}
