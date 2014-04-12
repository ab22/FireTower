using System;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using FireTower.Presentation.Responses;
using Machine.Specifications;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_logging_in_with_upper_case_email_address_and_password : given_an_api_server_context<CurrentlyDeveloping>
    {
        static IRestResponse<SuccessfulLoginResponse<Guid>> _result;

        Because of =
            () =>
            _result =
            Client.Execute<SuccessfulLoginResponse<Guid>>("/login", Method.POST,
                                                          new BasicLoginRequest { Email = "TEST@test.com", Password = "password" });

        It should_return_a_token =
            () => (_result.Data ?? new SuccessfulLoginResponse<Guid>()).Token.ShouldNotEqual(Guid.Empty);
    }
}