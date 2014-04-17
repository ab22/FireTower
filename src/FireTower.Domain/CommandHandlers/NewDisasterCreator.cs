using System;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Domain.Events;

namespace FireTower.Domain.CommandHandlers
{
    public class NewDisasterCreator : ICommandHandler
    {
        readonly ITimeProvider _timeProvider;
        readonly IWriteableRepository _writeableRepository;

        public NewDisasterCreator(IWriteableRepository writeableRepository, ITimeProvider timeProvider)
        {
            _writeableRepository = writeableRepository;
            _timeProvider = timeProvider;
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

        public event DomainEvent NotifyObservers;

        #endregion

        Disaster CreateDisaster(CreateNewDisaster c, UserSession u)
        {
            var itemToCreate = new Disaster(_timeProvider.Now(), c.LocationDescription, c.Latitude, c.Longitude);

            Disaster newDisaster = _writeableRepository.Create(itemToCreate);
            NotifyObservers(new NewDisasterCreated(u.User.Id, newDisaster.Id, newDisaster.CreatedDate,
                                                   c.LocationDescription,
                                                   c.Latitude, c.Longitude, c.FetchToken));
            return newDisaster;
        }

        void AddImage(CreateNewDisaster c, UserSession u, Disaster newDisaster)
        {
            newDisaster.AddImage(c.ImageUri.ToString());
            NotifyObservers(new NewImageAddedToDisaster(u.User.Id, newDisaster.Id, c.ImageUri.ToString()));
        }
    }
}