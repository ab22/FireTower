using System;
using System.IO;
using System.Linq;
using FireTower.Domain;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Infrastructure;
using Nancy;

namespace FireTower.Presentation.Modules
{
    public class ImageModule : NancyModule
    {
        public ImageModule(ICommandDispatcher commandDispatcher)
        {
            Post["/disasters/{disasterId}/image"] =
                r =>
                    {
                        HttpFile file = Request.Files.First();
                        using (var stream = new MemoryStream())
                        {
                            file.Value.CopyTo(stream);
                            UserSession userSession = this.UserSession();
                            commandDispatcher.Dispatch(userSession,
                                                       new AddImageToDisaster(Guid.Parse((string) r.disasterId),
                                                                              stream));
                        }

                        return null;
                    };
        }
    }
}