namespace Demo.Presentation.ViewModels.ManagerViewModel.UserManager
{
    public class UserEditViewModel
    {
        public string Fname { get; set; } = null!;
        public string Lname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public List<RoleSelectionViewModel> Roles { get; set; } = new();
    }
}
