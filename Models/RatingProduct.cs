using DiplomWebShopVN.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomWebShopVN.Models
{
    public class RatingProduct
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(productInRating))]
        public int ProductId { get; set; }
        public Product productInRating { get; set; } = null!;

        [Range(1, 5, ErrorMessage = "Value can be from {1} to {5}.")]
        public int Rating { get; set; }

    }
}
