using System;
using System.IO;

namespace FireTower.Domain.Services
{
    public interface IImageRepository
    {
        Uri Save(MemoryStream imageStream);
    }
}