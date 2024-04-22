using DiplomWebShopVN.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomWebShopVN.Models
{
    public class Order
    {
        [Key]
        public int? Id { get; set; }

        public DateTime? OrderDateTime { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }
        
        public string? DeliveryMethod { get; set; }

        [Required(ErrorMessage = "Введіть Область")]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string? DeliveryRegion { get; set; }

        [Required(ErrorMessage = "Введіть Місто")]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string? DeliveryCity { get; set; }

        [Required(ErrorMessage = "Введіть Адресу/Відділення")]
        [StringLength(100, ErrorMessage = "Not More Than 100.")]
        public string? DeliveryAdress { get; set; }

        public string? PayMethod { get; set; }

        [StringLength(300, ErrorMessage = "Not More Than 300.")]
        public string? UserComents { get; set; }

        public string? ManagerId { get; set; }
        public string? Status { get; set; } // Status : new / active / in progress / complited / canceled .

        public int? OrderPrice { get; set; } = 0;
        public int? CountProducts { get; set; } = 0;

        [StringLength(1000, ErrorMessage = "Not More Than 1000.")]
        public string? OrderDetails { get; set; }

        public int? Discont { get; set; }

        [Required(ErrorMessage = "Введіть Ім'я отримувача")]
        [StringLength(30, ErrorMessage = "Not More Than 50.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введіть Прізвище отримувача")]
        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string Prizvyshe { get; set; }

        [Required(ErrorMessage = "Введіть телефон отримувача для звьязку.")]
        [StringLength(30, ErrorMessage = "Not More Than 30.")]
        public string Phone { get; set; }

        [StringLength(50, ErrorMessage = "Not More Than 50.")]
        public string? Email { get; set; }

        public bool isNotRegitered { get; set; } = false;
    }
}
