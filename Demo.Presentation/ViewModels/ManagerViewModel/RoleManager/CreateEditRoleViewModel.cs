using Demo.DataAccess.Models.Shared.Enums;

namespace Demo.Presentation.ViewModels.ManagerViewModel.RoleManager
{
    public class CreateEditRoleViewModel
    {
        [Required(ErrorMessage ="Role name field Is Required")]
        public Role RoleName { get; set; } 
    }
}
