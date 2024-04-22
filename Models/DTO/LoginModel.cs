using System.ComponentModel.DataAnnotations;

namespace DiplomWebShopVN.Models.DTO
{
    public class LoginModel 
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
