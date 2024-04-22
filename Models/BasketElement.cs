using DiplomWebShopVN.Repositories.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiplomWebShopVN.Models
{
    public class BasketElement
    {
        [Key]
        public int? Id { get; set; }

        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(ProductInBasket))]
        public int? ProductId { get; set; }
        public Product? ProductInBasket { get; set; } = null!;

        public int? OrderId { get; set; } = null!;

        public int? Count { get; set; } = null!;

        public int? CurrentPrice { get; set; } = null!;

    }
}
