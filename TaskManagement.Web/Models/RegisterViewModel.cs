namespace TaskManagement.Web.Models
{
    public class RegisterViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string? Email { get; set; }

        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string? Tokken { get; set; } = null!;

        public DateTime? CreatedOn { get; set; }
    }
}
