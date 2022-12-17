namespace Cinema.Backend.Service.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Price { get; set; }

        public Guid MovieProjectionId { get; set; }

        //dodati UserId
    }
}
