using System;
using System.Net;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using FireTower.Presentation.Responses;
using Machine.Specifications;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_creating_an_account : given_an_api_server_context<CurrentlyDeveloping>
    {
        static IRestResponse _result;
        static Guid _token;

        Establish context =
            () => { _token = Login().Token; };

        Because of =
            () =>
            _result = Client.Post("/login", new NewUserRequest
                                                {
                                                    FirstName = "Byron",
                                                    LastName = "Sommardahl",
                                                    Name = "Byron Sommardahl",
                                                    FacebookId = "1817134138",
                                                    Locale = "es_ES",
                                                    Username = "bsommardahl",
                                                    Verified = true
                                                });

        It should_exist =
            () =>
            Client.Get<UserExistenceResponse>("/user/exists", new {token = _token}).Data.
                Exists.ShouldBeTrue();

        It should_success =
            () => _result.StatusCode.ShouldEqual(HttpStatusCode.OK);
    }
}