using System;
using AcklenAvenue.Testing.Moq;
using AcklenAvenue.Testing.Nancy;
using FireTower.Presentation.Requests;
using Machine.Specifications;
using Moq;
using Nancy;
using Nancy.Testing;
using FireTower.Domain;
using FireTower.Domain.Entities;
using It = Machine.Specifications.It;

namespace FireTower.Api.Specs.Users
{
    public class when_logging_in_with_correct_credentials_but_user_is_not_activated : given_a_login_module_context
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
                    var encryptedPassword = new EncryptedPassword(_password);
                    Mock.Get(PasswordEncryptor).Setup(x => x.Encrypt(_password)).Returns(
                        encryptedPassword);
                    
                    _matching = new User
                                    {
                                        FirstName = "Byron",
                                        LastName = "Sommardahl",
                                        Name = "Byron Sommardahl",
                                        FacebookId = "123456",
                                        Email = _email,
                                        EncryptedPassword = encryptedPassword.Password,
                                        Locale = "es_ES",
                                        Username = "bsommardahl",
                                        Verified = false
                                    };

                    Mock.Get(UserSessionFactory).Setup(x => x.Create(_matching)).Returns(new UserSession
                                                                                             {Id = Guid.NewGuid()});

                    Mock.Get(ReadOnlyRepository).Setup(
                        x =>
                        x.First(
                            ThatHas.AnExpressionFor<User>().ThatMatches(_matching).ThatDoesNotMatch(new User()).Build()))
                        .Returns(_matching);
                };

        Because of =
            () => _result = Browser.PostSecureJson("/login", new BasicLoginRequest { Email = _email, Password = _password });

        It should_return_a_bad_request =
            () =>
                {
                    //_result.StatusCode.ShouldEqual(HttpStatusCode.Forbidden);
                };

        static string _email = "email";
        static string _password = "password";
    }
}