namespace FireTower.Presentation.Requests
{
    public class NewUserRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Name { get; set; }

        public string FacebookId { get; set; }

        public string Locale { get; set; }

        public string Username { get; set; }

        public bool Verified { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}