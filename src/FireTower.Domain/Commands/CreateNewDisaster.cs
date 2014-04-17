using System;

namespace FireTower.Domain.Commands
{
    public class CreateNewDisaster
    {
        public readonly string FetchToken;
        public readonly double Latitude;
        public readonly double Longitude;
        public readonly Uri ImageUri;
        public readonly string LocationDescription;

        public CreateNewDisaster(string locationDescription, double latitude, double longitude, Uri imageUri,
                                 string fetchToken)
        {
            LocationDescription = locationDescription;
            Latitude = latitude;
            Longitude = longitude;
            ImageUri = imageUri;            
            FetchToken = fetchToken;
        }
    }
}