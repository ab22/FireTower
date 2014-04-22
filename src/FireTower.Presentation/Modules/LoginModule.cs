using System;
using FireTower.Data;
using FireTower.Domain;
using FireTower.Domain.Entities;
using FireTower.Infrastructure.Exceptions;
using FireTower.Presentation.Requests;
using FireTower.Presentation.Responses;
using Nancy;
using Nancy.ModelBinding;

namespace FireTower.Presentation.Modules
{
    public class LoginModule : NancyModule
    {
        public LoginModule(
            IReadOnlyRepository readOnlyRepository,
            IUserSessionFactory userSessionFactory, IPasswordEncryptor passwordEncryptor)
        {
            Post["/login/facebook"] =
                r =>
                    {
                        var loginInfo = this.Bind<FacebookLoginRequest>();
                        if (string.IsNullOrEmpty(loginInfo.FacebookId))
                            throw new UserInputPropertyMissingException("FacebookId");

                        try
                        {
                            var user =
                                readOnlyRepository.First<User>(x => x.FacebookId == loginInfo.FacebookId);

                            UserSession userSession = userSessionFactory.Create(user);

                            return new SuccessfulLoginResponse<Guid>(userSession.Id, userSession.Expires);
                        }
                        catch (ItemNotFoundException<User> ex)
                        {
                            return new Response().WithStatusCode(HttpStatusCode.Unauthorized);
                        }
                    };

            Post["/login"] =
                r =>
                    {
                        var loginInfo = this.Bind<BasicLoginRequest>();
                        if (loginInfo.Email == null) throw new UserInputPropertyMissingException("Email");
                        if (loginInfo.Password == null) throw new UserInputPropertyMissingException("Password");

                        EncryptedPassword encryptedPassword = passwordEncryptor.Encrypt(loginInfo.Password);

                        try
                        {
                            var user =
                                readOnlyRepository.First<User>(
                                    x => x.Email == loginInfo.Email 
                                        && x.EncryptedPassword == encryptedPassword.Password);

                            //if (!user.Activated) throw new ForbiddenRequestException();

                            UserSession userSession = userSessionFactory.Create(user);

                            return new SuccessfulLoginResponse<Guid>(userSession.Id, userSession.Expires);
                        }
                        catch (ItemNotFoundException<User>)
                        {
                            throw new UnauthorizedAccessException();
                        }
                    };

            Post["/logout"] =
                r =>
                    {
                        var loginInfo = this.Bind<FacebookLoginRequest>();
                        try
                        {
                            var session =
                                readOnlyRepository.First<UserSession>(x => x.User.FacebookId == loginInfo.FacebookId);

                            userSessionFactory.Delete(session.Id);

                            return new Response().WithStatusCode(HttpStatusCode.OK);
                        }
                        catch (ItemNotFoundException<UserSession> ex)
                        {
                            return new Response().WithStatusCode(HttpStatusCode.Unauthorized);
                        }
                    };
        }
    }
}