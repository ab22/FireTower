using AcklenAvenue.Testing.Moq.ExpectedObjects;
using AcklenAvenue.Testing.Nancy;
using FireTower.Domain.Commands;
using FireTower.Presentation.Requests;
using FizzWare.NBuilder;
using Machine.Specifications;
using Moq;
using Nancy;
using Nancy.Testing;
using It = Machine.Specifications.It;

namespace FireTower.Api.Specs
{
    public class when_creating_a_disaster : given_a_disaster_module
    {
        static readonly CreateNewDisasterRequest Request =
            Builder<CreateNewDisasterRequest>.CreateNew().Build();

        static BrowserResponse _result;
        static CreateNewDisaster _expectedCommand;

        Establish context =
            () =>
                {
                    _expectedCommand = new CreateNewDisaster(Request.LocationDescription,
                                                             Request.Latitude,
                                                             Request.Longitude, Request.FirstImageBase64);
                };

        Because of =
            () => _result = Browser.PostSecureJson("/Disasters", Request);

        It should_be_ok = () => _result.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_dispatch_the_expected_command =
            () =>
            Mock.Get(CommandDispatcher)
                .Verify(
                    x =>
                    x.Dispatch(UserSession,
                               WithExpected.Object(_expectedCommand)));
    }
}