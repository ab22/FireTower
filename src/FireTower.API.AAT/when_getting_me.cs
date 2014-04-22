using System;
using System.Net;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Responses;
using Machine.Specifications;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_getting_me : given_an_api_server_context<CurrentlyDeveloping>
    {
        static IRestResponse<MeResponse> _result;
        static Guid _token;

        Establish context =
            () => { _token = Login().Token; };

        Because of =
            () =>
            _result =
            Client.Get<MeResponse>("/me", new {token = _token});

        It should_have_an_ok_response =
            () => _result.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_say_that_the_user_exists = () => _result.Data.ShouldBeLike(new MeResponse
                                                                                 {
                                                                                     FacebookId = "1817134138"
                                                                                 });
    }
}