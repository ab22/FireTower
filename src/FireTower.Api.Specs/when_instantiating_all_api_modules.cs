using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FireTower.Infrastructure;
using FireTower.Presentation;
using Machine.Specifications;
using Nancy;

namespace FireTower.Api.Specs
{
    public class when_instantiating_all_api_modules
    {
        static IContainer _container;
        static List<Type> _modules;
        static readonly List<Exception> Exceptions = new List<Exception>();

        Establish context =
            () =>
                {
                    var typeScanner = new TypeScanner.TypeScanner();
                    _modules =
                        typeScanner.GetTypesWhere(
                            x => x.BaseType == typeof (NancyModule)
                                 && x.Assembly.FullName.Contains("FireTower")
                                 && x.BaseType == typeof (NancyWorkerModule)).ToList();

                    var containerBuilder = new ContainerBuilder();
                    new ConfigureCommonDependencies().Task(containerBuilder);
                    new ConfigureApiDependencies("test_queue").Task(containerBuilder);
                    new ConfigureDatabaseWiring().Task(containerBuilder);
                    _modules.ForEach(x => containerBuilder.RegisterType(x));

                    _container = containerBuilder.Build();
                };

        Because of =
            () => _modules.ForEach(x => Exceptions.Add(Catch.Exception(() =>
                                                                           {
                                                                               try
                                                                               {
                                                                                   _container.Resolve(x);
                                                                               }
                                                                               catch (Exception ex)
                                                                               {
                                                                                   throw new CannotResolveModuleException
                                                                                       (x, ex);
                                                                               }
                                                                           })));

        It should_not_throw_any_exceptions =
            () => Exceptions.ShouldBeEmpty();
    }


    public class when_instantiating_all_worker_modules
    {
        static IContainer _container;
        static List<Type> _modules;
        static readonly List<Exception> Exceptions = new List<Exception>();

        Establish context =
            () =>
                {
                    var typeScanner = new TypeScanner.TypeScanner();
                    _modules =
                        typeScanner.GetTypesWhere(
                            x => x.BaseType == typeof (NancyModule)
                                 && x.Assembly.FullName.Contains("FireTower")
                                 && x.BaseType == typeof (NancyWorkerModule)
                            ).ToList();

                    var containerBuilder = new ContainerBuilder();
                    new ConfigureCommonDependencies().Task(containerBuilder);
                    new ConfigureWorkerDependencies("test_queue").Task(containerBuilder);
                    new ConfigureDatabaseWiring().Task(containerBuilder);
                    _modules.ForEach(x => containerBuilder.RegisterType(x));

                    _container = containerBuilder.Build();
                };

        Because of =
            () => _modules.ForEach(x => Exceptions.Add(Catch.Exception(() =>
                                                                           {
                                                                               try
                                                                               {
                                                                                   _container.Resolve(x);
                                                                               }
                                                                               catch (Exception ex)
                                                                               {
                                                                                   throw new CannotResolveModuleException
                                                                                       (x, ex);
                                                                               }
                                                                           })));

        It should_not_throw_any_exceptions =
            () => Exceptions.ShouldBeEmpty();
    }
}