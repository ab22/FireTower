using System;
using System.Collections.Generic;
using System.Linq;
using AcklenAvenue.Testing.AAT;
using FireTower.Domain.Commands;
using FireTower.ViewStore;
using Machine.Specifications;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_voting_on_the_severity_of_a_fire : given_an_api_server_context<CurrentlyDeveloping>
    {
        static string _disasterId;
        static IRestResponse _result;
        static Guid _token;
        static string _locationName;
        static MongoCollection<DisasterViewModel> _disasterViewModelCollection;
        static DisasterViewModel _disaster;

        Establish context =
            () =>
                {
                    _token = Login().Token;
                    _locationName = "Santa Ana"+ new Random().Next(9999999);
                    Client.Post("/disasters", new CreateNewDisaster(_locationName, 123.45, 456.32, "first image base 64"),
                                _token);

                    var db = MongoDatabase();
                    _disasterViewModelCollection = db.GetCollection<DisasterViewModel>("DisasterViewModel");
                    var disasterViewModels =
                        _disasterViewModelCollection.AsQueryable().ToList();
                    _disaster = disasterViewModels.First(x => x.LocationDescription==_locationName);
                    _disasterId = _disaster.DisasterId;
                };

        Because of =
            () => _result = Client.Post("votes/severity", new {disasterId = _disasterId, severity = 3}, _token);

        It should_add_the_severity_vote_to_the_severity_votes_list =
            () =>
                {
                    var disasterViewModel =
                        _disasterViewModelCollection.AsQueryable().FirstOrDefault(
                            x => x.LocationDescription == _locationName);
                    disasterViewModel.SeverityVotes.Count().ShouldEqual(2);
                };

        It should_return_ok =
            () => _result.ShouldBeOk();

        Cleanup when_finished =
            () =>
                {
                    /*if (_disaster != null)
                    {
                        _disasterViewModelCollection.Remove(Query<DisasterViewModel>.EQ(x => x.DisasterId, _disaster.DisasterId));
                    }*/
                };
    }
}