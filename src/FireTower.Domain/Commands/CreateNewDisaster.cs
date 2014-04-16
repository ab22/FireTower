using System.IO;

namespace FireTower.Domain.Commands
{
    public class CreateNewDisaster
    {
        public readonly string FetchToken;

        public CreateNewDisaster(string locationDescription, double lat, double lng, MemoryStream fileStream,
                                 string fetchToken)
        {
            FetchToken = fetchToken;
            LocationDescription = locationDescription;
            Latitude = lat;
            Longitude = lng;
            ImageStream = fileStream;
        }

        public string LocationDescription { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public MemoryStream ImageStream { get; private set; }
    }
}