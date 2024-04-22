using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomWebShopVN.Models
{
    public class Product
    {
            [Key]
            public int? Id { get; set; }

            [Required]
            public string Name { get; set; } = null!;

            [Required]
            public string ShortDescription { get; set; } = null!;

            [Required]
            public string LongDescription { get; set; } = null!;

            public string? Image { get; set; } = null!;

            public ushort? Price { get; set; }

            public int? Count { get; set; }  

            public int? Rating { get; set; }

            public int? Votes { get; set; }

            [Required]
            [ForeignKey(nameof(ProductCategory))]
            public int? CategoryId { get; set; }

            public Category? ProductCategory { get; set; }

            public List<Photo>? Photos { get; set; }
    }
}
