using System;
using AcklenAvenue.Testing.AAT;
using FireTower.Presentation.Requests;
using Machine.Specifications;
using RestSharp;

namespace FireTower.API.AAT
{
    public class when_creating_a_new_disaster : given_an_api_server_context<DeployedToQA>
    {
        static Guid _token;
        static IRestResponse _result;
        static string _url;
        static string _imageString;

        Establish context =
            () =>
                {
                    _token = Login().Token;

                    _url = string.Format("http://{0}.url.com", new Random().Next(9999999));

                    _imageString =
                        "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAACXBIWXMAAAsTAAALEwEAmpwYAAAABGdBTUEAALGOfPtRkwAAACBjSFJNAAB6JQAAgIMAAPn/AACA6QAAdTAAAOpgAAA6mAAAF2+SX8VGAAABLElEQVR42qSTQStFURSFP7f3XygyoAwoYSYMPCIpk2egMFSmUvwCRpSRDIwYGbwyVuYykB9y914m951z7nHe6J26dc9u77XXWmdvJLF7/audqx9JYuvyW92LL0li8K2df2r17CPEVk7ftXTclyQqAMmRCwC5I3fS42a4W7y74VYDNAAuJA8AaXIsSACsDgAdAJeFrnnyoMBygKZJJ3b1It0AmsTMDPdEgrujJqHEwCxqznMaD2KgyCDRnEuo8qJhHvx/hcQDbzGoix5Yi4G1TcwZWNEDKwJU+WDkhg2ToDaD+M65YcVB8jg3Y5IY5VQAyyf9gLJw+CqAuYNnAczsPQpgevtBU937kDexcdssj8Ti0ZskMd97CRs3u//U2sjJzbtwH1+/Cf8jS/gbAMmWc42HzdIjAAAAAElFTkSuQmCC";
                };

        Because of =
            () =>
            _result =
            Client.Post("/disasters",
                        new CreateNewDisasterRequest
                            {
                                LocationDescription = "Santa Ana",
                                Latitude = 123.45,
                                Longitude = 31.32,
                                FirstImageBase64 = _imageString
                            },
                        _token);

        It should_put_a_disaster_in_the_view_model_store =
            () => { };

        It should_return_ok =
            () => _result.ShouldBeOk();
    }
}