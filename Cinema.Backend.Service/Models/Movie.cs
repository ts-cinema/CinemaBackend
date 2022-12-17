namespace Cinema.Backend.Service.Models
{
    public class Movie
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public List<Guid> MovieProjectionIds { get; set; } = new List<Guid>();
    }
}
