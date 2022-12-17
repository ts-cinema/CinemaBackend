namespace Cinema.Backend.Service.Models
{
    public class Rating
    {
        public Guid Id { get; set; }

        public Guid MovieId { get; set; }

        public double Value { get; set; }

        //dodaj UserId
    }
}
