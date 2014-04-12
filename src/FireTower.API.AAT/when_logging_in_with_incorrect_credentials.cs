using System.Net;
using AcklenAvenue.Testing.AAT;
using Machine.Specifications;
using FireTower.Presentation.Requests;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_logging_in_with_facebook_and_unknown_user : given_an_api_server_context<CurrentlyDeveloping>
    {
        static IRestResponse _result;

        Because of =
            () => _result = Client.Execute("/login/facebook", Method.POST, new FacebookLoginRequest { FacebookId = 1817134133});

        It should_be_unauthorized =
            () => _result.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
    }
}