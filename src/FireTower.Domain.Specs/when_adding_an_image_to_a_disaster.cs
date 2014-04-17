using System;
using System.IO;
using FireTower.Domain.CommandHandlers;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Domain.Events;
using FireTower.Domain.Services;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace FireTower.Domain.Specs
{
    public class when_adding_an_image_to_a_disaster
    {
        static AddImageToDisaster _command;
        static ICommandHandler _commandHandler;
        static IReadOnlyRepository _readOnlyRepo;
        static object _eventRaised;
        static NewImageAddedToDisaster _expectedEvent;
        static readonly Guid UserId = Guid.NewGuid();
        static Disaster _disaster;
        
        Establish context =
            () =>
                {
                    _readOnlyRepo = Mock.Of<IReadOnlyRepository>();
                    _commandHandler = new DisasterImageAdder(_readOnlyRepo);

                    _command = new AddImageToDisaster(Guid.NewGuid(), new Uri("http://image.com"));

                    _disaster = new Disaster(DateTime.Now, "somewhere", 1, 2);
                    Mock.Get(_readOnlyRepo).Setup(x => x.GetById<Disaster>(_command.DisasterId)).Returns(_disaster);
                    _commandHandler.NotifyObservers += x => _eventRaised = x;
                    _expectedEvent = new NewImageAddedToDisaster(UserId, _command.DisasterId, _command.ImageUri.ToString());
                };

        Because of =
            () => _commandHandler.Handle(new UserSession {User = new User {Id = UserId}}, _command);

        It should_add_the_image_to_the_disaster =
            () => _disaster.Images.ShouldContain(x => x.Url == _command.ImageUri.ToString());

        It should_handle_the_expected_command_type =
            () => _commandHandler.CommandType.ShouldEqual(_command.GetType());

        It should_raise_the_expected_event =
            () => _eventRaised.ShouldBeLike(_expectedEvent);
    }
}