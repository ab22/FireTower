using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IdentityModel;
using System.Linq;
using Autofac;
using FireTower.Domain;
using FireTower.Domain.Exceptions;
using FireTower.Infrastructure;
using FireTower.Infrastructure.Exceptions;
using FireTower.Presentation.Logging;
using Nancy;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Security;
using Newtonsoft.Json;

namespace FireTower.Presentation
{
    public class ApiBootstrapper : Bootstrapper
    {
        const string QueueName = "commands";

        public ApiBootstrapper()
        {
            //if (IsWorker())
            //{
                AddBootstrapperTask(new ConfigureWorkerDependencies(QueueName));
                AddBootstrapperTask(new ConfigureNotificationEmails());
                AddBootstrapperTask(new ConfigureEventHandlerDependencies());
            //}

            //if(IsApi())
            //{
                AddBootstrapperTask(new ConfigureApiDependencies(QueueName));
                AddBootstrapperTask(new ConfigureAutomapperMappings());
            //}
        }

        //static bool IsWorker()
        //{
        //    return (ConfigurationManager.AppSettings["Roles"] ?? "").ToLower().Contains("worker");
        //}

        //static bool IsApi()
        //{
        //    return (ConfigurationManager.AppSettings["Roles"] ?? "").ToLower().Contains("api");
        //}

        //protected override IEnumerable<INancyModule> GetAllModules(ILifetimeScope container)
        //{
        //    Type workerModuleType = typeof (NancyWorkerModule);

        //    var nancyModules = new List<INancyModule>();

        //    if (IsWorker())
        //    {
        //        nancyModules.AddRange(base.GetAllModules(container).Where(x => x.GetType().BaseType == workerModuleType));
        //    }

        //    if(IsApi())
        //    {
        //        nancyModules.AddRange(base.GetAllModules(container).Where(x => x.GetType().BaseType != workerModuleType));
        //    }

        //    return nancyModules;
        //}

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;

            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/Test"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("App"));
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Content"));
            base.ConfigureConventions(nancyConventions);
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            StaticConfiguration.DisableErrorTraces = false;

            var configuration =
                new StatelessAuthenticationConfiguration(
                    ctx =>
                        {
                            string token = GetTokenFromRequest(ctx);

                            bool hasToken = !string.IsNullOrEmpty(token);

                            if (hasToken)
                            {
                                var apiUserMapper = container.Resolve<IApiUserMapper<Guid>>();
                                Guid tokenGuid;
                                if (!string.IsNullOrEmpty(token) && Guid.TryParse(token, out tokenGuid))
                                {
                                    try
                                    {
                                        IUserIdentity userFromAccessToken =
                                            apiUserMapper.GetUserFromAccessToken(tokenGuid);
                                        return userFromAccessToken;
                                    }
                                    catch (TokenDoesNotExistException)
                                    {
                                    }
                                }
                            }

                            return new FireTowerUserIdentity(new VisitorSession());
                        });

            pipelines.OnError += (ctx, err) => HandleExceptions(err, ctx);

            pipelines.AfterRequest.AddItemToEndOfPipeline(AddCorsHeaders());

            StatelessAuthentication.Enable(pipelines, configuration);

            base.RequestStartup(container, pipelines, context);
        }

        static string GetTokenFromRequest(NancyContext ctx)
        {
            var token = (string) ctx.Request.Query.token;
            if (token == null)
            {
                token = (string) ctx.Request.Form.token;
            }
            if (token == null)
            {
                const string headerName = "Authorization";
                bool hasAuthHeader = ctx.Request.Headers.Keys.Contains(headerName);
                if (hasAuthHeader)
                {
                    string authHeader =
                        ctx.Request.Headers[headerName].FirstOrDefault();
                    if (authHeader != null)
                        token = authHeader.Replace("OAuth ", "");
                }
            }
            if (token == null)
            {
                var stream = new StreamReader(ctx.Request.Body);
                string body = stream.ReadToEnd();
                ctx.Request.Body.Seek(0, SeekOrigin.Begin);
                try
                {
                    var bodyWithToken = JsonConvert.DeserializeObject<BodyWithToken>(body);
                    if (bodyWithToken != null && bodyWithToken.Token != Guid.Empty)
                        token = bodyWithToken.Token.ToString();
                }
                catch
                {
                }
            }
            return token;
        }

        static Response HandleExceptions(Exception err, NancyContext ctx)
        {
            if (ctx.Response == null)
            {
                ctx.Response = new Response();
                AddCorsHeaders().Invoke(ctx);
            }

            if (err is NotFoundException)
            {
                return ctx.Response
                    .WithStatusCode(HttpStatusCode.NotFound);
            }

            if (err is ForbiddenRequestException)
            {
                return ctx.Response
                    .WithStatusCode(HttpStatusCode.Forbidden);
            }

            if (err is BadRequestException)
            {
                return ctx.Response
                    .WithStringContents(err.Message)
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (err is UserInputPropertyMissingException)
            {
                return ctx.Response
                    .WithStringContents(err.Message)
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (err is UserInputPropertyValidationException)
            {
                return ctx.Response
                    .WithStringContents(err.Message)
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (err is NoAvailableHandlerException)
            {
                return ctx.Response
                    .WithStringContents("No handler exists to for the given command")
                    .WithStatusCode(HttpStatusCode.NotImplemented);
            }

            if (err is UserAlreadyExistsException)
            {
                return ctx.Response
                    .WithStringContents(err.Message)
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (err is NoFireTowerUserException)
            {
                return ctx.Response
                    .WithStringContents(
                        "This endpoint requires a valid Fire Tower user account. Did you include a 'token' in your request?")
                    .WithStatusCode(HttpStatusCode.Unauthorized);
            }

            if (err is UnauthorizedAccessException)
            {
                return ctx.Response
                    .WithStatusCode(HttpStatusCode.Unauthorized);
            }

            if (err is TokenDoesNotExistException)
            {
                return ctx.Response
                    .WithStringContents("The token you tried to use is not valid.")
                    .WithStatusCode(HttpStatusCode.Unauthorized);
            }

            if (err is TokenExpiredException)
            {
                return ctx.Response
                    .WithStringContents(
                        "The token you tried to use has expired. Please log in to get a new one.")
                    .WithStatusCode(HttpStatusCode.Unauthorized);
            }

            //if we get to this point, we need to log the exception...
            var exceptionFormatterLogger = new ExceptionFormatterLogger();
            exceptionFormatterLogger.LogException(err, ctx);

            if (err is NotImplementedException)
            {
                return ctx.Response
                    .WithStringContents(err.Message)
                    .WithStatusCode(HttpStatusCode.NotImplemented);
            }
            else
            {
                string exceptionText = AddException(err);
                return ctx.Response
                    .WithStringContents(
                        string.Format("The {0} request to '{1}' resulted in an unhandled exception!\r\n\r\n{2}",
                                      ctx.Request.Method, ctx.Request.Url, exceptionText))
                    .WithStatusCode(HttpStatusCode.InternalServerError);
            }
        }

        static string AddException(Exception ex)
        {
            string original = string.Format("{0}: {1}\r\n{2}\r\n\r\n",
                                            ex.GetType().Name,
                                            ex.Message,
                                            ex.StackTrace);

            if (ex.InnerException != null)
                original += AddException(ex.InnerException);

            return original;
        }

        static Action<NancyContext> AddCorsHeaders()
        {
            return x =>
                       {
                           x.Response.WithHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                           x.Response.WithHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                           x.Response.WithHeader("Access-Control-Max-Age", "1728000");
                           x.Response.WithHeader("Access-Control-Allow-Origin", "*");
                       };
        }

        #region Nested type: BodyWithToken

        class BodyWithToken
        {
            public Guid Token { get; set; }
        }

        #endregion
    }
}