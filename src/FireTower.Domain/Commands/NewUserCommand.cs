using FireTower.Domain.Entities;

namespace FireTower.Domain.Commands
{
    public class NewUserCommand
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        public string FacebookId { get; set; }

        public string Locale { get; set; }

        public string Username { get; set; }

        public Location Location { get; set; }

        public string Email { get; set; }

        public EncryptedPassword EncryptedPassword { get; set; }
    }
}