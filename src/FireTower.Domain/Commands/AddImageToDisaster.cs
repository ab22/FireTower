using System;
using System.IO;

namespace FireTower.Domain.Commands
{
    public class AddImageToDisaster
    {
        public readonly Guid DisasterId;
        public readonly MemoryStream ImageStream;

        public AddImageToDisaster(Guid disasterId, MemoryStream imageStream)
        {
            DisasterId = disasterId;
            ImageStream = imageStream;
        }
    }
}