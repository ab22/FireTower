using System;
using AutoMapper;
using Autofac;
using FireTower.Domain;
using FireTower.Domain.Services;
using FireTower.Infrastructure;
using FireTower.IronMq;
using FireTower.S3;

namespace FireTower.Presentation
{
    public class ConfigureApiDependencies : IBootstrapperTask<ContainerBuilder>
    {
        readonly string _queueName;

        public ConfigureApiDependencies(string queueName)
        {
            _queueName = queueName;
        }

        #region IBootstrapperTask<ContainerBuilder> Members

        public Action<ContainerBuilder> Task
        {
            get
            {
                return builder =>
                           {
                               builder.RegisterType<JsonCommandDeserializer>().As<ICommandDeserializer>();                               
                               builder.RegisterInstance(Mapper.Engine).As<IMappingEngine>();
                               builder.RegisterType<TokenExpirationProvider>().As<ITokenExpirationProvider>();
                               builder.RegisterType<AmazonImageRepository>().As<IImageRepository>();                               
                               builder.RegisterType<ApiUserMapper>().As<IApiUserMapper<Guid>>();
                               builder.RegisterType<IronMqCommandDispatcher>().As<ICommandDispatcher>();
                               builder.RegisterInstance(new RestSharpIronMqClientAdapter(_queueName)).As<IIronMqPusher>();
                           };
            }
        }

        #endregion
    }
}