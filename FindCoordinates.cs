using System.Text.Json;

namespace APIProject
{
    public class FindCoordinates
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<(string latitude, string longitude, string cityName)> FindCoordinatess()
        {
            Console.WriteLine("Please enter a city name.");
            string cityName = Console.ReadLine();
            string requestUri = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(cityName)}&format=json&limit=1";

            //Headers
            client.DefaultRequestHeaders.Add("User-Agent", "C# App");

            var response = await client.GetStringAsync(requestUri);
            var json = JsonDocument.Parse(response);
            string latitude = string.Empty;
            string longitude = string.Empty;

            if (json.RootElement.GetArrayLength() > 0)
            {
                var location = json.RootElement[0];

                if (location.TryGetProperty("lat", out JsonElement latElement) && latElement.ValueKind == JsonValueKind.String)
                {
                    latitude = latElement.GetString();
                    Console.WriteLine($"Latitude: {latitude}");
                }

                if (location.TryGetProperty("lon", out JsonElement lonElement) && lonElement.ValueKind == JsonValueKind.String)
                {
                    longitude = lonElement.GetString();
                    Console.WriteLine($"Longitude: {longitude}");
                }
            }
            else
            {
                Console.WriteLine("City not found.");
            }

            return (latitude, longitude, cityName);
        }
    }
}
        