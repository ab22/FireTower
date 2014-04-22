using System;
using DomainDrivenDatabaseDeployer;
using FireTower.Domain.Entities;
using FireTower.Domain.Services;
using NHibernate;

namespace DatabaseDeploymentTool
{
    public class UserSeeder : IDataSeeder
    {
        readonly ISession _session;

        public UserSeeder(ISession session)
        {
            _session = session;
        }

        #region IDataSeeder Members

        public void Seed()
        {
            var location = new Location {LocationId = 106781442691621, LocationName = "San Pedro Sula, Cortes"};
            _session.Save(new User
                {
                    FirstName = "Byron",
                    LastName = "Sommardahl",
                    Name = "Byron Sommardahl",
                    FacebookId = "1817134138",
                    Locale = "es_ES",
                    Username = "bsommardahl",
                    Verified = true,
                    Location = location
                });

            var user = new User
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Name = "Test Test",
                    Email = "test@test.com",
                    EncryptedPassword = new HashPasswordEncryptor().Encrypt("password").Password,
                    FacebookId = "1937134326",
                    Locale = "es_ES",
                    Username = "ttest",
                    Verified = true,
                    Location = location
                };
            _session.Save(user);

            _session.Save(new UserSession
                {
                    Expires = DateTime.Now.AddYears(1),
                    User = user,
                    Id = Guid.NewGuid()
                });
        }

        #endregion
    }
}