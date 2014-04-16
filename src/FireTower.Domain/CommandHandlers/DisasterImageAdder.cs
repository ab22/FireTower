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
        readonly IImageRepository _imageRepository;
        readonly IReadOnlyRepository _readOnlyRepo;

        public DisasterImageAdder(IReadOnlyRepository readOnlyRepo, IImageRepository imageRepository)
        {
            _readOnlyRepo = readOnlyRepo;
            _imageRepository = imageRepository;
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

            Uri imageUrl = _imageRepository.Save(c.ImageStream);
            var disaster = _readOnlyRepo.GetById<Disaster>(c.DisasterId);
            disaster.AddImage(imageUrl.ToString());
            NotifyObservers(new NewImageAddedToDisaster(u.User.Id, c.DisasterId, imageUrl.ToString()));
        }

        public event DomainEvent NotifyObservers;

        #endregion        
    }
}