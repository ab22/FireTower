using System;
using AcklenAvenue.Testing.Moq;
using Machine.Specifications;
using Moq;
using Nancy.Security;
using FireTower.Domain.Entities;
using FireTower.Infrastructure;
using It = Machine.Specifications.It;

namespace FireTower.Api.Specs.Users
{
    public class when_mapping_api_user_from_token : given_an_api_user_mapper_context
    {
        static readonly Guid AccessToken = Guid.NewGuid();

        static IUserIdentity _result;
        static UserSession _validClientLoginSession;

        Establish context =
            () =>
            {
                _validClientLoginSession = new UserSession { Id = AccessToken, User = new User { FacebookId = "123456" } };
                Mock.Get(_readOnlyRepo).Setup(
                    x => x.First(ThatHas.AnExpressionFor<UserSession>()
                                     .ThatMatches(_validClientLoginSession)
                                     .ThatDoesNotMatch(new UserSession())
                                     .Build()))
                    .Returns(_validClientLoginSession);
            };

        Because of =
            () => _result = _mapper.GetUserFromAccessToken(AccessToken);

        It should_return_the_expected_user =
            () => _result.ShouldBeLike(new FireTowerUserIdentity(_validClientLoginSession));
    }
}