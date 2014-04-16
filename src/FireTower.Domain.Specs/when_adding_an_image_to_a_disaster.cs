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
        static IImageRepository _imageRepository;
        static object _eventRaised;
        static NewImageAddedToDisaster _expectedEvent;
        static readonly Guid UserId = Guid.NewGuid();
        static Disaster _disaster;
        static Uri _imageUrl;

        Establish context =
            () =>
                {
                    _readOnlyRepo = Mock.Of<IReadOnlyRepository>();
                    _imageRepository = Mock.Of<IImageRepository>();
                    _commandHandler = new DisasterImageAdder(_readOnlyRepo, _imageRepository);

                    _command = new AddImageToDisaster(Guid.NewGuid(), new MemoryStream());

                    _imageUrl = new Uri("http://www.something.com/imageUrl" + new Random().Next(99999999));
                    Mock.Get(_imageRepository).Setup(x => x.Save(_command.ImageStream)).Returns(_imageUrl);

                    _disaster = new Disaster(DateTime.Now, "somewhere", 1, 2);
                    Mock.Get(_readOnlyRepo).Setup(x => x.GetById<Disaster>(_command.DisasterId)).Returns(_disaster);
                    _commandHandler.NotifyObservers += x => _eventRaised = x;
                    _expectedEvent = new NewImageAddedToDisaster(UserId, _command.DisasterId, _imageUrl.ToString());
                };

        Because of =
            () => _commandHandler.Handle(new UserSession {User = new User {Id = UserId}}, _command);

        It should_add_the_image_to_the_disaster =
            () => _disaster.Images.ShouldContain(x => x.Url == _imageUrl.ToString());

        It should_handle_the_expected_command_type =
            () => _commandHandler.CommandType.ShouldEqual(_command.GetType());

        It should_raise_the_expected_event =
            () => _eventRaised.ShouldBeLike(_expectedEvent);
    }
}