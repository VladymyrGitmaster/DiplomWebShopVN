using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomWebShopVN.Models
{
    public class Category
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string CategoryName { get; set; } = null!;

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Description { get; set; } = null!;

        public List<Product>? Products { get; set; }
    }
}
