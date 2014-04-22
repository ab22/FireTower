using System;
using System.Net;
using AcklenAvenue.Testing.AAT;
using Machine.Specifications;
using FireTower.Presentation.Requests;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_verifying_an_account_with_invalid_code : given_an_api_server_context<CurrentlyDeveloping>
    {
        static readonly Random Rnd = new Random();
        static IRestResponse _result;
        static string _email;
        static string _password;

        Establish context =
            () =>
                {
                    _email = Rnd.Next(99999) + "@test.com";

                    Client.Post("/user", new NewUserRequest
                                             {
                                                 FirstName = "Byron",
                                                 LastName = "Sommardahl",
                                                 Name = "Byron Sommardahl",
                                                 FacebookId = "1817134138",
                                                 Locale = "es_ES",
                                                 Username = "bsommardahl",
                                                 Verified = true
                                             });
                };

        Because of =
            () =>
            _result = Client.Post("/verify", new VerifyAccountRequest
                                                 {
                                                     Email = _email,
                                                     Code = "invalid"
                                                 });

        It should_be_unauthorized =
            () => _result.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
    }
}