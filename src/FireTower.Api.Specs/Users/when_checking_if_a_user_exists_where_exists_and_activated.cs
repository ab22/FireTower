using AcklenAvenue.Testing.Moq;
using AcklenAvenue.Testing.Nancy;
using Machine.Specifications;
using Moq;
using Nancy.Testing;
using FireTower.Domain.Entities;
using FireTower.Presentation.Responses;
using It = Machine.Specifications.It;

namespace FireTower.Api.Specs.Users
{
    public class when_checking_if_a_user_exists_where_exists_and_activated : given_a_user_module_context
    {
        static BrowserResponse _result;
        static User _user;
        static UserExistenceResponse _expectedResponse;

        Establish context =
            () =>
                {
                    _user = new User
                                {
                                    FirstName = "Byron",
                                    LastName = "Sommardahl",
                                    Name = "Byron Sommardahl",
                                    FacebookId = "123456",
                                    Locale = "es_ES",
                                    Username = "bsommardahl",
                                    Verified = true
                                };
                    Mock.Get(ReadOnlyRepository).Setup(
                        x =>
                        x.First(ThatHas.AnExpressionFor<User>().ThatMatches(_user).ThatDoesNotMatch(new User()).Build()))
                        .Returns(_user);

                    _expectedResponse = new UserExistenceResponse
                                            {
                                                Activated = _user.Verified,
                                                Exists = true
                                            };
                };

        Because of =
            () => _result = Browser.GetSecureJson("/user/exists", new { facebookId = 123456 });

        It should_return_the_expected_response =
            () => _result.Body<UserExistenceResponse>().ShouldBeLike(_expectedResponse);
    }
}