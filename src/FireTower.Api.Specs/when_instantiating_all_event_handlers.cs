using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Machine.Specifications;
using Nancy;
using FireTower.Domain.EventHandlers;
using FireTower.Infrastructure;
using FireTower.Presentation;

namespace FireTower.Api.Specs
{
    public class when_instantiating_all_event_handlers
    {
        static IContainer _container;
        static List<Type> _types;
        static TypeScanner.TypeScanner _typeScanner;

        Establish context =
            () =>
                {
                    _typeScanner = new TypeScanner.TypeScanner();

                    var containerBuilder = new ContainerBuilder();
                    new ConfigureCommonDependencies().Task(containerBuilder);
                    new ConfigureWorkerDependencies("test_queue").Task(containerBuilder);
                    new ConfigureDatabaseWiring().Task(containerBuilder);
                    new ConfigureEventHandlerDependencies().Task(containerBuilder);
                    new ConfigureNotificationEmails().Task(containerBuilder);
                    containerBuilder.RegisterType<DefaultRootPathProvider>().As<IRootPathProvider>();

                    GetTypesToTest("IBlingHandler").ForEach(x => containerBuilder.RegisterType(x));

                    _container = containerBuilder.Build();
                };

        It should_not_throw_any_exceptions =
            () => ForEachTypeInNamespaceOf<NewUserVerificationEventHandler>(
                ResolveAllTypesWithMatchingGenericInterface("IBlingHandler"));

        static Action<Type> ResolveAllTypesWithMatchingGenericInterface(string interfaceName)
        {
            return x =>
                       {
                           var handlers = GetTypesToTest(interfaceName);

                           handlers.ForEach(h =>
                                                {
                                                    _container.Resolve(h);
                                                    Console.WriteLine("Resolved {0}", h.Name);
                                                });
                       };
        }

        static List<Type> GetTypesToTest(string interfaceNAme)
        {
            List<Type> handlers =
                _typeScanner.GetTypesWhere(
                    h =>
                    h.GetInterfaces().Any(
                        i =>
                        i.Name.StartsWith(interfaceNAme))).ToList();
            return handlers;
        }

        static void ForEachTypeInNamespaceOf<T>(Action<Type> func)
        {
            List<Type> types = AllTypesInNamespaceOf<T>();
            types.ForEach(func);
        }

        static List<Type> AllTypesInNamespaceOf<T>()
        {
            List<Type> types =
                _typeScanner.GetTypesWhere(
                    x => x.Namespace != null && x.Namespace.Equals(typeof (T).Namespace) && !x.IsAbstract)
                    .ToList();
            return types;
        }
    }
}