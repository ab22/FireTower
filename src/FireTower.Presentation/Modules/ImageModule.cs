using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FireTower.Domain;
using FireTower.Domain.Commands;
using FireTower.Domain.Entities;
using FireTower.Domain.Services;
using FireTower.Infrastructure;
using FireTower.Infrastructure.Exceptions;
using Nancy;

namespace FireTower.Presentation.Modules
{
    public class ImageModule : NancyModule
    {
        public ImageModule(ICommandDispatcher commandDispatcher, IImageRepository imageRepository)
        {
            Post["/disasters/{disasterId}/image"] =
                r =>
                    {
                        Guid disasterId = Guid.Parse((string) r.disasterId);
                        if (disasterId == Guid.Empty)
                        {
                            throw new UserInputPropertyMissingException("disasterId");
                        }

                        HttpFile file = Request.Files.First();
                        var stream = new MemoryStream();
                        file.Value.CopyTo(stream);
                        UserSession userSession = this.UserSession();

                        Task.Run(() =>
                                     {
                                         Uri uri = imageRepository.Save(stream);
                                         commandDispatcher.Dispatch(userSession,
                                                                    new AddImageToDisaster(disasterId,
                                                                                           uri));
                                         stream.Dispose();
                                     });

                        return null;
                    };
        }
    }
}