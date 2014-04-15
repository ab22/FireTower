using System;
using AcklenAvenue.Testing.Moq.ExpectedObjects;
using BlingBag;
using FireTower.Domain;
using FireTower.Domain.Events;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace FireTower.ViewStore.Specs
{
    public class after_creating_a_new_disaster
    {
        static IBlingHandler<NewDisasterCreated> _eventHandler;
        static IViewModelRepository _viewModelRepository;

        static readonly NewDisasterCreated NewDisasterCreated =
            new NewDisasterCreated(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Santa Ana", 1234.43, 12321.43);

        static INotificationPublisher _notificationPublisher;

        Establish context =
            () =>
                {
                    _viewModelRepository = Mock.Of<IViewModelRepository>();
                    _notificationPublisher = Mock.Of<INotificationPublisher>();
                    _eventHandler = new DisasterViewModelCreator(_viewModelRepository, _notificationPublisher);
                };

        Because of =
            () => _eventHandler.Handle(NewDisasterCreated);

        It should_add_the_disaster_to_the_view_model_store =
            () => Mock.Get(_viewModelRepository)
                      .Verify(x =>
                              x.Create(
                                  WithExpected.Object(
                                      new DisasterViewModel(NewDisasterCreated.UserId,
                                                            NewDisasterCreated.DisasterId,
                                                            NewDisasterCreated.CreatedDate,
                                                            NewDisasterCreated.LocationDescription,
                                                            NewDisasterCreated.Latitude,
                                                            NewDisasterCreated.Longitude))));
    }
}