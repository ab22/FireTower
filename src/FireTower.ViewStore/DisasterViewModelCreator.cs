using BlingBag;
using FireTower.Domain;
using FireTower.Domain.Events;

namespace FireTower.ViewStore
{
    public class DisasterViewModelCreator : IBlingHandler<NewDisasterCreated>
    {
        readonly IViewModelRepository _repository;
        readonly INotificationPublisher _notificationPublisher;

        public DisasterViewModelCreator(IViewModelRepository repository, INotificationPublisher notificationPublisher)
        {
            _repository = repository;
            _notificationPublisher = notificationPublisher;
        }

        #region IBlingHandler<NewDisasterCreated> Members

        public void Handle(NewDisasterCreated @event)
        {
            var vm =_repository.Create(new DisasterViewModel(@event.UserId, @event.DisasterId, @event.CreatedDate, @event.LocationDescription,
                                                     @event.Latitude, @event.Longitude));
        }

        #endregion
    }

    public class DisasterViewModelId
    {
        public string ViewModelId { get; set; }
    }
}