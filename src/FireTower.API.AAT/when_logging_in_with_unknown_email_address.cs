using System;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using FireTower.Presentation.Responses;
using Machine.Specifications;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_logging_in_with_unknown_email_address : given_an_api_server_context<CurrentlyDeveloping>
    {
        static Exception _exception;

        Because of =
            () =>
            _exception = Catch.Exception(() => Client.Execute<SuccessfulLoginResponse<Guid>>("/login", Method.POST,
                                                                                             new BasicLoginRequest
                                                                                                 {
                                                                                                     Email =
                                                                                                         "unknown@test.com",
                                                                                                     Password =
                                                                                                         "password"
                                                                                                 }));

        It should_return_unauthorized =
            () => _exception.ShouldBeOfExactType<AATUnauthorizedException>();
    }
}