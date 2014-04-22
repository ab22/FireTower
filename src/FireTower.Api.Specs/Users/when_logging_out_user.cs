using System;
using AcklenAvenue.Testing.Moq;
using AcklenAvenue.Testing.Nancy;
using FireTower.Domain.Entities;
using Machine.Specifications;
using Moq;
using Nancy;
using Nancy.Testing;
using It = Machine.Specifications.It;

namespace FireTower.Api.Specs.Users
{
    public class when_logging_out_user : given_a_login_module_context
    {
        const string FacebookId = "123456";
        static readonly Guid Token = Guid.NewGuid();
        static BrowserResponse _result;
        static UserSession _matching;
        static UserSession _noMatching;

        Establish context =
            () =>
                {
                    _matching = new UserSession
                        {
                            User = new User{
                                FacebookId = FacebookId               
                            },
                            Id = Token
                   
                        };

                    _noMatching = new UserSession
                    {
                        User = new User(),
                        Id = Guid.NewGuid()

                    };

                    Mock.Get(ReadOnlyRepository).Setup(
                        x =>
                        x.First(
                            ThatHas.AnExpressionFor<UserSession>().ThatMatches(_matching).ThatDoesNotMatch(_noMatching).Build()))
                        .Returns(_matching);

                    Mock.Get(UserSessionFactory).Setup(x => x.Delete(Token));

                };

        Because of =
            () => _result = Browser.PostSecureJson("/logout", new { facebookId = FacebookId });

        It should_remove_the_user_session =
            () => _result.StatusCode.ShouldEqual(HttpStatusCode.OK);
    }
}