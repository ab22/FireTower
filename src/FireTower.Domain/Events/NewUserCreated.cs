namespace FireTower.Domain.Events
{
    public class NewUserCreated
    {
        public string FacebookId { get; private set; }

        public NewUserCreated(string facebookId)
        {
            FacebookId = facebookId;
        }
    }
}