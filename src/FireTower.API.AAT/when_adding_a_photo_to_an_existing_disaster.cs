using System;
using System.Linq;
using AcklenAvenue.Testing.AAT;
using FireTower.Domain.Commands;
using FireTower.Presentation.Requests;
using FireTower.ViewStore;
using Machine.Specifications;
using MongoDB.Driver.Linq;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_adding_a_photo_to_an_existing_disaster : given_an_api_server_context<CurrentlyDeveloping>
    {
        static Guid _token;
        static string _disasterId;
        static IRestResponse _result;
        static string _locationDescription;
        static string _imageString;
        static int _imageCount;

        Establish context =
            () =>
                {
                    _token = Login().Token;

                    int rnd = new Random().Next(999999999);
                    _locationDescription = "Santa Ana " + rnd;
                    _imageString = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAAsTAAALEwEAmpwYAAAABGdBTUEAALGOfPtRkwAAACBjSFJNAAB6JQAAgIMAAPn/AACA6QAAdTAAAOpgAAA6mAAAF2+SX8VGAAABLElEQVR42qSTQStFURSFP7f3XygyoAwoYSYMPCIpk2egMFSmUvwCRpSRDIwYGbwyVuYykB9y914m951z7nHe6J26dc9u77XXWmdvJLF7/audqx9JYuvyW92LL0li8K2df2r17CPEVk7ftXTclyQqAMmRCwC5I3fS42a4W7y74VYDNAAuJA8AaXIsSACsDgAdAJeFrnnyoMBygKZJJ3b1It0AmsTMDPdEgrujJqHEwCxqznMaD2KgyCDRnEuo8qJhHvx/hcQDbzGoix5Yi4G1TcwZWNEDKwJU+WDkhg2ToDaD+M65YcVB8jg3Y5IY5VQAyyf9gLJw+CqAuYNnAczsPQpgevtBU937kDexcdssj8Ti0ZskMd97CRs3u//U2sjJzbtwH1+/Cf8jS/gbAMmWc42HzdIjAAAAAElFTkSuQmCC";
                    Client.Post("/disasters",
                                new CreateNewDisaster(_locationDescription, 123.45, 456.32, _imageString),
                                _token);

                    IQueryable<DisasterViewModel> disasterViewModelCollection =
                        MongoDatabase().GetCollection<DisasterViewModel>("DisasterViewModel").AsQueryable();
                    DisasterViewModel disaster =
                        disasterViewModelCollection.First(x => x.LocationDescription == _locationDescription);
                    _disasterId = disaster.DisasterId;
                    _imageCount = disaster.Images.Count();
                };

        Because of =
            () =>
            _result =
            Client.Post("/disasters/" + _disasterId + "/image",
                        new AddImageRequest {Base64Image = _imageString}, _token);

        It should_add_a_photo_url_to_the_disaster_model =
            () =>
                {
                    IQueryable<DisasterViewModel> disasterViewModelCollection =
                        MongoDatabase().GetCollection<DisasterViewModel>("DisasterViewModel").AsQueryable();
                    DisasterViewModel disaster =
                        disasterViewModelCollection.First(x => x.LocationDescription == _locationDescription);
                    disaster.Images.Count().ShouldEqual(_imageCount + 1);
                };

        It should_be_ok = () => _result.ShouldBeOk();
    }
}