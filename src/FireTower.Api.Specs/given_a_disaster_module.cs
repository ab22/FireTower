﻿using AutoMapper;
using FireTower.Domain;
using FireTower.Domain.Entities;
using FireTower.Presentation.Modules;
using Machine.Specifications;
using Moq;
using Nancy.Testing;

namespace FireTower.Api.Specs
{
    public class given_a_disaster_module
    {
        protected static Browser Browser;
        protected static IMappingEngine Mapper;
        protected static ICommandDispatcher CommandDispatcher;
        protected static IReadOnlyRepository ReadOnlyRepo;


        Establish master_context =
            () =>
            {
                Mapper = Mock.Of<IMappingEngine>();
                CommandDispatcher = Mock.Of<ICommandDispatcher>();
                ReadOnlyRepo = Mock.Of<IReadOnlyRepository>();

                Browser = new Browser(x =>
                {
                    x.Module<DisasterModule>();
                    x.Dependency(Mapper);
                    x.Dependency(CommandDispatcher);
                    x.Dependency(ReadOnlyRepo);
                    _user = new User();
                    x.WithUser(_user);
                });
            };

        protected static User _user;
    }
}