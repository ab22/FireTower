using System;

namespace FireTower.Domain.Commands
{
    public class AddImageToDisaster
    {
        public readonly Guid DisasterId;
        public readonly Uri ImageUri;

        public AddImageToDisaster(Guid disasterId, Uri imageUri)
        {
            DisasterId = disasterId;
            ImageUri = imageUri;
        }
    }
}