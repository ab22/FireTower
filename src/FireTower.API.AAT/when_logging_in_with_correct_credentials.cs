using System;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using FireTower.Presentation.Responses;
using Machine.Specifications;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_logging_in_with_facebook : given_an_api_server_context<CurrentlyDeveloping>
    {
        static IRestResponse<SuccessfulLoginResponse<Guid>> _result;
        static string _facebookId;

        Establish context = () => { _facebookId = RegisterUser(); };

        Because of =
            () =>
            _result =
            Client.Execute<SuccessfulLoginResponse<Guid>>("/login/facebook", Method.POST,
                                                          new FacebookLoginRequest {FacebookId = _facebookId});

        It should_return_a_token =
            () => (_result.Data ?? new SuccessfulLoginResponse<Guid>()).Token.ShouldNotEqual(Guid.Empty);
    }
}