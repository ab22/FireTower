namespace FireTower.Domain.Commands
{
    public class CreateNewDisaster
    {
        public CreateNewDisaster(string locationDescription, double lat, double lng, string firstImageBase64)
        {
            LocationDescription = locationDescription;
            Latitude = lat;
            Longitude = lng;
            FirstImageBase64 = firstImageBase64;
        }

        public string LocationDescription { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string FirstImageBase64 { get; private set; }
    }
}