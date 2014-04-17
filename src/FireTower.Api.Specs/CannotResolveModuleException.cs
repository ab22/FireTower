using System;

namespace FireTower.Api.Specs
{
    public class CannotResolveModuleException : Exception
    {
        public CannotResolveModuleException(Type moduleType, Exception exception):base(string.Format("Cannot resolve '{0}' because of an unregistered dependency.", moduleType.Name), exception)
        {            
        }
    }
}