using System;
using System.IO;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Domain.Events;
using FireTower.Domain.Services;

namespace FireTower.Domain.CommandHandlers
{
    public class DisasterImageAdder : ICommandHandler
    {
        readonly IReadOnlyRepository _readOnlyRepo;

        public DisasterImageAdder(IReadOnlyRepository readOnlyRepo)
        {
            _readOnlyRepo = readOnlyRepo;
        }

        #region ICommandHandler Members

        public Type CommandType
        {
            get { return typeof (AddImageToDisaster); }
        }

        public void Handle(IUserSession userSessionIssuingCommand, object command)
        {
            var c = (AddImageToDisaster) command;
            var u = (UserSession) userSessionIssuingCommand;

            var disaster = _readOnlyRepo.GetById<Disaster>(c.DisasterId);
            disaster.AddImage(c.ImageUri.ToString());
            NotifyObservers(new NewImageAddedToDisaster(u.User.Id, c.DisasterId, c.ImageUri.ToString()));
        }

        public event DomainEvent NotifyObservers;

        #endregion        
    }
}