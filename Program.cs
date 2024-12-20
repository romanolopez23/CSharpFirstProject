using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APIProject
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //Run PlayWithODS method
            var playwithodss = new PlayWithODS();
            playwithodss.ODSData();
            
            //Run RunProgram method
            await RunProgram();
        }

        static async Task RunProgram()
        {



            //Run FindCoordinates method
            var findcoordinates = new FindCoordinates();
            var coordinates = await findcoordinates.FindCoordinatess();

            //Initialize variables with values returned from FindCoordinates task.
            var latitude = coordinates.latitude;
            var longitude = coordinates.longitude;
            var cityName = coordinates.cityName;

            Console.WriteLine(cityName);

            if (string.IsNullOrEmpty(cityName) || string.IsNullOrEmpty(latitude))
            {
                Console.WriteLine("We can't run the program. Please try again.");
                await RunProgram();
                return;
            }

            //Initialize BaseUrl variable
            var BaseUrl = "https://api.open-meteo.com/v1/forecast?latitude="+latitude+"&longitude="+longitude+"&current=temperature_2m,apparent_temperature,weather_code&temperature_unit=fahrenheit&wind_speed_unit=mph&precipitation_unit=inch";

            //make http request
            using HttpClient client = new HttpClient();
            //get response message
            HttpResponseMessage response = await client.GetAsync(BaseUrl);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                Console.WriteLine(response.StatusCode);
                Console.WriteLine("We end the process");
                return;
            }
            //get json response
            var jsonresponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<WeatherData>(jsonresponse);
            var GMTDateTime = DateTime.Parse(data.Current.Time);

            var estTimeZone = TimeZoneInfo.ConvertTime(GMTDateTime,TimeZoneInfo.Utc,TimeZoneInfo.Local);
            var localtime = estTimeZone.ToString();
            var localtimezone = TimeZoneInfo.Local.ToString();
            var temperatureInCelsius = Math.Round((data.Current.Temperature2m - 32)*5/9,2);

            Console.WriteLine("\nIt is currently " + data.Current.Temperature2m + data.CurrentUnits.Temperature2m + " in " + cityName + " at " + localtime  + " " + localtimezone);
            Console.WriteLine("The temperature in Celsius is {0}°C", temperatureInCelsius);

            while (true)
            {
                Console.WriteLine("\nType 'Yes' if you want to Continue. Type 'No' if you want to exit the program");
                var userresponse = Console.ReadLine();

                if (userresponse == "Yes")
                {
                    await RunProgram();
                    break;
                }
                else if (userresponse == "No")
                {
                    Console.WriteLine("You are exiting the program");
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter either 'Yes' or 'No'");
                    continue;
                }

            }
        }


    }

    public class CurrentUnits
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("interval")]
        public string Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string Temperature2m { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public string ApparentTemperature { get; set; }

        [JsonPropertyName("weather_code")]
        public string WeatherCode { get; set; }
    }

    public class Current
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double Temperature2m { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public double ApparentTemperature { get; set; }

        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }
    }

    public class WeatherData
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; }

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        [JsonPropertyName("current_units")]
        public CurrentUnits CurrentUnits { get; set; }

        [JsonPropertyName("current")]
        public Current Current { get; set; }
    }
}