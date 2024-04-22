using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomWebShopVN.Models
{
    public class Photo
    {
        public int Id { get; set; }

        public string Filename { get; set; } = null!;

        public string PhotoUrl { get; set; } = null!;

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public Product? Product { get; set; } = null!;
    }
}
