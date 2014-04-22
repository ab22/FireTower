using System;
using AcklenAvenue.Testing.Moq;
using AcklenAvenue.Testing.Nancy;
using FireTower.Presentation.Requests;
using Machine.Specifications;
using Moq;
using Nancy.Testing;
using FireTower.Domain;
using FireTower.Domain.Entities;
using FireTower.Presentation.Responses;
using It = Machine.Specifications.It;

namespace FireTower.Api.Specs.Users
{
    public class when_logging_in_with_correct_credentials : given_a_login_module_context
    {
        const long FacebookId = 123456;
        static readonly Guid Token = Guid.NewGuid();
        static BrowserResponse _result;
        static User _matching;
        static UserSession _userSession;
        static DateTime _expires;

        Establish context =
            () =>
                {
                    var encryptedPassword = new EncryptedPassword("password");
                    Mock.Get(PasswordEncryptor).Setup(x => x.Encrypt(_password)).Returns(
                        encryptedPassword);
                    _matching = new User
                                    {
                                        FirstName = "Byron",
                                        LastName = "Sommardahl",
                                        Name = "Byron Sommardahl",
                                        Email = _emailEmcilCom,
                                        EncryptedPassword = encryptedPassword.Password,
                                        FacebookId = "123456",
                                        Locale = "es_ES",
                                        Username = "bsommardahl",
                                        Verified = true
                                    };

                    Mock.Get(ReadOnlyRepository).Setup(
                        x =>
                        x.First(
                            ThatHas.AnExpressionFor<User>().ThatMatches(_matching).ThatDoesNotMatch(new User()).Build()))
                        .Returns(_matching);

                    _expires = DateTime.Now.Date.AddDays(10);
                    _userSession = new UserSession
                                       {
                                           Id = Token,
                                           Expires = _expires
                                       };
                    Mock.Get(UserSessionFactory).Setup(x => x.Create(_matching)).Returns(_userSession);
                };

        Because of =
            () => _result = Browser.PostSecureJson("/login", new BasicLoginRequest { Email = _emailEmcilCom, Password = _password});

        It should_return_a_token =
            () => _result.Body<SuccessfulLoginResponse<Guid>>().Token.ShouldEqual(Token);

        It should_return_an_expiration_date =
            () => _result.Body<SuccessfulLoginResponse<Guid>>().Expires.ShouldEqual(_expires);

        static string _password = "password";
        static string _emailEmcilCom = "email@emcil.com";
    }
}