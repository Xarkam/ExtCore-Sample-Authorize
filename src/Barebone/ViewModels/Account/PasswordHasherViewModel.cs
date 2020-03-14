using System.ComponentModel.DataAnnotations;

namespace Barebone.ViewModels.Account
{
    public class PasswordHasherViewModel
    {
        [Display(Name = "Password to hash")]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 8)]
        public string PasswordToHash { get; set; }
    }
}