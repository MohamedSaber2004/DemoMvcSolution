namespace Demo.Presentation.ViewModels.ManagerViewModel.UserManager
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string Fname { get; set; } = null!;
        public string Lname { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
