using System;
using System.Configuration;
using System.Web;
using Autofac;
using BlingBag;
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
    public class ConfigureWorkerDependencies : IBootstrapperTask<ContainerBuilder>
    {
        readonly string _queueName;

        public ConfigureWorkerDependencies(string queueName)
        {
            _queueName = queueName;
        }

        public Action<ContainerBuilder> Task
        {
            get
            {
                return builder =>
                           {
                               builder.RegisterType<JsonCommandDeserializer>().As<ICommandDeserializer>();
                               builder.RegisterType<SynchronousCommandDispatcher>().As<ICommandDispatcher>();
                               builder.RegisterType<PubNubNotificationPublisher>().As<INotificationPublisher>();
                               builder.RegisterAssemblyTypes(typeof(ViewModelRepository).Assembly).
                                   AsImplementedInterfaces();

                               builder.Register(context =>
                               {
                                   var uri =
                                       new MongoUrl(
                                           @"mongodb://server:password@ds045137.mongolab.com:45137/appharbor_ab50c767-930d-4b7d-9571-dd2a0b62d5a9");

                                   var server = new MongoClient(uri).GetServer();
                                   return server.GetDatabase(uri.DatabaseName);
                               }).As<MongoDatabase>();
                               RegisterBlingBagServices(builder);
                               SelfSubscribeWorkerToQueue();
                           };
            }
        }

        static void RegisterBlingBagServices(ContainerBuilder container)
        {
            container.RegisterType<BlingInitializer<DomainEvent>>().As<IBlingInitializer<DomainEvent>>();
            container.RegisterType<BlingConfigurator>().As<IBlingConfigurator<DomainEvent>>();
            container.RegisterType<AutoFacBlingDispatcher>().As<IBlingDispatcher>();
        }

        void SelfSubscribeWorkerToQueue()
        {
            if (HttpContext.Current != null)
            {
                var urlFromConfig = ConfigurationManager.AppSettings["hostname"];
                Uri url = !string.IsNullOrEmpty(urlFromConfig)
                              ? new Uri(urlFromConfig)
                              : HttpContext.Current.Request.Url;
                new IronMqSubscriber().Subscribe(_queueName,
                                                 string.Format("{0}://{1}:{2}/work", url.Scheme,
                                                               url.Host, url.Port));
            }
        }

    }
}