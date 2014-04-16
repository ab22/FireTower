using System;
using System.Linq;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using FireTower.ViewStore;
using Machine.Specifications;
using MongoDB.Driver.Linq;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_adding_a_photo_to_an_existing_disaster : given_an_api_server_context<CurrentlyDeveloping>
    {
        static Guid _token;
        static string _disasterId;
        static IRestResponse _result;
        static string _locationDescription;
        static int _imageCount;
        static string _imageUrl;

        Establish context =
            () =>
                {
                    _token = Login().Token;

                    int rnd = new Random().Next(999999999);
                    _locationDescription = "Santa Ana " + rnd;
                    _imageUrl = "http://www.wildlandfire.com/pics/wall/wildfire_elkbath.jpg";
                    Client.UploadFile("/disasters",
                                      _imageUrl,
                                      _token,
                                      new CreateNewDisasterRequest
                                          {
                                              LocationDescription = _locationDescription,
                                              Latitude = 123.45,
                                              Longitude = 456.32
                                          });

                    IQueryable<DisasterViewModel> disasterViewModelCollection =
                        MongoDatabase().GetCollection<DisasterViewModel>("DisasterViewModel").AsQueryable();
                    DisasterViewModel disaster =
                        disasterViewModelCollection.First(x => x.LocationDescription == _locationDescription);
                    _disasterId = disaster.DisasterId;
                    _imageCount = disaster.Images.Count();
                };

        Because of =
            () =>
            _result =
            Client.UploadFile("/disasters/" + _disasterId + "/image", _imageUrl, _token);

        It should_add_a_photo_url_to_the_disaster_model =
            () =>
                {
                    IQueryable<DisasterViewModel> disasterViewModelCollection =
                        MongoDatabase().GetCollection<DisasterViewModel>("DisasterViewModel").AsQueryable();
                    DisasterViewModel disaster =
                        disasterViewModelCollection.First(x => x.LocationDescription == _locationDescription);
                    disaster.Images.Count().ShouldEqual(_imageCount + 1);
                };

        It should_be_ok = () => _result.ShouldBeOk();
    }
}