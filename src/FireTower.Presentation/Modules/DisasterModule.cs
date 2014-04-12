using System;
using System.Collections.Generic;
using FireTower.Domain;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Infrastructure;
using FireTower.Mailgun;
using FireTower.Presentation.EmailSubjects;
using FireTower.Presentation.EmailTemplates;
using FireTower.Presentation.Requests;
using Nancy;
using Nancy.ModelBinding;

namespace FireTower.Presentation.Modules
{
    public class DisasterModule : NancyModule
    {
        public DisasterModule(ICommandDispatcher commandDispatcher)
        {
            Post["/Disasters"] =
                Request =>
                    {
                        var req = this.Bind<CreateNewDisasterRequest>();
                        commandDispatcher.Dispatch(this.UserSession(),
                                                   new CreateNewDisaster(req.LocationDescription,
                                                                         req.Latitude, req.Longitude, req.FirstImageBase64));
                        return new Response().WithStatusCode(HttpStatusCode.OK);
                    };

            Post["/SendDisasterByEmail"] =
                Request =>
                    {
                        var emailInfo = this.Bind<SendDisasterByEmailRequest>();
                        try
                        {
                            var _model = new Disaster(DateTime.Parse(emailInfo.CreatedDate),
                                                      emailInfo.LocationDescription,
                                                      double.Parse(emailInfo.Latitude),
                                                      double.Parse(emailInfo.Longitude));
                            var sender =
                                new EmailSender(
                                    new EmailBodyRenderer(
                                        new TemplateProvider(new List<IEmailTemplate>
                                                                 {
                                                                     new DisasterEmailTemplate(
                                                                         new DefaultRootPathProvider())
                                                                 }),
                                        new DefaultViewEngine())
                                    , new EmailSubjectProvider(new List<IEmailSubject> {new DisasterEmailSubject()}),
                                    new MailgunSmtpClient());
                            sender.Send(_model, emailInfo.Email);
                            return new Response().WithStatusCode(HttpStatusCode.OK);
                        }
                        catch (Exception ex)
                        {
                            return new Response().WithStatusCode(HttpStatusCode.NotFound);
                        }
                    };
        }
    }
}