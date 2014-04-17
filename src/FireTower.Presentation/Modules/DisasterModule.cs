using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FireTower.Domain;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Domain.Services;
using FireTower.Infrastructure;
using FireTower.Infrastructure.Exceptions;
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
        public DisasterModule(ICommandDispatcher commandDispatcher, IImageRepository imageRepository)
        {
            Post["/Disasters"] =
                r =>
                    {
                        HttpFile file = Request.Files.FirstOrDefault();
                        if (file == null)
                        {
                            throw new UserInputPropertyMissingException("file");
                        }

                        var stream = new MemoryStream();
                        file.Value.CopyTo(stream);
                        var req = this.Bind<CreateNewDisasterRequest>();
                        if (string.IsNullOrEmpty(req.LocationDescription))
                            throw new UserInputPropertyMissingException("LocationDescription");


                        Uri uri = imageRepository.Save(stream);

                        commandDispatcher.Dispatch(this.UserSession(),
                                                   new CreateNewDisaster(req.LocationDescription,
                                                                         req.Latitude,
                                                                         req.Longitude, uri,
                                                                         req.FetchToken));
                        stream.Dispose();

                        return null;
                    }
                ;

            Post[
                "/SendDisasterByEmail"] =
                r
                =>
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