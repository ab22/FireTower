namespace FireTower.Presentation.Requests
{
    public class CreateNewDisasterRequest
    {
        public string LocationDescription { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }        
    }
}