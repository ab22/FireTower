using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Machine.Specifications;
using FireTower.Domain;
using FireTower.Infrastructure;
using FireTower.Presentation;

namespace FireTower.Api.Specs
{
    public class when_instantiating_all_command_handlers
    {
        static IContainer _container;
        static List<Type> _types;

        Establish context =
            () =>
                {
                    var typeScanner = new TypeScanner.TypeScanner();
                    _types =
                        typeScanner.GetTypesOf<ICommandHandler>().ToList();

                    var containerBuilder = new ContainerBuilder();
                    new ConfigureCommonDependencies().Task(containerBuilder);
                    new ConfigureWorkerDependencies("test_queue").Task(containerBuilder);
                    new ConfigureDatabaseWiring().Task(containerBuilder);
                    _types.ForEach(x => containerBuilder.RegisterType(x));

                    _container = containerBuilder.Build();
                };

        Because of =
            () => _types.ForEach(x => _container.Resolve(x));

        It should_not_throw_any_exceptions =
            () => { };
    }
}