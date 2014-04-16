using System;
using System.IO;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Domain.Events;
using FireTower.Domain.Services;

namespace FireTower.Domain.CommandHandlers
{
    public class NewDisasterCreator : ICommandHandler
    {
        readonly ITimeProvider _timeProvider;
        readonly IImageRepository _imageRepository;
        readonly IWriteableRepository _writeableRepository;

        public NewDisasterCreator(IWriteableRepository writeableRepository, ITimeProvider timeProvider, IImageRepository imageRepository)
        {
            _writeableRepository = writeableRepository;
            _timeProvider = timeProvider;
            _imageRepository = imageRepository;
        }

        #region ICommandHandler Members

        public Type CommandType
        {
            get { return typeof (CreateNewDisaster); }
        }

        public void Handle(IUserSession userSessionIssuingCommand, object command)
        {
            var c = (CreateNewDisaster) command;
            var u = (UserSession) userSessionIssuingCommand;

            Disaster newDisaster = CreateDisaster(c, u);
            AddImage(c, u, newDisaster);
        }

        Disaster CreateDisaster(CreateNewDisaster c, UserSession u)
        {
            var itemToCreate = new Disaster(_timeProvider.Now(), c.LocationDescription, c.Latitude, c.Longitude);

            var newDisaster = _writeableRepository.Create(itemToCreate);
            NotifyObservers(new NewDisasterCreated(u.User.Id, newDisaster.Id, newDisaster.CreatedDate,
                                                   c.LocationDescription,
                                                   c.Latitude, c.Longitude, c.FetchToken));
            return newDisaster;
        }

        void AddImage(CreateNewDisaster c, UserSession u, Disaster newDisaster)
        {
            Uri imageUrl = _imageRepository.Save(c.ImageStream);
            newDisaster.AddImage(imageUrl.ToString());
            NotifyObservers(new NewImageAddedToDisaster(u.User.Id, newDisaster.Id, imageUrl.ToString()));
        }

        public event DomainEvent NotifyObservers;

        #endregion
    }
}