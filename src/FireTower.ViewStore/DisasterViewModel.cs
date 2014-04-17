using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace FireTower.ViewStore
{
    public class DisasterViewModel : IViewModel
    {
        public DisasterViewModel()
        {
        }

        public DisasterViewModel(Guid userId, Guid disasterId, DateTime createdDate,
                                 string locationDescription, double latitude,
                                 double longitude, string fetchToken)
        {
            DisasterId = disasterId.ToString();
            UserId = userId.ToString();
            CreatedDate = createdDate;
            LocationDescription = locationDescription;
            FetchToken = fetchToken;
            Location = new Location {Latitude = latitude, Longitude = longitude};
            SeverityVotes = new int[] {};
            Images = new string[] {};
        }

        public string DisasterId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LocationDescription { get; set; }
        public string FetchToken { get; set; }
        public Location Location { get; set; }
        public int[] SeverityVotes { get; set; }
        public string[] Images { get; set; }

        #region IViewModel Members

        public BsonObjectId Id { get; set; }

        #endregion

        public void AddImage(string imageUrl)
        {
            var images = new List<string>(Images) {imageUrl};
            Images = images.ToArray();
        }
    }
}