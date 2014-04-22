using System;
using AcklenAvenue.Testing.Moq;
using AcklenAvenue.Testing.Nancy;
using Machine.Specifications;
using Moq;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Presentation;
using FireTower.Presentation.Requests;
using It = Machine.Specifications.It;

namespace FireTower.Api.Specs.Users
{
    public class when_attempting_to_create_a_new_user_account_where_one_already_exists_with_the_same_email :
        given_a_new_user_module_context
    {
        static NewUserRequest _request;
        static User _existingUser;
        static Exception _exception;

        Establish context =
            () =>
                {
                    _request = new NewUserRequest
                                   {
                                       FirstName = "Byron",
                                       LastName = "Sommardahl",
                                       Name = "Byron Sommardahl",
                                       FacebookId = "1817134138",
                                       Locale = "es_ES",
                                       Username = "bsommardahl",
                                       Verified = true
                                   };

                    _existingUser = new User {FacebookId = _request.FacebookId};
                    Mock.Get(ReadOnlyRepository).Setup(
                        x =>
                        x.First(
                            ThatHas.AnExpressionFor<User>().ThatMatches(_existingUser).
                                ThatDoesNotMatch(new User()).Build()))
                        .Returns(_existingUser);
                };

        Because of =
            () => _exception = Catch.Exception(() => Browser.PostSecureJson("/user", _request));

        It should_not_add_a_command_to_the_queue =
            () => Mock.Get(CommandDispatcher).Verify(x =>
                                                     x.Dispatch(VisitorSession, Moq.It.IsAny<NewUserCommand>()), Times.Never());

        It should_throw_a_bad_request_exception =
            () => _exception.InnerException.InnerException.ShouldBeOfExactType<UserAlreadyExistsException>();
    }
}