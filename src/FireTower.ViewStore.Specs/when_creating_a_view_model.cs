using System;
using AcklenAvenue.Testing.ExpectedObjects;
using FireTower.Domain.Services;
using Machine.Specifications;
using MongoDB.Driver;

namespace FireTower.ViewStore.Specs
{
    public class when_creating_a_view_model
    {
        static IViewModelRepository _repo;
        static Random _random;
        static DisasterViewModel _result;
        static DisasterViewModel _viewModel;
        static MongoServer _server;
        static MongoUrl _uri;

        Establish context =
            () =>
                {
                    _uri =
                        new MongoUrl(
                            @"mongodb://server:password@ds045137.mongolab.com:45137/appharbor_ab50c767-930d-4b7d-9571-dd2a0b62d5a9");

                    _server = new MongoClient(_uri).GetServer();
                    _repo = new ViewModelRepository(_server.GetDatabase(_uri.DatabaseName));

                    _random = new Random();

                    _viewModel = new DisasterViewModel(Guid.NewGuid(), Guid.NewGuid(), DateTime.Today.ToLocalTime(), "Santa Ana",
                                                       _random.NextDouble(), _random.NextDouble(), "fetchToken");
                };

        Because of =
            () =>
            _result =
            _repo.Create(_viewModel);

        It should_add_the_docuemnt_to_mongo =
            () =>
                {
                    var resultFromMongoLab = _server.GetDatabase(_uri.DatabaseName)
                           .GetCollection<DisasterViewModel>(typeof (DisasterViewModel).Name)
                           .FindOneById(_result.Id);
                    resultFromMongoLab.CreatedDate = resultFromMongoLab.CreatedDate.ToLocalTime();
                    resultFromMongoLab.IsExpectedToBeLike(_result);
                };

        It should_return_the_same_viewmodel = () => _result.ShouldEqual(_viewModel);
    }
}