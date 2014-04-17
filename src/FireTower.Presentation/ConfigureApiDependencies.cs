using System;
using System.Configuration;
using System.Web;
using AutoMapper;
using Autofac;
using FireTower.Domain;
using FireTower.Domain.CommandDispatchers;
using FireTower.Domain.Services;
using FireTower.Infrastructure;
using FireTower.IronMq;
using FireTower.S3;
using FireTower.ViewStore;
using MongoDB.Driver;
using PubNubMessaging.Core;

namespace FireTower.Presentation
{
    public class ConfigureApiDependencies : IBootstrapperTask<ContainerBuilder>
    {
        #region IBootstrapperTask<ContainerBuilder> Members

        public Action<ContainerBuilder> Task
        {
            get
            {
                return builder =>
                           {
                               builder.RegisterInstance(Mapper.Engine).As<IMappingEngine>();
                               builder.RegisterType<TokenExpirationProvider>().As<ITokenExpirationProvider>();
                               builder.RegisterType<ApiUserMapper>().As<IApiUserMapper<Guid>>();
                               builder.RegisterType<AmazonImageRepository>().As<IImageRepository>();
                               builder.RegisterType<PubNubNotificationPublisher>().As<INotificationPublisher>();
                               builder.RegisterInstance(new RestSharpIronMqClientAdapter("commands"));

                               ChooseCommandDispatcherBasedOnRoleType(builder);
                               SelfSubscribeWorkerToQueue();

                               builder.RegisterAssemblyTypes(typeof (ViewModelRepository).Assembly).
                                   AsImplementedInterfaces();

                               builder.Register(context =>
                                                    {
                                                        var uri =
                                                            new MongoUrl(
                                                                @"mongodb://server:password@ds045137.mongolab.com:45137/appharbor_ab50c767-930d-4b7d-9571-dd2a0b62d5a9");

                                                        var server = new MongoClient(uri).GetServer();
                                                        return server.GetDatabase(uri.DatabaseName);
                                                    }).As<MongoDatabase>();
                           };
            }
        }

        static void SelfSubscribeWorkerToQueue()
        {
            if (IsWorker() && HttpContext.Current != null)
            {
                Uri url = HttpContext.Current.Request.Url;
                new IronMqSubscriber().Subscribe("commands",
                                                 string.Format("{0}://{1}:{2}/work", url.Scheme,
                                                               url.Host, url.Port));
            }
        }

        static void ChooseCommandDispatcherBasedOnRoleType(ContainerBuilder builder)
        {
            if (IsWorker())
            {
                builder.RegisterType<SynchronousCommandDispatcher>().As<ICommandDispatcher>();
            }
            else
            {
                builder.RegisterType<IronMqCommandDispatcher>().As<ICommandDispatcher>();
            }
        }

        static bool IsWorker()
        {
            bool isWorkerAndShouldHandleCommandsSynchronously
                = (ConfigurationManager.AppSettings["Roles"] ?? "").ToLower().Contains("worker");
            return isWorkerAndShouldHandleCommandsSynchronously;
        }

        #endregion
    }
}