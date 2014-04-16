using System;
using System.IO;
using System.Linq;
using FireTower.Domain;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Infrastructure;
using FireTower.Infrastructure.Exceptions;
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
                        var disasterId = Guid.Parse((string)r.disasterId);
                        if(disasterId==Guid.Empty)
                        {
                            throw new UserInputPropertyMissingException("disasterId");
                        }

                        HttpFile file = Request.Files.First();
                        using (var stream = new MemoryStream())
                        {
                            file.Value.CopyTo(stream);
                            UserSession userSession = this.UserSession();
                            commandDispatcher.Dispatch(userSession,
                                                       new AddImageToDisaster(disasterId,
                                                                              stream));
                        }

                        return null;
                    };
        }
    }
}