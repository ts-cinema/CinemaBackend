namespace Cinema.Backend.Service.Models
{
    public class MovieProjection
    {   
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }

        public int TotalSeats { get; set; }

        public int AvailableSeats { get; set; }

        public Guid MovieId { get; set; }
    }
}
