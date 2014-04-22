using System;

namespace FireTower.Domain.Entities
{
    public class User : IEntity
    {
        public virtual string FacebookId { get; set; }
        public virtual string Name { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool Verified { get; set; }
        public virtual string Locale { get; set; }
        public virtual string Username { get; set; }
        public virtual Location Location { get; set; }

        #region IEntity Members

        public virtual Guid Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string EncryptedPassword { get; set; }

        public virtual bool Activated { get; set; }

        #endregion
    }
}