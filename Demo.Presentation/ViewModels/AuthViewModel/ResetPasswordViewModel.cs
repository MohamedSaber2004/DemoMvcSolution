namespace Demo.Presentation.ViewModels.AuthViewModel
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Password Is Required")]
        public string Password { get; set; }
        [DataType(DataType.Password),Compare(nameof(Password))]
        [Required(ErrorMessage ="Confirmed Password Is Required")]
        public string ConfirmPassword { get; set; }
    }
}
