using System;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using Machine.Specifications;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_creating_a_new_disaster : given_an_api_server_context<CurrentlyDeveloping>
    {
        static Guid _token;
        static IRestResponse _result;
        static string _imageUrl;

        Establish context =
            () =>
                {
                    _token = Login().Token;

                    _imageUrl = "http://www.wildlandfire.com/pics/wall/wildfire_elkbath.jpg";
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
                                      Latitude = 123.45,
                                      Longitude = 31.32
                                  });

        It should_put_a_disaster_in_the_view_model_store =
            () => { };

        It should_return_ok =
            () => _result.ShouldBeOk();
    }
}