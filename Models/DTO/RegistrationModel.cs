using System.ComponentModel.DataAnnotations;

namespace DiplomWebShopVN.Models.DTO
{
    public class RegistrationModel
    {
        /*[Required(ErrorMessage ="Please Enter Your Name.")]
        [Display(Name ="Name")]
        [StringLength(30,ErrorMessage = "Not More Than 30.")]*/

        [Required]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string Prizvyshe { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string  Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string  Email { get; set; } = null!;

        [Required]
        [Phone]
        [StringLength(20, ErrorMessage = "Not More Than 20.")]
        public string Phone { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string  UserName { get; set; } = null!;

        

      
        [Required]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        /*[RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$",
        ErrorMessage = "Мінімум 6 символів, 1 велика [A-Z], 1 цифра [0-9], 1 знак [#$^+=!*()@%&].")]*/
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&./-]).{6,}$",
        ErrorMessage = "Мінімум 6 символів, 1 велика [A-Z], 1 мала [a-z], 1 цифра [0-9], 1 знак [#$^+=!*()@%&./-].")]
        public string  Password { get; set; } = null!;

        [Required]
        [Compare("Password")]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string PasswordConfirm { get; set; } = null!;

        //Roles can be = admin / manager /user .
        public string? Role { get; set; }
    }
}
