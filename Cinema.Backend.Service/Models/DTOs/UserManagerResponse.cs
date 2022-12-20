namespace Cinema.Backend.Service.Models.DTOs
{
    public class UserManagerResponse
    {
        public string Message { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public string? AccessToken { get; set; }

        public DateTime? ExpireDate { get; set; }
    }
}
