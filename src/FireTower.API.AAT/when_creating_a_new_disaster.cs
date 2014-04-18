using System;
using System.Linq;
using System.Threading;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using FireTower.ViewStore;
using Machine.Specifications;
using MongoDB.Driver.Linq;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_creating_a_new_disaster : given_an_api_server_context<CurrentlyDeveloping>
    {
        static Guid _token;
        static IRestResponse _result;
        static string _imageUrl;
        static string _fetchToken;

        Establish context =
            () =>
                {
                    _token = Login().Token;

                    _imageUrl = "http://www.wildlandfire.com/pics/wall/wildfire_elkbath.jpg";

                    _fetchToken = new Random().Next(999999999).ToString();
                };

        Because of =
            () =>
            _result =
            Client.UploadFile("/disasters",
                              _imageUrl,
                              _token,
                              new CreateNewDisasterRequest
                                  {
                                      LocationDescription = "Santa Ana",
                                      Latitude = 14.083300000000001,
                                      Longitude = -87.2167,
                                      FetchToken = _fetchToken
                                  });

        It should_put_a_disaster_in_the_view_model_store =
            () =>
                {
                    DateTime startLoop = DateTime.Now;
                    DisasterViewModel disaster = null;
                    while (DateTime.Now < startLoop.AddSeconds(30))
                    {
                        disaster =
                            MongoDatabase().GetCollection<DisasterViewModel>("DisasterViewModel").AsQueryable().
                                FirstOrDefault(x => x.FetchToken == _fetchToken);

                        Console.WriteLine("Looking for viewModel with FetchToken '{0}'...", _fetchToken);
                        if (disaster != null) break;
                        Thread.Sleep(1000);
                    }
                    disaster.ShouldNotBeNull();
                };

        It should_return_ok =
            () => _result.ShouldBeOk();
    }
}