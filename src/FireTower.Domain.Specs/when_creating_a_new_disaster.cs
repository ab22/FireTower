using System;
using System.Collections.Generic;
using System.IO;
using AcklenAvenue.Testing.Moq.ExpectedObjects;
using FireTower.Domain.CommandHandlers;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Domain.Events;
using FireTower.Domain.Services;
using FizzWare.NBuilder;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace FireTower.Domain.Specs
{
    public class when_creating_a_new_disaster
    {
        static IWriteableRepository _writeableRepository;
        static ICommandHandler _commandHandler;
        static CreateNewDisaster _command;
        static NewDisasterCreated _expectedDisasterCreatedEvent;
        static Disaster _disasterToCreate;
        static ITimeProvider _timeProvider;
        static DateTime _now;
        static readonly User User = new User {Id = Guid.NewGuid()};
        static List<object> _eventsRaised;
        static Disaster _createdDisaster;
        static NewImageAddedToDisaster _expectedImageAddedEvent;

        Establish context =
            () =>
                {
                    _writeableRepository = Mock.Of<IWriteableRepository>();
                    _timeProvider = Mock.Of<ITimeProvider>();
                    _commandHandler = new NewDisasterCreator(_writeableRepository, _timeProvider);

                    _now = DateTime.Now;
                    Mock.Get(_timeProvider).Setup(x => x.Now()).Returns(_now);

                    _command = new CreateNewDisaster("LocationDescription1", 123.34, 456.32,
                                                     new Uri("http://image.com"), "fetchToken");
                    
                    _disasterToCreate =
                        Builder<Disaster>.CreateNew()
                            .With(x => x.Id, Guid.Empty)
                            .With(disaster => disaster.CreatedDate, _now)
                            .With(disaster => disaster.LocationDescription, _command.LocationDescription)
                            .With(disaster => disaster.Latitude, _command.Latitude)
                            .With(disaster => disaster.Longitude, _command.Longitude)
                            .Build();

                    _createdDisaster =
                        Builder<Disaster>.CreateNew()
                            .With(x => x.Id, Guid.NewGuid())
                            .With(disaster => disaster.CreatedDate, _now)
                            .With(disaster => disaster.LocationDescription, _command.LocationDescription)
                            .With(disaster => disaster.Latitude, _command.Latitude)
                            .With(disaster => disaster.Longitude, _command.Longitude)
                            .Build();

                    Mock.Get(_writeableRepository).Setup(x => x.Create(WithExpected.Object(_disasterToCreate)))
                        .Returns(_createdDisaster);

                    _eventsRaised = new List<object>();
                    _commandHandler.NotifyObservers += x => _eventsRaised.Add(x);

                    _expectedDisasterCreatedEvent = new NewDisasterCreated(User.Id, _createdDisaster.Id, _now,
                                                                           _command.LocationDescription,
                                                                           _command.Latitude, _command.Longitude, _command.FetchToken);

                    _expectedImageAddedEvent = new NewImageAddedToDisaster(User.Id, _createdDisaster.Id, _command.ImageUri.ToString());
                };

        Because of =
            () =>
            _commandHandler.Handle(UserSession.New(User), _command);

        It should_add_the_image_to_the_new_disaster =
            () => _createdDisaster.Images.ShouldContain(x => x.Url == _expectedImageAddedEvent.ImageUrl);

        It should_handle_the_expected_command_type =
            () => _commandHandler.CommandType.ShouldEqual(typeof (CreateNewDisaster));

        It should_raise_the_expected_creation_event =
            () => _eventsRaised[0].ShouldBeLike(_expectedDisasterCreatedEvent);

        It should_raise_the_expected_image_added_event =
            () => _eventsRaised[1].ShouldBeLike(_expectedImageAddedEvent);        
    }
}