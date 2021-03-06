using System;
using AcklenAvenue.Data.NHibernate;
using Autofac;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using FireTower.Data;

namespace FireTower.Infrastructure
{
    public class ConfigureDatabaseWiring : IBootstrapperTask<ContainerBuilder>
    {
        #region IBootstrapperTask<ContainerBuilder> Members

        public Action<ContainerBuilder> Task
        {
            get
            {
                MsSqlConfiguration databaseConfiguration = MsSqlConfiguration.MsSql2008.ShowSql().
                    ConnectionString(x => x.Is(ConnectionStrings.Get()));

                return container =>
                           {
                               container.Register(c => c.Resolve<ISessionFactory>().OpenSession()).As
                                   <ISession>()
                                   .InstancePerLifetimeScope()
                                   .OnActivating(c =>
                                                     {
                                                         if (!c.Instance.Transaction.IsActive)
                                                             c.Instance.BeginTransaction();
                                                     }
                                   )
                                   .OnRelease(c =>
                                                  {
                                                      if (c.Transaction.IsActive)
                                                      {
                                                          c.Transaction.Commit();
                                                      }
                                                      c.Dispose();
                                                  });

                    

                               container.Register(c =>
                                                  new SessionFactoryBuilder(new MappingScheme(), databaseConfiguration)
                                                      .Build())
                                   .SingleInstance()
                                   .As<ISessionFactory>();
                           };
            }
        }

        #endregion
    }
}